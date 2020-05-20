using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Dtos;
using WebApplication.Models;

namespace WebApplication.DAL
{
    public class DbService : IDbService
    {
        private const string ProcedureName = "Promotions";
        private readonly IConfiguration _configuration;
        private readonly StudentsContext _context;

        public DbService(IConfiguration configuration, StudentsContext context)
        {
            _configuration = configuration;
            _context = context;

            _context.Database.ExecuteSqlRaw(@"
CREATE OR ALTER PROCEDURE {ProcedureName}
    @StudiesId INT,
    @Semester INT
AS BEGIN
    DECLARE @NextEnrollmentId INT;
    SELECT @NextEnrollmentId = IdEnrollment FROM [Enrollment] WHERE [IdStudy] = @StudiesId AND Semester = @Semester + 1;
	IF @NextEnrollmentId IS NULL BEGIN
		INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate)
		SELECT MAX(IdEnrollment) + 1 AS IdEnrollment, @Semester + 1 as Semester, @StudiesId as IdStudy, GETDATE() as StartDate FROM Enrollment;

		SELECT @NextEnrollmentId = IdEnrollment FROM [Enrollment] WHERE [IdStudy] = @StudiesId AND Semester = @Semester + 1;
	END;

	UPDATE Student SET IdEnrollment = @NextEnrollmentId
	WHERE IdEnrollment = (SELECT TOP 1 IdEnrollment FROM [Enrollment] WHERE [IdStudy] = @StudiesId AND Semester = @Semester);
END
");
        }

        public IEnumerable<StudentDto> GetStudents()
        {
            return _context
                .Students
                .Include(s => s.Enrollment)
                .ThenInclude(s => s.Study)
                .Select(s => new StudentDto()
                {
                    IndexNumber = s.IndexNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    BirthDate = s.BirthDate,
                    Semester = s.Enrollment.Semester,
                    StudyName = s.Enrollment.Study.Name
                });
        }

        public IEnumerable<StudentDto> GetStudent(string indexNumber)
        {
            return _context
                .Students
                .Include(s => s.Enrollment)
                .ThenInclude(s => s.Study)
                .Where(s => s.IndexNumber == indexNumber)
                .Select(s => new StudentDto()
                {
                    IndexNumber = s.IndexNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    BirthDate = s.BirthDate,
                    Semester = s.Enrollment.Semester,
                    StudyName = s.Enrollment.Study.Name
                });
        }

        public int? GetStudiesIdByName(string name)
        {
            return _context.Studies.Where(s => s.Name == name).Select(s => s.IdStudy).FirstOrDefault();
        }

        public bool IsIndexNumberUnique(string indexNumber)
        {
            return !_context
                .Students
                .Any(s => s.IndexNumber == indexNumber);
        }

        public int? GetEnrollmentByStudyIdAndSemester(int studyId, int semester)
        {
            return _context
                .Enrollments
                .Where(e => e.IdStudy == studyId && e.Semester == semester)
                .Select(e => e.IdEnrollment)
                .FirstOrDefault();
        }

        public void CreateStudent(StudentCreateDto dto, int studiesId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var enrollmentId = GetEnrollmentByStudyIdAndSemester(studiesId, 1);

                if (!enrollmentId.HasValue)
                {
                    var latest = _context.Enrollments.Max(e => e.IdEnrollment);
                    
                    _context.Enrollments.Add(new Enrollment
                    {
                        IdEnrollment = latest + 1,
                        Semester = 1,
                        IdStudy = studiesId,
                        StartDate = DateTime.Now
                    });

                    enrollmentId = latest + 1;
                }
                
                var dateSplitted = dto.BirthDate.Split('.');
                var day = int.Parse(dateSplitted[0]);
                var month = int.Parse(dateSplitted[1]);
                var year = int.Parse(dateSplitted[2]);
                var parsedDate = new DateTime(year, month, day);

                _context.Students.Add(new Student()
                {
                    IndexNumber = dto.IndexNumber,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    BirthDate = parsedDate,
                    IdEnrollment = enrollmentId.Value
                });

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }

        public void PromoteStudents(int studiesId, int semester)
        {
            var connection = _context.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;
            command.Parameters.Add(new SqlParameter("@StudiesId", studiesId));
            command.Parameters.Add(new SqlParameter("@Semester", semester));

            command.ExecuteNonQuery();
        }
    }
}