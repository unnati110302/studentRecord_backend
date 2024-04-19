using student_crud.Data;

namespace student_crud.Models
{
    public class ClassStudent
    {
        public int Id { get; set; }
        public int ClassId {  get; set; }
        public int StudentId {  get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public Class Class { get; set; }
        public Student Student { get; set; }
    }
}
