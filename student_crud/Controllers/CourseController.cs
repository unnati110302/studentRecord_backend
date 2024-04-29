using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using System.Data;
using student_crud.Models;
using RSA_Angular_.NET_CORE.RSA;
using NuGet.DependencyResolver;
using Microsoft.AspNetCore.Authorization;


namespace student_crud.Controllers
{
    public class CourseController : Controller
    {
        private readonly StudentContext _studentContext;

        public CourseController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet, Authorize]
        [Route("api/courses")]
        public async Task<ActionResult> GetCourses()
        {
            if (_studentContext.Courses == null)
            {
                return NotFound();
            }
            var sql = "SELECT Id, Name ,IsActive, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn from Courses WHERE IsActive = 1 ";
            var courses = await _studentContext.Courses.FromSqlRaw(sql).ToListAsync();

            return Ok(courses);
        }

        [HttpPost, Authorize]
        [Route("api/courses")]
        public async Task<ActionResult<Course>> PostCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _studentContext.Courses.Add(course);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, course);
        }

        [HttpGet, Authorize]
        [Route("api/get{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            if (_studentContext.Courses == null)
            {
                return NotFound();
            }
            var course = await _studentContext.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpPut, Authorize]
        [Route("api/update/{id}")]
        public async Task<ActionResult<Course>> PutCourse(int id, [FromBody] Course course)
        {
            var existingCourse = await _studentContext.Courses.FirstOrDefaultAsync(t => t.Id == id);

            if (existingCourse == null)
            {
                return NotFound();
            }

            existingCourse.Name = course.Name;
            existingCourse.IsActive = course.IsActive;
            existingCourse.CreatedBy = course.CreatedBy;
            existingCourse.CreatedOn = DateTime.UtcNow;
            existingCourse.ModifiedBy = course.ModifiedBy;
            existingCourse.ModifiedOn = DateTime.UtcNow;


            await _studentContext.SaveChangesAsync();

            return Ok(course.Id);


        }


        [HttpDelete]
        [Route("api/delete-courses")]
        public async Task<ActionResult> DeleteCourses([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var courses = await _studentContext.Courses
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (courses == null || courses.Count == 0)
            {
                return NotFound("No matching courses found for deletion.");
            }

            foreach (var course in courses)
            {
                course.IsActive = 0;
            }

            await _studentContext.SaveChangesAsync();

            return Ok("Selected courses have been deleted.");
        }

    }
}
