using student_crud.Data;

namespace student_crud.Models
{
    public class CourseStudent
    {
        public int Id { get; set; }
        public int CourseId {get; set; }
        public int StudentId { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}
