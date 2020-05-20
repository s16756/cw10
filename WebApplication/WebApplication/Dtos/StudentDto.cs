using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class StudentDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string IndexNumber { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        public int Semester { get; set; }
        
        public string StudyName { get; set; }
    }
}