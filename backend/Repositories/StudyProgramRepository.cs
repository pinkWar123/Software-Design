using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.StudyProgram;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class StudyProgramRepository : IStudyProgramRepository
    {
        private readonly ApplicationDbContext _context;

        public StudyProgramRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StudyProgram> CreateStudyProgram(CreateStudyProgramDto studyProgram)
        {
            var studyProgramEntity = new StudyProgram
            {
                Name = studyProgram.Name
            };
            _context.StudyPrograms.Add(studyProgramEntity);
            await _context.SaveChangesAsync();
            return studyProgramEntity;
        }

        public async Task<IEnumerable<StudyProgram>> GetAllStudyPrograms()
        {
            return await _context.StudyPrograms.ToListAsync();
        }

        public async Task<StudyProgram> GetStudyProgramById(int id)
        {
            return await _context.StudyPrograms.FindAsync(id);
        }

        public async Task<StudyProgram> UpdateStudyProgram(int id, UpdateStudyProgramDto studyProgram)
        {
            var studyProgramEntity = await _context.StudyPrograms.FindAsync(id);
            if (studyProgramEntity == null)
            {
                throw new Exception("Study program not found");
            }
            studyProgramEntity.Name = studyProgram.Name;
            _context.StudyPrograms.Update(studyProgramEntity);
            await _context.SaveChangesAsync();
            return studyProgramEntity;
        }
    }
}