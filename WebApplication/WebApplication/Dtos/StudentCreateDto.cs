using System;

namespace WebApplication.Dtos
{
    public class StudentCreateDto
    {
        public string IndexNumber { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string BirthDate { get; set; }
        
        public string Studies { get; set; }
    }
}