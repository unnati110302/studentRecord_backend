using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using student_crud.Data;
using student_crud.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace student_crud.Controllers
{
    public class TeacherSubjectController : Controller
    {
        private readonly StudentContext _studentContext;

        public TeacherSubjectController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        [Route("api/teacherSubjects")]
        public async Task<ActionResult> GetTeacherSubjects()
        {
            if (_studentContext.TeacherSubjects == null)
            {
                return NotFound();
            }
            var sql = "SELECT t.Id,t.TeacherId,t.IsActive,t.CourseId,t.ClassId,t.SectionId,STRING_AGG(g.Name, ',') AS SubjectId,t.CreatedBy,t.CreatedOn,t.ModifiedBy,t.ModifiedOn,d.Name AS CourseName,e.Name AS ClassName,f.Name AS SectionName FROM TeacherSubjects AS t LEFT OUTER JOIN Courses d ON t.CourseId = d.Id LEFT OUTER JOIN Classes e ON t.ClassId = e.Id LEFT OUTER JOIN ClassSections f ON t.SectionId = f.Id LEFT OUTER JOIN ClassSubjects g ON CHARINDEX(',' + CAST(g.Id AS NVARCHAR(MAX)) + ',', ',' + t.SubjectId + ',') > 0 GROUP BY t.Id,t.TeacherId,t.IsActive,t.CourseId, t.ClassId, t.SectionId,t.CreatedBy, t.CreatedOn,t.ModifiedBy,t.ModifiedOn,d.Name, e.Name, f.Name ";
            var teacherSubjects = await _studentContext.TeacherSubjectDAOs.FromSqlRaw(sql).ToListAsync();

            return Ok(teacherSubjects);
        }

        [HttpGet, Authorize]
        [Route("api/subjectBy/{teacherId}")]
        public async Task<ActionResult> GetSubjectByTeacherId(int teacherId)
        {
            var teacherIdParam = new SqlParameter("@TeacherId", teacherId);
            var teacherSubjects = await _studentContext.TeacherSubjectDAOs.FromSqlRaw("EXEC tsc_GetTeacherSubjects @TeacherId", teacherIdParam).ToListAsync();
            if (teacherSubjects.Count == 0)
            {
                return NotFound();
            }

            return Ok(teacherSubjects);
        }

        [HttpPost, Authorize]
        [Route("api/teacherSubjects")]
        public async Task<ActionResult<TeacherSubjects>> PostTeacherSubject([FromBody] TeacherSubjects teacherSubject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacherIdParam = new SqlParameter("@TeacherId", teacherSubject.TeacherId);
            var isActiveParam = new SqlParameter("@IsActive", teacherSubject.IsActive);
            var courseIdParam = new SqlParameter("@CourseId", teacherSubject.CourseId);
            var classIdParam = new SqlParameter("@ClassId", teacherSubject.ClassId);
            var sectionIdParam = new SqlParameter("@SectionId", teacherSubject.SectionId);
            var subjectIdsParam = new SqlParameter("@SubjectIds", teacherSubject.SubjectId);
            var createdByParam = new SqlParameter("@CreatedBy", teacherSubject.CreatedBy);
            var createdOnParam = new SqlParameter("@CreatedOn", teacherSubject.CreatedOn);
            var modifiedByParam = new SqlParameter("@ModifiedBy", teacherSubject.ModifiedBy);
            var modifiedOnParam = new SqlParameter("@ModifiedOn", teacherSubject.ModifiedOn);

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int);
            newIdParam.Direction = ParameterDirection.Output;

            await _studentContext.Database.ExecuteSqlRawAsync("EXEC tsc_InsertTeacherSubject @TeacherId, @IsActive, @CourseId, @ClassId, @SectionId, @SubjectIds, @CreatedBy, @CreatedOn, @ModifiedBy, @ModifiedOn, @NewId OUTPUT",
                teacherIdParam, isActiveParam, courseIdParam, classIdParam, sectionIdParam, subjectIdsParam, createdByParam, createdOnParam, modifiedByParam, modifiedOnParam, newIdParam);

            int newId = (int)newIdParam.Value;

            teacherSubject.Id = newId;

            return CreatedAtAction(nameof(GetTeacherSubjects), new { id = newId }, teacherSubject);
        }


        /*[HttpPost]
        [Route("api/teacherSubjects")]
        public async Task<ActionResult<TeacherSubjects>> PostCourse([FromBody] TeacherSubjects subject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _studentContext.TeacherSubjects.Add(subject);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacherSubjects), new { id = subject.Id }, subject);
        }*/



        [HttpGet, Authorize]
        [Route("api/getTeacherSubject/{id}")]
        public async Task<ActionResult<TeacherSubjects>> GetSubjects(int id)
        {
            var IdParam = new SqlParameter("@Id", id);
            var subjectsList = await _studentContext.TeacherSubjects
                .FromSqlRaw($"EXEC tsc_GetTeacherSubjectById @Id", IdParam)
                .ToListAsync();

            var subject = subjectsList.FirstOrDefault();

            if (subject == null)
            {
                return NotFound();
            }
            return subject;
        }

        [HttpPut, Authorize]
        [Route("api/updateTeacherSubjects/{id}")]
        public async Task<ActionResult<TeacherSubjects>> PutSubjects(int id, [FromBody] TeacherSubjects subjects)
        {
            var idParam = new SqlParameter("@Id", id);
            var teacherIdParam = new SqlParameter("@TeacherId", subjects.TeacherId);
            var courseIdParam = new SqlParameter("@CourseId", subjects.CourseId);
            var classIdParam = new SqlParameter("@ClassId", subjects.ClassId);
            var sectionIdParam = new SqlParameter("@SectionId", subjects.SectionId);
            var subjectIdParam = new SqlParameter("@SubjectId", subjects.SubjectId);
            var isActiveParam = new SqlParameter("@IsActive", subjects.IsActive);
            var modifiedByParam = new SqlParameter("@ModifiedBy", subjects.ModifiedBy);
            var modifiedOnParam = new SqlParameter("@ModifiedOn", DateTime.UtcNow);

            var result = await _studentContext.Database.ExecuteSqlRawAsync("EXEC tsc_UpdateTeacherSubjects @Id, @TeacherId, @CourseId, @ClassId, @SectionId, @SubjectId, @IsActive, @ModifiedBy, @ModifiedOn",
                idParam, teacherIdParam, courseIdParam, classIdParam, sectionIdParam, subjectIdParam, isActiveParam, modifiedByParam, modifiedOnParam);

            if (result > 0)
            {
                return Ok(id);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("api/delete-teacherSubjects")]
        public async Task<ActionResult> DeleteSubjects([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var idsString = string.Join(",", ids);

            var affectedRows = await _studentContext.Database.ExecuteSqlRawAsync(
                "EXEC tsc_DeleteTeacherSubjects @Ids",
                new SqlParameter("@Ids", idsString)
            );

            if (affectedRows > 0)
            {
                return Ok("Selected subjects have been deleted.");
            }
            else
            {
                return NotFound("No matching subjects found for deletion.");
            }
        }
    }
}
