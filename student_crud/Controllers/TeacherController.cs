using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using student_crud.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace student_crud.Controllers
{
    public class TeacherController : Controller
    {
        private readonly StudentContext _studentContext;

        public TeacherController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet, Authorize]
        [Route("api/teachers")]
        public async Task<ActionResult> GetTeachers()
        {
            if (_studentContext.Teacher == null)
            {
                return NotFound();
            }
            var teachers = await _studentContext.Teacher.FromSqlRaw("Exec tc_GetActiveTeachers").ToListAsync();

            return Ok(teachers);
        }

        [HttpPost]
        [Route("api/addTeachers")]
        public async Task<IActionResult> AddTeacher(
            string name,
            string email,
            string mobile,
            string subjectIds,
            int isActive,
            int createdBy,
            DateTime createdOn,
            int modifiedBy,
            DateTime modifiedOn)
            {
                var parameters = new[]
                {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email },
                new SqlParameter("@Mobile", SqlDbType.NVarChar) { Value = mobile },
                new SqlParameter("@subjectIds", SqlDbType.NVarChar) { Value = subjectIds ?? (object)DBNull.Value },
                new SqlParameter("@IsActive", SqlDbType.Int) { Value = isActive },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = createdBy },
                new SqlParameter("@CreatedOn", SqlDbType.DateTime) { Value = createdOn },
                new SqlParameter("@ModifiedBy", SqlDbType.Int) { Value = modifiedBy },
                new SqlParameter("@ModifiedOn", SqlDbType.DateTime) { Value = modifiedOn }
            };

                var teacherId = await _studentContext.Database
                    .ExecuteSqlRawAsync("EXECUTE dbo.sp_Addteacher @Name, @Email, @Mobile, @subjectIds, @IsActive, @CreatedBy, @CreatedOn, @ModifiedBy, @ModifiedOn", parameters);

                if (teacherId <= 0)
                {
                    return NotFound();
                }

                return Ok(new { TeacherId = teacherId });
        }

        [HttpPost, Authorize]
        [Route("api/teachers")]
        public async Task<ActionResult<Teacher>> PostCourse([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nameParam = new SqlParameter("@Name", teacher.Name);
            var emailParam = new SqlParameter("@Email", teacher.Email);
            var mobileParam = new SqlParameter("@Mobile", teacher.Mobile);
            var createdByParam = new SqlParameter("@CreatedBy", teacher.CreatedBy);
            var createdOnParam = new SqlParameter("@CreatedOn", teacher.CreatedOn);
            var modifiedByParam = new SqlParameter("@ModifiedBy", teacher.ModifiedBy);
            var modifiedOnParam = new SqlParameter("@ModifiedOn", teacher.ModifiedOn);

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int);
            newIdParam.Direction = ParameterDirection.Output;

            await _studentContext.Database.ExecuteSqlRawAsync("EXEC tc_AddTeacher @Name, @Email, @Mobile, @CreatedBy, @CreatedOn, @ModifiedBy, @ModifiedOn, @NewId OUTPUT",
                nameParam, emailParam, mobileParam, createdByParam, createdOnParam, modifiedByParam, modifiedOnParam, newIdParam);

            int newId = (int)newIdParam.Value;

            teacher.Id = newId;

            return CreatedAtAction(nameof(GetTeachers), new { id = newId }, teacher);
        }



        /*[HttpPost]
        [Route("api/teachers")]
        public async Task<ActionResult<Teacher>> PostCourse([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _studentContext.Teacher.Add(teacher);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeachers), new { id = teacher.Id }, teacher);
        }*/


        [HttpGet, Authorize]
        [Route("api/get/{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            var idParam = new SqlParameter("@Id", id);

            var teachers = await _studentContext.Teacher
                .FromSqlRaw("EXEC tc_GetTeacherById @Id", idParam)
                .ToListAsync();
            var teacher = teachers.FirstOrDefault();

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }


        [HttpPut, Authorize]
        [Route("api/modify/{id}")]
        public async Task<ActionResult<Teacher>> PutCourse(int id, [FromBody] Teacher teacher)
        {
            var idParam = new SqlParameter("@Id", id);
            var nameParam = new SqlParameter("@Name", teacher.Name);
            var emailParam = new SqlParameter("@Email", teacher.Email);
            var mobileParam = new SqlParameter("@Mobile", teacher.Mobile);
            var isActiveParam = new SqlParameter("@IsActive", teacher.IsActive);
            var modifiedByParam = new SqlParameter("@ModifiedBy", teacher.ModifiedBy);

            var result = await _studentContext.Database.ExecuteSqlRawAsync(
                "EXEC tc_UpdateTeacher @Id, @Name, @Email, @Mobile, @IsActive, @ModifiedBy",
                idParam, nameParam, emailParam, mobileParam, isActiveParam, modifiedByParam
            );

            if (result > 0)
            {
                return Ok(teacher.Id);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("api/delete-teachers")]
        public async Task<ActionResult> DeleteTeachers([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var idsString = string.Join(",", ids);

            var affectedRows = await _studentContext.Database.ExecuteSqlRawAsync(
                "EXEC tc_DeleteTeachers @Ids",
                new SqlParameter("@Ids", idsString)
            );

            if (affectedRows > 0)
            {
                return Ok("Selected teachers have been deleted.");
            }
            else
            {
                return NotFound("No matching teachers found for deletion.");
            }
        }

    }
}
