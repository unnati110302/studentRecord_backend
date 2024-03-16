using System.ComponentModel.DataAnnotations;

namespace student_crud.Models
{
    public class StudentDAO
    {
        public int? ID { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int? State { get; set; }
        public int? City { get; set; }
        public string? StateName { get; set; }

        public string? CityName { get; set; }
        public int? Gender { get; set; }
        public int? Status { get; set; }
        public int? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
