using System.Collections.Generic;
using WebApplication.Dtos;
using WebApplication.Models;

namespace WebApplication.DAL
{
    public interface IDbService
    {
        public IEnumerable<StudentDto> GetStudents();

        public IEnumerable<StudentDto> GetStudent(string indexNumber);

        public int? GetStudiesIdByName(string name);

        public void CreateStudent(StudentCreateDto dto, int studiesId);

        public bool IsIndexNumberUnique(string indexNumber);

        public int? GetEnrollmentByStudyIdAndSemester(int studyId, int semester);

        public void PromoteStudents(int studiesId, int semester);
    }
}