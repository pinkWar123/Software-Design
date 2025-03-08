using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.StudyProgram;
using backend.Entities;

namespace backend.Repositories
{
    public interface IStudyProgramRepository
    {
        Task<IEnumerable<StudyProgram>> GetAllStudyPrograms();
        Task<StudyProgram> GetStudyProgramById(int id);
        Task<StudyProgram> CreateStudyProgram(CreateStudyProgramDto studyProgram);
        Task<StudyProgram> UpdateStudyProgram(int id, UpdateStudyProgramDto studyProgram);
        Task<int> GetStudentCountInStudyProgram(int id);
        Task DeleteStudyProgram(StudyProgram studyProgram);
    }
}