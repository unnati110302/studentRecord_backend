namespace student_crud.Models
{
    public class TeacherSubjectDAO
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string SubjectId { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
