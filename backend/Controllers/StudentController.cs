using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Student;
using backend.Entities;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using backend.Services;
namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentService _studentService;
        private readonly ILoggingService _loggingService;

        public StudentController(ApplicationDbContext context, IStudentRepository studentRepository, IStudentService studentService, ILoggingService loggingService)
        {
            _context = context;
            _studentRepository = studentRepository;
            _studentService = studentService;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentRepository.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto student)
        {
        try 
        {
            var newStudent = await _studentRepository.CreateStudentAsync(student);
            await _loggingService.LogAsync(
                "CreateStudent",
                $"Created student: {newStudent.StudentId} - {newStudent.FullName}"
            );
            return CreatedAtAction(nameof(GetStudents), new { id = newStudent.StudentId }, newStudent);
        }
        catch (Exception ex)
        {
            await _loggingService.LogAsync(
                "Error",
                $"Failed to create student: {ex.Message}"
            );
            throw;
        }
    }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _studentRepository.DeleteStudentAsync(id);
                await _loggingService.LogAsync(
                    "DeleteStudent",
                    $"Deleted student ID: {id}"
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error",
                    $"Failed to delete student {id}: {ex.Message}"
                );
                throw;
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentDto student)
        {
            try
            {
                var isUpdated = await _studentRepository.UpdateStudentAsync(student);
                if (!isUpdated)
                {
                    return NotFound();
                }
                await _loggingService.LogAsync(
                    "UpdateStudent",
                    $"Updated student ID: {student.StudentId}"
                );
                await _loggingService.LogAsync(
                    "UpdateStudent",
                    $"Updated student ID: {student.StudentId}"
                );

                return NoContent();
            }
            catch (System.Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error",
                    $"Failed to update student: {ex.Message}"
                );

                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            await _loggingService.LogAsync(
                "GetStudentById",
                $"Get student ID: {id}"
            );
            return Ok(student);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không tìm thấy file");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".csv" && extension != ".json")
                return BadRequest("Chỉ hỗ trợ file CSV và JSON");

            try
            {
                using var stream = file.OpenReadStream();
                if (extension == ".csv")
                {
                    await _studentService.ImportFromCsv(stream);
                }
                else if (extension == ".json")
                {
                    Console.WriteLine("This is json file");
                    await _studentService.ImportFromJson(stream);
                }
                await _loggingService.LogAsync(
                "ImportStudents",
                $"Imported students from {file.FileName}"
            );
                return Ok(new { message = "Import thành công" });
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error",
                    $"Failed to import students: {ex.Message}"
                );
                return BadRequest(new { message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportStudents([FromQuery] string format = "csv")
        {
            try
            {
                if (format.ToLower() == "json")
                {
                    var jsonData = await _studentService.ExportToJson();
                    await _loggingService.LogAsync(
                        "ExportStudents",
                        $"Exported students to JSON"
                    );
                    return File(jsonData, "application/json", "students.json");
                }
                else
                {
                    var csvData = await _studentService.ExportToCsv();
                    await _loggingService.LogAsync(
                        "ExportStudents",
                        $"Exported students to CSV"
                    );
                    return File(csvData, "text/csv", "students.csv");
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                "Error",
                $"Failed to import students: {ex.Message}"
            );
                return BadRequest($"Export failed: {ex.Message}");
            }
        }
    }
}