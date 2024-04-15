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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using student_crud.Models;
using Google.Apis.Auth;
using Microsoft.Identity.Client;
using student_crud.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;



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
        [Route("api/users"), Authorize]

        public async Task<ActionResult> GetUsers()
        {
            if (_studentContext.Users == null)
            {
                return NotFound();
            }
            //var sql = "SELECT a.id as Id, a.Name, Email ,Islocked, b.UserId, STRING_AGG(c.RoleName,', ') as role,Password,SecurityQuestionId,AnswerId from users a left outer join UserRoles b on a.id = b.UserId left outer join Roles c on b.RoleId = c.id group by a.id,a.Name,a.email,islocked,b.UserId,Password,SecurityQuestionId,AnswerId; ";
            var users = await _studentContext.UserS.FromSqlRaw("EXECUTE GetUsers").ToListAsync();

            return Ok(users);
        }

        [HttpPost, Authorize]
        [Route("api/users")]
        public async Task<ActionResult> CreateUser(UserInput users)
        {
            var parameters = new[]
            {
                new SqlParameter("@Name", users.Name),
                new SqlParameter("@Email", users.Email),
                new SqlParameter("@Password", users.Password),
                new SqlParameter("@IsLocked", users.IsLocked),
                new SqlParameter("@SecurityQuestionID", users.SecurityQuestionId),
                new SqlParameter("@AnswerId", users.AnswerId),
                new SqlParameter("@Roles", users.role)
            };

            var newUserId = await _studentContext.Database.ExecuteSqlRawAsync("EXECUTE sp_insert_user @Name, @Email, @Password, @SecurityQuestionID, @AnswerId, @Roles", parameters);

            users.Id = Convert.ToInt32(newUserId);
            return CreatedAtAction(nameof(GetUsers), new { id = users.Id }, users);
        }



        [HttpPost]
        [Route("api/authenticate")]
        public async Task<IActionResult> AuthenticateUser(LoginRequest loginRequest)
        {

            var user = _studentContext.Users.FirstOrDefault(u => u.Email == loginRequest.Email);
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

        [HttpPost]
        [Route("api/authenticate/validate-google-token")]
        public async Task<IActionResult> ValidatGoogleToken([FromBody] AccessTokenRequest request)
        {
            try
            {
                string t = request.AccessToken;
                var payload = await GoogleJsonWebSignature.ValidateAsync(t);
                var email = payload.Email;
                var user = _studentContext.Users.FirstOrDefault(u => u.Email == email);
                if(user != null)
                {
                    var role = await GetUserRole(user.Id);
                    return Ok(new { UserName = user.Name,  Role = role});
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (InvalidJwtException)
            {
                return BadRequest("Invalid access token");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
                  

        [HttpPost("LoginWithMicrosoft")]
        public async Task<IActionResult> LoginWithMicrosoft(string username)
        {
            if (username is null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            try
            {
                var user = await _studentContext.Users.FirstOrDefaultAsync(u => u.Email == username);

                if (user != null)
                {
                    var role = GetUserRole(user.Id);
                    return Ok(new { UserId = user.Id, UserName = user.Name, Role =role });
                }
                else
                {
                    return BadRequest("User not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in with Microsoft: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


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

            var us = await _studentContext.Users
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (us == null || us.Count == 0)
            {
                return NotFound("No matching users found for deletion.");
            }
            var userRolesToDelete = _studentContext.UserRoles
            .Where(ur => ids.Contains(ur.UserId));

            _studentContext.UserRoles.RemoveRange(userRolesToDelete);

            _studentContext.Users.RemoveRange(us);

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