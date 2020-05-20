using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication.DAL;
using WebApplication.Dtos;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentController(IDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpPost]
        public IActionResult Post(StudentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.IndexNumber) 
                || string.IsNullOrWhiteSpace(dto.FirstName)
                || string.IsNullOrWhiteSpace(dto.LastName) 
                || string.IsNullOrWhiteSpace(dto.BirthDate)
                || !new Regex("^\\d{1,2}\\.\\d{1,2}\\.\\d{4}$").IsMatch(dto.BirthDate))
            {
                return BadRequest("Format danych jest nieprawidłowy");
            }
            
            var studiesId = _dbService.GetStudiesIdByName(dto.Studies);
            
            if (!studiesId.HasValue)
            {
                return BadRequest($"Nie znaleziono studiów o nazwie = {dto.Studies}");
            }

            if (!_dbService.IsIndexNumberUnique(dto.IndexNumber))
            {
                return BadRequest("Student o podanym id już istnieje");
            }
            
            _dbService.CreateStudent(dto, studiesId.Value);

            return Created("", "");
        }

        [HttpPost("promotions")]
        public IActionResult Promotions(PromotionsDto dto)
        {
            var studyId = _dbService.GetStudiesIdByName(dto.Studies);
            if (!studyId.HasValue) return NotFound();

            var enrollmentId = _dbService.GetEnrollmentByStudyIdAndSemester(studyId.Value, dto.Semester);
            if (!enrollmentId.HasValue) return NotFound();
            
            _dbService.PromoteStudents(studyId.Value, dto.Semester);

            return Created("", "");
        }
    }
}