using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Text.Json;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using backend.Entities;
using backend.Dtos.Student;
using backend.Data;

namespace backend.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ImportFromCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            var records = csv.GetRecords<StudentImportDto>();
            var faculty = await _context.Faculties.FirstOrDefaultAsync();
            var program = await _context.StudyPrograms.FirstOrDefaultAsync();
            var students = records.Select(r => new Student
            {
                FullName = r.FullName,
                DateOfBirth = r.DateOfBirth,
                Gender = r.Gender,
                Batch = r.Batch,
                Address = r.Address,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                StatusId = 1,
                FacultyId = faculty.Id,
                ProgramId = program.Id,
            });

            await _context.Students.AddRangeAsync(students);
            await _context.SaveChangesAsync();
        }

        public async Task ImportFromJson(Stream stream)
        {
            try
            {
                // Đọc stream thành string
                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                // Parse JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var studentData = JsonSerializer.Deserialize<StudentJsonImport>(jsonContent, options);
                
                if (studentData?.Students == null || !studentData.Students.Any())
                    throw new Exception("Không có dữ liệu sinh viên trong file");

                // Chuyển đổi dữ liệu và thêm vào database
                var faculty = await _context.Faculties.FirstOrDefaultAsync();
                var program = await _context.StudyPrograms.FirstOrDefaultAsync();
                var students = studentData.Students.Select(r => new Student
                {
                    FullName = r.FullName,
                    DateOfBirth = r.DateOfBirth,
                    Gender = r.Gender,
                    Batch = r.Batch,
                    Address = r.Address,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    StatusId = 1,
                    FacultyId = faculty.Id,
                    ProgramId = program.Id,
                });

                await _context.Students.AddRangeAsync(students);
                await _context.SaveChangesAsync();
            }
            catch (JsonException ex)
            {
                throw new Exception("File JSON không đúng định dạng: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi import: " + ex.Message);
            }
        }

        public async Task<byte[]> ExportToCsv()
        {
            var students = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Program)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(students.Select(s => new StudentExportDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                // ... map other properties
            }));

            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportToJson()
        {
            var students = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Program)
                .ToListAsync();

            var dtos = students.Select(s => new StudentExportDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                // ... map other properties
            });

            return JsonSerializer.SerializeToUtf8Bytes(dtos);
        }
    }

    // Class để deserialize JSON
    public class StudentJsonImport
    {
        public List<StudentImportDto> Students { get; set; }
    }
}