using System.ComponentModel.DataAnnotations;

namespace student_crud.Models
{
    public class State
    {
        [Key]
        public int SId { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

