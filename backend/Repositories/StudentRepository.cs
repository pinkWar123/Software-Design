using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Student;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> CreateStudentAsync(CreateStudentDto student)
        {
            var newStudent = new Student
            {
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Batch = student.Batch,
                Address = student.Address,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                FacultyId = student.FacultyId,
                StatusId = student.StatusId,
                ProgramId = student.ProgramId
            };

            await _context.Students.AddAsync(newStudent);
            await _context.SaveChangesAsync();
            return newStudent;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Status)
                .Include(s => s.Program)
                .Include(s => s.Faculty)
                .ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Status)
                .Include(s => s.Program)
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<bool> UpdateStudentAsync(UpdateStudentDto student)
        {
            var existingStudent = await _context.Students.FindAsync(student.StudentId);
            if (existingStudent == null)
            {
                return false;
            }

            existingStudent.FullName = student.FullName;
            existingStudent.DateOfBirth = student.DateOfBirth;
            existingStudent.Gender = student.Gender;
            existingStudent.Batch = student.Batch;
            existingStudent.Address = student.Address;
            existingStudent.Email = student.Email;
            existingStudent.PhoneNumber = student.PhoneNumber;
            existingStudent.StatusId = student.StatusId;
            existingStudent.ProgramId = student.ProgramId;
            existingStudent.FacultyId = student.FacultyId;
            

            await _context.SaveChangesAsync();
            return true;
        }
    }
}