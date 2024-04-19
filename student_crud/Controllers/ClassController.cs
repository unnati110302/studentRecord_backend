using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using student_crud.Models;


namespace student_crud.Controllers
{
    public class ClassController : Controller
    {
        private readonly StudentContext _studentContext;

        public ClassController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        [Route("api/classes")]
        public async Task<ActionResult> GetClasses()
        {
            if (_studentContext.Classes == null)
            {
                return NotFound();
            }
            var sql = "SELECT * from Classes WHERE IsActive = 1 ";
            var classes = await _studentContext.Classes.FromSqlRaw(sql).ToListAsync();

            return Ok(classes);
        }

        [HttpGet]
        [Route("api/getClass/{id}")]
        public async Task<ActionResult<Class>> GetClass(int id)
        {
            if (_studentContext.Classes == null)
            {
                return NotFound();
            }
            var classes = await _studentContext.Classes.FindAsync(id);
            if (classes == null)
            {
                return NotFound();
            }
            return classes;
        }

        [HttpPost]
        [Route("api/classes")]
        public async Task<ActionResult<Class>> PostCourse([FromBody] Class classes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _studentContext.Classes.Add(classes);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClasses), new { id = classes.Id }, classes);
        }

        [HttpGet]
        [Route("api/class/bycourse/{courseId}")]
        public async Task<ActionResult> GetClassesByCourse(int courseId)
        {
            var classesByCourse = _studentContext.Classes.Where(c => c.CourseId == courseId).Where(c=>c.IsActive == 1).ToList();
            if (classesByCourse.Count == 0)
            {
                return NotFound();
            }
            return Ok(classesByCourse);
        }

        [HttpPut]
        [Route("api/updateClass/{id}")]
        public async Task<ActionResult<Class>> PutCourse(int id, [FromBody] Class classes)
        {
            var existingClass = await _studentContext.Classes.FirstOrDefaultAsync(t => t.Id == id);

            if (existingClass == null)
            {
                return NotFound();
            }

            classes.CourseId = existingClass.CourseId;
            existingClass.Name = classes.Name;
            existingClass.Session = classes.Session;
            existingClass.IsActive = classes.IsActive;
            existingClass.CreatedBy = classes.CreatedBy;
            existingClass.CreatedOn = DateTime.UtcNow;
            existingClass.ModifiedBy = classes.ModifiedBy;
            existingClass.ModifiedOn = DateTime.UtcNow;


            await _studentContext.SaveChangesAsync();

            return Ok(classes.Id);


        }

        [HttpDelete]
        [Route("api/delete-classes")]
        public async Task<ActionResult> DeleteCourses([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var classes = await _studentContext.Classes
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (classes == null || classes.Count == 0)
            {
                return NotFound("No matching classes found for deletion.");
            }

            foreach (var Class in classes)
            {
                Class.IsActive = 0;
            }

            await _studentContext.SaveChangesAsync();

            return Ok("Selected classes have been deleted.");
        }



    }
}
