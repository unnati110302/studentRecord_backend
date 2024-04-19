using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using student_crud.Models;


namespace student_crud.Controllers
{
    public class SubjectController : Controller
    {
        private readonly StudentContext _studentContext;

        public SubjectController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        [Route("api/subject")]
        public async Task<ActionResult> GetSubjects()
        {
            if (_studentContext.ClassSubjects == null)
            {
                return NotFound();
            }
            var sql = "SELECT * from ClassSubjects WHERE IsActive = 1 ";
            var subject = await _studentContext.ClassSubjects.FromSqlRaw(sql).ToListAsync();

            return Ok(subject);
        }

        [HttpGet]
        [Route("api/getSubject/{id}")]
        public async Task<ActionResult<ClassSubjects>> GetSubjects(int id)
        {
            if (_studentContext.ClassSubjects == null)
            {
                return NotFound();
            }
            var subject = await _studentContext.ClassSubjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return subject;
        }

        [HttpPost]
        [Route("api/subject")]
        public async Task<ActionResult<ClassSections>> PostCourse([FromBody] ClassSubjects subject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            subject.SubCode = await GetNextSubjectCodeAsync();
            _studentContext.ClassSubjects.Add(subject);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubjects), new { id = subject.Id }, subject);
        }

        [HttpGet]
        [Route("api/subject/byclass/{classId}")]
        public async Task<ActionResult> GetSubjectByClass(int classId)
        {
            var subjectByClass = _studentContext.ClassSubjects.Where(c => c.ClassId == classId).Where(c => c.IsActive == 1).ToList();
            if (subjectByClass.Count == 0)
            {
                return NotFound();
            }
            return Ok(subjectByClass);
        }

        [HttpPut]
        [Route("api/updateSubject/{id}")]
        public async Task<ActionResult<ClassSubjects>> PutCourse(int id, [FromBody] ClassSubjects subject)
        {
            var existingSubject = await _studentContext.ClassSubjects.FirstOrDefaultAsync(t => t.Id == id);

            if (existingSubject == null)
            {
                return NotFound();
            }

            subject.ClassId = existingSubject.ClassId;
            existingSubject.SubCode = subject.SubCode;
            existingSubject.Name = subject.Name;
            existingSubject.TotalLectures = subject.TotalLectures;
            existingSubject.IsActive = subject.IsActive;
            existingSubject.CreatedBy = subject.CreatedBy;
            existingSubject.CreatedOn = DateTime.UtcNow;
            existingSubject.ModifiedBy = subject.ModifiedBy;
            existingSubject.ModifiedOn = DateTime.UtcNow;


            await _studentContext.SaveChangesAsync();

            return Ok(subject.Id);


        }

        [HttpDelete]
        [Route("api/delete-subjects")]
        public async Task<ActionResult> DeleteCourses([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var subjects = await _studentContext.ClassSubjects
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (subjects == null || subjects.Count == 0)
            {
                return NotFound("No matching classes found for deletion.");
            }

            foreach (var subject in subjects)
            {
                subject.IsActive = 0;
            }

            await _studentContext.SaveChangesAsync();

            return Ok("Selected subjectss have been deleted.");
        }

        private async Task<string> GetNextSubjectCodeAsync()
        {
            var lastSubject = await _studentContext.ClassSubjects
                                            .OrderByDescending(s => s.Id)
                                            .FirstOrDefaultAsync();

            if (lastSubject != null)
            {
                string lastCode = lastSubject.SubCode;
                if (string.IsNullOrEmpty(lastCode))
                {
                    return "A1";
                }

                string numberPart = string.Concat(lastCode.Where(char.IsDigit));
                if (string.IsNullOrEmpty(numberPart))
                {
                    return "A1";
                }
                int nextNumber = int.Parse(numberPart) + 1;
                string prefix = lastCode.Substring(0, lastCode.Length - numberPart.Length);
                return $"{prefix}{nextNumber}";
            }
            else
            {
                return "A1";
            }
        }




    }
}
