using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;

namespace backend.Services
{
    public interface IFacultyService
    {
        Task<bool> DeleteFaculty(int id);
        Task<Faculty?> GetFacultyById(int id);
    }
}