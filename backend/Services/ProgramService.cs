using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;
using backend.Repositories;

namespace backend.Services
{
    public class ProgramService : IProgramService
    {
        private readonly IStudyProgramRepository _programRepository;
        public ProgramService(IStudyProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }
        
        public async Task<bool> DeleteProgram(int id)
        {
            var program = await _programRepository.GetStudyProgramById(id);
            if(program == null)
            {
                return false;
            }

            var studentCount = await _programRepository.GetStudentCountInStudyProgram(id);
            if(studentCount > 0)
            {
                return false;
            }

            await _programRepository.DeleteStudyProgram(program);
            return true;
        }

        public async Task<StudyProgram?> GetProgramById(int id)
        {
            return await _programRepository.GetStudyProgramById(id);
        }
    }
}