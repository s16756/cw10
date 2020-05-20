using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    [Table("Enrollment")]
    public class Enrollment
    {
        [Key]
        public int IdEnrollment { get; set; }
        
        public int Semester { get; set; }
        
        [ForeignKey(nameof(Study))]
        public int IdStudy { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public virtual Study Study { get; set; }
    }
}