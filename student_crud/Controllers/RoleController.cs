using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using student_crud.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace student_crud.Controllers
{
    public class RoleController : Controller
    {
        private readonly StudentContext _studentContext;

        public RoleController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet, Authorize]
        [Route("api/roles")]
        public async Task<ActionResult> GetRole()
        {
            if (_studentContext.Roles == null)
            {
                return NotFound();
            }
            var roles = await _studentContext.Roles.FromSqlRaw("Exec rc_getRoles").ToListAsync();

            return Ok(roles);
        }

        [HttpPost, Authorize]
        [Route("api/role")]
        public async Task<ActionResult<Teacher>> PostRole([FromBody] Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nameParam = new SqlParameter("@RoleName", role.RoleName);
            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int);
            newIdParam.Direction = ParameterDirection.Output;

            await _studentContext.Database.ExecuteSqlRawAsync("EXEC rc_addRole @RoleName , @NewId OUTPUT",
                nameParam, newIdParam);

            int newId = (int)newIdParam.Value;

            role.Id = newId;

            return CreatedAtAction(nameof(GetRole), new { id = newId }, role);
        }

        [HttpGet, Authorize]
        [Route("api/getRole/{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var idParam = new SqlParameter("@Id", id);

            var roles = await _studentContext.Roles
                .FromSqlRaw("EXEC rc_GetRoleById @Id", idParam)
                .ToListAsync();
            var role = roles.FirstOrDefault();

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        [HttpPut, Authorize]
        [Route("api/modifyRole/{id}")]
        public async Task<ActionResult<Role>> PutRole(int id, [FromBody] Role role)
        {
            var idParam = new SqlParameter("@Id", id);
            var nameParam = new SqlParameter("@RoleName", role.RoleName);

            var result = await _studentContext.Database.ExecuteSqlRawAsync(
                "EXEC rc_UpdateRole @Id, @RoleName",
                idParam, nameParam
            );

            if (result > 0)
            {
                return Ok(role.Id);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("api/delete-role")]
        public async Task<ActionResult> DeleteRoles([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var idsString = string.Join(",", ids);

            var affectedRows = await _studentContext.Database.ExecuteSqlRawAsync(
                "EXEC rc_DeleteRoles @Ids",
                new SqlParameter("@Ids", idsString)
            );

            if (affectedRows > 0)
            {
                return Ok("Selected roles have been deleted.");
            }
            else
            {
                return NotFound("No matching roles found for deletion.");
            }
        }
    }
}