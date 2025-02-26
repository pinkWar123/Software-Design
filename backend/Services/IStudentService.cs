using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Student;
using backend.Entities;

namespace backend.Services
{
    public interface IStudentService
    {
        Task ImportFromCsv(Stream stream);
        Task ImportFromJson(Stream stream);
        Task<byte[]> ExportToCsv();
        Task<byte[]> ExportToJson();

        Task<Student> CreateNewStudent(CreateStudentDto student);
        Task<Student?> GetStudentById(int id);
        Task<Student?> UpdateStudent(int studentId,UpdateStudentDto student);
        bool ValidatePhone(string phoneNumber);
        bool ValidateEmail(string email);
    }
}