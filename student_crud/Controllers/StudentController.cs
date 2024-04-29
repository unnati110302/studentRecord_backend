using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using student_crud.Data;
using System.Data;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;


namespace student_crud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentContext _studentContext;
        public StudentController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet, Authorize]

        public async Task<ActionResult<IEnumerable<StudentDAO>>> GetStudents(int studentId = 0, int pageNumber = 1, int pageSize = 5, string search = "", string sortAttribute = "", string sortOrder = "")
        {
            var parameters = new[]
            {
                    new SqlParameter("@studentId", SqlDbType.Int) { Value = studentId },
                    new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
                    new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                    new SqlParameter("@Search", SqlDbType.NVarChar) { Value = search ?? (object)DBNull.Value },
                    new SqlParameter("@SortAttribute", SqlDbType.NVarChar) { Value = sortAttribute ?? (object)DBNull.Value },
                    new SqlParameter("@SortOrder", SqlDbType.NVarChar) { Value = sortOrder ?? (object)DBNull.Value }
                };

            var result = await _studentContext.StudentDAOs
                .FromSqlRaw("EXECUTE sp_get_student_data @studentId, @PageNumber, @PageSize, @Search, @SortAttribute, @SortOrder", parameters)
                .ToListAsync();

            if (result == null || !result.Any())
            {
                return NotFound();
            }


            var paginatedStudents = await PaginatedList<StudentDAO>.Create(result, pageNumber, pageSize);

            var totalPages = paginatedStudents.TotalPages;

            return Ok(new { Data = paginatedStudents, TotalPages = totalPages });
        }


        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            if (_studentContext.Students == null)
            {
                return NotFound();
            }
            var student = await _studentContext.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await IsDuplicateEmailOrMobile(student.Email, student.Mobile))
            {
                return BadRequest(new { ErrorMessage = "A record with the same email or mobile number already exists.", DuplicateFound = true });
            }
            int maxId = await _studentContext.Students.MaxAsync(s => (int?)s.ID) ?? 0;
            int newId = maxId + 1;
            string studentCode = $"{DateTime.Now:yyyyMMdd}00{newId}";
            student.Code = studentCode;

            _studentContext.Students.Add(student);
            await _studentContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.ID }, student);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult> PutStudent(int id, Student student)
        {
            if (id != student.ID)
            {
                return BadRequest();
            }

            _studentContext.Entry(student).State = EntityState.Modified;

            try
            {
                await _studentContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            if (_studentContext.Students == null)
            {
                return NotFound();
            }

            var student = await _studentContext.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.IsActive = 0;
            await _studentContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete-multiple")]
        public async Task<ActionResult> DeleteStudents([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            var students = await _studentContext.Students
                                                .Where(s => ids.Contains(s.ID))
                                                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("No matching students found for deletion.");
            }

            foreach (var student in students)
            {
                student.IsActive = 0;
                // _studentContext.Students.Remove(student);
            }

            await _studentContext.SaveChangesAsync();

            return Ok("Selected students have been deleted.");
        }

        private async Task<bool> IsDuplicateEmailOrMobile(string email, string mobile)
        {
            return await _studentContext.Students.AnyAsync(s => s.Email == email || s.Mobile == mobile);
        }

        [HttpGet("/excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            /* var students = await _studentContext.Student.Where(s => s.IsActive == 1).ToListAsync();*/
            /* string sql = "SELECT s.Id, s.Code, s.Name, s.Email, s.Mobile, s.Address1, s.Address2, s.IsActive, s.Gender, s.MaritalStatus, s.CityId, s.CreatedBy, s.CreatedOn, s.ModifiedBy, s.ModifiedOn, s.StateId, st.Name AS StateName, c.Name AS CityName " +
                          "FROM Students s " +
                          "LEFT OUTER JOIN States st ON s.StateId = st.Id " +
                          "LEFT OUTER JOIN Cities c ON s.CityId = c.Id where s.IsActive=1";

             var students = await _studentContext.Student.FromSqlRaw(sql).ToListAsync();*/
            string sql = $"Execute sp_get_student_data 0";
            var students = await _studentContext.StudentDAOs.FromSqlRaw(sql).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");

                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Code";
                worksheet.Cell(1, 3).Value = "Name";
                worksheet.Cell(1, 4).Value = "Email";
                worksheet.Cell(1, 5).Value = "Mobile";
                worksheet.Cell(1, 6).Value = "Address1";
                worksheet.Cell(1, 7).Value = "Address2";
                worksheet.Cell(1, 8).Value = "IsActive";
                worksheet.Cell(1, 9).Value = "Gender";
                worksheet.Cell(1, 10).Value = "MaritalStatus";
                worksheet.Cell(1, 11).Value = "CityId";
                worksheet.Cell(1, 12).Value = "StateId";
                worksheet.Cell(1, 13).Value = "StateName";
                worksheet.Cell(1, 14).Value = "CityName";
                worksheet.Cell(1, 15).Value = "CourseId";
                worksheet.Cell(1, 16).Value = "ClassId";
                worksheet.Cell(1, 17).Value = "SectionId";
                worksheet.Cell(1, 18).Value = "CourseName";
                worksheet.Cell(1, 19).Value = "ClassName";
                worksheet.Cell(1, 20).Value = "SectionName";

                // Add data
                int row = 2;
                foreach (var stu in students)
                {
                    worksheet.Cell(row, 1).Value = stu.ID;
                    worksheet.Cell(row, 2).Value = stu.Code;
                    worksheet.Cell(row, 3).Value = stu.Name;
                    worksheet.Cell(row, 4).Value = stu.Email;
                    worksheet.Cell(row, 5).Value = stu.Mobile;
                    worksheet.Cell(row, 6).Value = stu.Address1;
                    worksheet.Cell(row, 7).Value = stu.Address2;
                    worksheet.Cell(row, 8).Value = stu.IsActive;
                    worksheet.Cell(row, 9).Value = (stu.Gender == 0) ? "Male" : "Female";
                    worksheet.Cell(row, 10).Value = (stu.Status == 0) ? "Single" : (stu.Status == 1 ? "Married" : "Separated");
                    worksheet.Cell(row, 11).Value = stu.City;
                    worksheet.Cell(row, 12).Value = stu.State;
                    worksheet.Cell(row, 13).Value = stu.StateName;
                    worksheet.Cell(row, 14).Value = stu.CityName;
                    worksheet.Cell(row, 15).Value = stu.CourseId;
                    worksheet.Cell(row, 16).Value = stu.ClassId;
                    worksheet.Cell(row, 17).Value = stu.SectionId;
                    worksheet.Cell(row, 18).Value = stu.CourseName;
                    worksheet.Cell(row, 19).Value = stu.ClassName;
                    worksheet.Cell(row, 20).Value = stu.SectionName;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
                }
            }
        }
    }
}