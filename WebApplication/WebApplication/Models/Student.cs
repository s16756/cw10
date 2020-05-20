using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public string IndexNumber { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        [ForeignKey(nameof(Enrollment))]
        public int IdEnrollment { get; set; }

        public Enrollment Enrollment { get; set; }
    }
}