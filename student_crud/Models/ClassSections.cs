namespace student_crud.Models
{
    public class ClassSections
    {
        public int Id { get; set; }
        public int ClassId {  get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        //public Class Class { get; set; }
    }
}
