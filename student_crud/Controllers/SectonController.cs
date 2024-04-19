using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using student_crud.Models;


namespace student_crud.Controllers
{
    public class SectionController : Controller
    {
        private readonly StudentContext _studentContext;

        public SectionController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]
        [Route("api/section")]
        public async Task<ActionResult> GetSections()
        {
            if (_studentContext.ClassSections == null)
            {
                return NotFound();
            }
            var sql = "SELECT * from ClassSections WHERE IsActive = 1 ";
            var section = await _studentContext.ClassSections.FromSqlRaw(sql).ToListAsync();

            return Ok(section);
        }

        [HttpGet]
        [Route("api/getSection/{id}")]
        public async Task<ActionResult<ClassSections>> GetSection(int id)
        {
            if (_studentContext.ClassSections == null)
            {
                return NotFound();
            }
            var section = await _studentContext.ClassSections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }
            return section;
        }

        [HttpPost]
        [Route("api/section")]
        public async Task<ActionResult<ClassSections>> PostCourse([FromBody] ClassSections section)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _studentContext.ClassSections.Add(section);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSection), new { id = section.Id }, section);
        }

        [HttpGet]
        [Route("api/section/byclass/{classId}")]
        public async Task<ActionResult> GetSectionByClass(int classId)
        {
            var sectionByClass = _studentContext.ClassSections.Where(c => c.ClassId == classId ).Where(c => c.IsActive == 1).ToList();
            if (sectionByClass.Count == 0)
            {
                return NotFound();
            }
            return Ok(sectionByClass);
        }

        [HttpPut]
        [Route("api/updateSection/{id}")]
        public async Task<ActionResult<ClassSections>> PutCourse(int id, [FromBody] ClassSections section)
        {
            var existingSection = await _studentContext.ClassSections.FirstOrDefaultAsync(t => t.Id == id);

            if (existingSection == null)
            {
                return NotFound();
            }

            section.ClassId = existingSection.ClassId;
            existingSection.Name = section.Name;
            existingSection.IsActive = section.IsActive;
            existingSection.CreatedBy = section.CreatedBy;
            existingSection.CreatedOn = DateTime.UtcNow;
            existingSection.ModifiedBy = section.ModifiedBy;
            existingSection.ModifiedOn = DateTime.UtcNow;


            await _studentContext.SaveChangesAsync();

            return Ok(section.Id);


        }

        [HttpDelete]
        [Route("api/delete-sections")]
        public async Task<ActionResult> DeleteCourses([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var sections = await _studentContext.ClassSections
                                                .Where(s => ids.Contains(s.Id))
                                                .ToListAsync();

            if (sections == null || sections.Count == 0)
            {
                return NotFound("No matching classes found for deletion.");
            }

            foreach (var section in sections)
            {
                section.IsActive = 0;
            }

            await _studentContext.SaveChangesAsync();

            return Ok("Selected sections have been deleted.");
        }



    }
}
