using System;
using System.ComponentModel.DataAnnotations;

namespace student_crud.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public int SecurityQuestionId { get; set; }
        public int AnswerId { get; set; }
         
        public string RoleIds {  get; set; }
    }
}