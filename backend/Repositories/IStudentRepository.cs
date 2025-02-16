using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Student;
using backend.Entities;

namespace backend.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id);
        Task<Student> CreateStudentAsync(CreateStudentDto student);
        Task<bool> UpdateStudentAsync(UpdateStudentDto student);
        Task<bool> DeleteStudentAsync(int id);
    }
}