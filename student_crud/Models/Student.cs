using System;
using System.ComponentModel.DataAnnotations;

namespace student_crud.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string Code{get; set;}

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name should contain only alphabets")] 
        public string Name {get; set;}

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$", ErrorMessage = "Enter a valid email address")]
        public string Email { get; set;}

        [Required(ErrorMessage = "Mobile is required")]
        [RegularExpression("^[6789]\\d{9}$", ErrorMessage = "Mobile number should start with 6, 7, 8, or 9 and be of 10 digits")] 
        public string Mobile {get; set;}
        public string Address1 { get; set;}
        public string Address2 { get; set;}
        public int State {  get; set;}
        public int City { get; set; }
        public int Gender { get; set;}
        public int Status { get; set;}
        public int IsActive {  get; set;}
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
