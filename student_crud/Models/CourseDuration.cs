namespace student_crud.Models
{
    public class CourseDuration
    {
        public int Id {  get; set; }
        public int CourseId {  get; set; }
        public TimeSpan Start_time { get; set; }
        public TimeSpan End_time { get; set;}
        public int ClassDurationMinutes { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set;}
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public Course Course { get; set; }


    }
}
