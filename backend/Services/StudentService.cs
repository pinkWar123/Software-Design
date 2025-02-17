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
using System.Text.Encodings.Web;
using System.Text.Unicode;

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
                .Include(s => s.Status)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Viết header
            csv.WriteField("Mã sinh viên");
            csv.WriteField("Họ và tên");
            csv.WriteField("Ngày sinh");
            csv.WriteField("Giới tính");
            csv.WriteField("Khoa");
            csv.WriteField("Chương trình");
            csv.WriteField("Email");
            csv.WriteField("Số điện thoại");
            csv.WriteField("Trạng thái");
            csv.NextRecord();

            // Viết dữ liệu
            foreach (var student in students)
            {
                csv.WriteField(student.StudentId);
                csv.WriteField(student.FullName);
                csv.WriteField(student.DateOfBirth.ToString("yyyy-MM-dd"));
                csv.WriteField(student.Gender);
                csv.WriteField(student.Faculty?.Name);
                csv.WriteField(student.Program?.Name);
                csv.WriteField(student.Email);
                csv.WriteField(student.PhoneNumber);
                csv.WriteField(student.Status?.Name);
                csv.NextRecord();
            }

            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportToJson()
        {
            var students = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Program)
                .Include(s => s.Status)
                .Select(s => new
                {
                    s.StudentId,
                    s.FullName,
                    DateOfBirth = s.DateOfBirth.ToString("dd/MM/yyyy"),
                    s.Gender,
                    Faculty = s.Faculty.Name,
                    Program = s.Program.Name,
                    s.Email,
                    s.PhoneNumber,
                    Status = s.Status.Name
                })
                .ToListAsync();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            return JsonSerializer.SerializeToUtf8Bytes(new { students }, options);
        }
    }

    // Class để deserialize JSON
    public class StudentJsonImport
    {
        public List<StudentImportDto> Students { get; set; }
    }
}