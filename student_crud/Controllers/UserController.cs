using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using student_crud.Data;
using System.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using RSA_Angular_.NET_CORE.RSA;

namespace student_crud.Controllers
{

    public class UserController : Controller
    {
        private readonly StudentContext _studentContext;
        private readonly IRsaHelper _rsaHelper;

        public UserController(StudentContext studentContext, IRsaHelper rsaHelper)
        {
            _studentContext = studentContext;
            _rsaHelper = rsaHelper;
        }

        [HttpGet]
        [Route("api/users")]

        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_studentContext.Users == null)
            {
                return NotFound();
            }
            var users = await _studentContext.Users.FromSqlRaw("EXECUTE GetUsers").ToListAsync();

            return users;
        }

        [HttpPost]
        [Route("api/users")]
        public async Task<ActionResult> CreateUser(User users)
        {
            var parameters = new[]
            {
                new SqlParameter("@Name", users.Name),
                new SqlParameter("@Email", users.Email),
                new SqlParameter("@Password", users.Password),
                new SqlParameter("@IsLocked", users.IsLocked),
                new SqlParameter("@SecurityQuestionID", users.SecurityQuestionId),
                new SqlParameter("@AnswerId", users.AnswerId),
                new SqlParameter("@Roles", users.RoleIds)
            };

            var newUserId = await _studentContext.Database.ExecuteSqlRawAsync("EXECUTE sp_insert_user @Name, @Email, @Password, @SecurityQuestionID, @AnswerId, @Roles", parameters);

            users.Id = Convert.ToInt32(newUserId);
            return CreatedAtAction(nameof(GetUsers), new { id = users.Id }, users);
        }



        [HttpPost]
        [Route("api/authenticate")]
        public async Task<IActionResult> AuthenticateUser(LoginRequest loginRequest)
        {

            var user = _studentContext.users.FirstOrDefault(u => u.Email == loginRequest.Email);
            if (user != null)
            {

                var enteredPassword = _rsaHelper.Decrypt(loginRequest.Password);
                var storedPassword = _rsaHelper.Decrypt(user.Password);

                if (storedPassword.Equals(enteredPassword))
                {
                    var role = await GetUserRole(user.Id);
                    return Ok(new { UserId = user.Id, UserName = user.Name, Role = role });
                }
                else
                {
                    return Unauthorized();
                }
            }

            else
            {
                return Unauthorized();
            }
        }

        /*private async Task<string?> GetUserRole(StudentContext context, int userId)
        {
            var userRole = context.UserRoles.FirstOrDefault(ur => ur.UserId == userId);
            if (userRole != null)
            {
                var role = await context.Roles.FindAsync(userRole.RoleId);
                return role?.RoleName;
            }
            return null;
        }*/

        [HttpGet("role")]
        public async Task<List<string>> GetUserRole(int userId)
        {
            var userrole = _studentContext.UserRoles.Where(u => u.UserId == userId).ToList();
            if (userrole != null)
            {
                var roleids = userrole.Select(ur => ur.RoleId).ToList();
                var rolename = await _studentContext.Roles.Where(r => roleids.Contains(r.Id)).Select(r => r.RoleName).ToListAsync();
                return rolename;
            }
            return null;
        }

        [HttpDelete("delete-multiple")]
        public async Task<ActionResult> DeleteUsers([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var us = await _studentContext.users
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (us == null || us.Count == 0)
            {
                return NotFound("No matching users found for deletion.");
            }
            var userRolesToDelete = _studentContext.UserRoles
            .Where(ur => ids.Contains(ur.UserId));

            _studentContext.UserRoles.RemoveRange(userRolesToDelete);

            _studentContext.users.RemoveRange(us);

            /*foreach (var user in us)
            {
                //user.IsLocked = false;
                //_studentContext.UserRoles.Remove(Id);
                _studentContext.users.Remove(user);
            }*/

            await _studentContext.SaveChangesAsync();

            return Ok("Selected users have been deleted.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            if (_studentContext.users == null)
            {
                return NotFound();
            }

            var user = await _studentContext.users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            var userRoles = _studentContext.UserRoles.Where(ur => ur.UserId == id);
            _studentContext.UserRoles.RemoveRange(userRoles);
            //user.IsLocked = false;
            _studentContext.users.Remove(user);
            await _studentContext.SaveChangesAsync();

            return Ok();
        }

    }
}