using Microsoft.EntityFrameworkCore;

namespace student_crud.Data
{
    public class SecurityQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
    }
}