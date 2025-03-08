using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;

namespace backend.Services
{
    public interface IProgramService
    {
        Task<bool> DeleteProgram(int id);
        Task<StudyProgram?> GetProgramById(int id);
    }
}