namespace student_crud.Models
{
    public class TeacherSubjects
    {
        public int Id { get; set; }
        public int TeacherId {  get; set; }
        public int SubjectId { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public Teacher Teacher { get; set; }
        public ClassSubjects Subject { get; set; }
    }
}
