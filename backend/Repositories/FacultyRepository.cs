using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Faculty;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly ApplicationDbContext _context;

        public FacultyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Faculty> CreateFaculty(CreateFacultyDto faculty)
        {
            var newFaculty = new Faculty
            {
                Name = faculty.Name
            };
            _context.Faculties.Add(newFaculty);
            await _context.SaveChangesAsync();
            return newFaculty;
        }

        public async Task<IEnumerable<Faculty>> GetFaculties()
        {
            return await _context.Faculties.ToListAsync();
        }

        public async Task<Faculty> GetFacultyById(int id)
        {
            return await _context.Faculties.FindAsync(id);
        }

        public async Task<Faculty> UpdateFaculty(int id, UpdateFacultyDto faculty)
        {
            var existingFaculty = await _context.Faculties.FindAsync(id);
            if (existingFaculty == null)
            {
                throw new Exception("Faculty not found");
            }
            existingFaculty.Name = faculty.Name;
            await _context.SaveChangesAsync();
            return existingFaculty;
        }
    }
}