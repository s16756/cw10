using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DAL;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly StudentsContext _context;

        public StudentsController(IDbService dbService, StudentsContext context)
        {
            _dbService = dbService;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_context.Students.ToList());
        }
        
        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var student = _dbService.GetStudent(indexNumber);
            if (student != null)
                return Ok(student);
            return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateStudent(StudentDto studentDto)
        {
            var student = _context.Students.Add(new Student()
            {
                IndexNumber = studentDto.IndexNumber,
                BirthDate = studentDto.BirthDate,
                IdEnrollment = _context
                    .Enrollments
                    .Where(e => e.Study.Name == studentDto.StudyName && e.Semester == studentDto.Semester)
                    .Select(e => e.IdEnrollment)
                    .Single(),
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName
            });

            _context.SaveChanges();
            
            return Ok(student.Entity);
        }

        [HttpPut]
        public IActionResult UpdateStudent([FromBody] StudentDto studentDto)
        {
            var student = _context.Students.SingleOrDefault(s => s.IndexNumber == studentDto.IndexNumber);
            if (student == null) return BadRequest("Nie znaleziono studneta");

            student.BirthDate = studentDto.BirthDate;
            student.IdEnrollment = _context
                .Enrollments
                .Where(e => e.Study.Name == studentDto.StudyName && e.Semester == studentDto.Semester)
                .Select(e => e.IdEnrollment)
                .Single();
            student.FirstName = studentDto.FirstName;
            student.LastName = studentDto.LastName;
            
            _context.SaveChanges();
            
            return Ok(student);
        }

        [HttpDelete("{indexNumber}")]
        public IActionResult DeleteStudent(string indexNumber)
        {
            var student = _context.Students.SingleOrDefault(s => s.IndexNumber == indexNumber);
            if (student == null) return BadRequest("Nie znaleziono studenta");

            _context.Students.Remove(student);

            _context.SaveChanges();
            
            return Ok("Usuwanie ukończone");
        }
    }
}