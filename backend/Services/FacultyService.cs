using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;
using backend.Repositories;

namespace backend.Services
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _facultyRepository;
        public FacultyService(IFacultyRepository facultyRepository)
        {
            _facultyRepository = facultyRepository;
        }
        public async Task<bool> DeleteFaculty(int id)
        {
            var faculty = await GetFacultyById(id);
            if (faculty == null)
            {
                return false;
            }

            var studentCount = await _facultyRepository.GetStudentCountInFaculty(id);
            if (studentCount > 0)
            {
                return false;
            }

            return await _facultyRepository.DeleteFaculty(faculty);
        }

        public async Task<Faculty?> GetFacultyById(int id)
        {
            return await _facultyRepository.GetFacultyById(id);
        }
    }
}