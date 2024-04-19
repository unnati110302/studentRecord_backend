using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using student_crud.Models;

namespace student_crud.Controllers
{
    public class TeacherController : Controller
    {
        private readonly StudentContext _studentContext;

        public TeacherController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        [Route("api/teachers")]
        public async Task<ActionResult> GetTeachers()
        {
            if (_studentContext.Teacher == null)
            {
                return NotFound();
            }
            var sql = "SELECT Id, Name ,Email, Mobile, IsActive, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn from Teacher WHERE IsActive = 1 ";
            var teachers = await _studentContext.Teacher.FromSqlRaw(sql).ToListAsync();

            return Ok(teachers);
        }
        [HttpPost]
        [Route("api/teachers")]
        public async Task<ActionResult<Teacher>> PostTeacher([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _studentContext.Teacher.Add(teacher);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeachers), new { id = teacher.Id }, teacher);
        }

        [HttpGet]
        [Route("api/get/{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            if (_studentContext.Teacher == null)
            {
                return NotFound();
            }
            var teacher = await _studentContext.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return teacher;
        }

        [HttpPut]
        [Route("api/modify/{id}")]
        public async Task<ActionResult<Teacher>> PutCourse(int id, [FromBody] Teacher teacher)
        {
            var existingTeacher = await _studentContext.Teacher.FirstOrDefaultAsync(t => t.Id == id);

            if (existingTeacher == null)
            {
                return NotFound();
            }

            existingTeacher.Name = teacher.Name;
            existingTeacher.Email = teacher.Email;
            existingTeacher.Mobile = teacher.Mobile;
            existingTeacher.IsActive = teacher.IsActive;
            existingTeacher.CreatedBy = teacher.CreatedBy;
            existingTeacher.CreatedOn = DateTime.UtcNow; ;
            existingTeacher.ModifiedBy = teacher.ModifiedBy;
            existingTeacher.ModifiedOn = DateTime.UtcNow;


            await _studentContext.SaveChangesAsync();

            return Ok(teacher.Id);
        }


           [HttpDelete]
        [Route("api/delete-teachers")]
        public async Task<ActionResult> DeleteTeachers([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var teachers = await _studentContext.Teacher
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (teachers == null || teachers.Count == 0)
            {
                return NotFound("No matching teachers found for deletion.");
            }

            foreach (var teacher in teachers)
            {
                teacher.IsActive = 0;
            }

            await _studentContext.SaveChangesAsync();

            return Ok("Selected teachers have been deleted.");
        }
    }
}
