using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Faculty;
using backend.Entities;

namespace backend.Repositories
{
    public interface IFacultyRepository
    {
        Task<IEnumerable<Faculty>> GetFaculties();
        Task<Faculty> GetFacultyById(int id);
        Task<Faculty> CreateFaculty(CreateFacultyDto faculty);
        Task<Faculty> UpdateFaculty(int id, UpdateFacultyDto faculty);
        Task<int> GetStudentCountInFaculty(int id);
        Task<bool> DeleteFaculty(Faculty faculty);
    }
}