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

        public StudentController(ApplicationDbContext context, IStudentRepository studentRepository, IStudentService studentService)
        {
            _context = context;
            _studentRepository = studentRepository;
            _studentService = studentService;
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
            if (student == null)
            {
                return BadRequest("Student data is required");
            }

            var newStudent = await _studentRepository.CreateStudentAsync(student);

            return CreatedAtAction(nameof(GetStudents), new { id = newStudent.StudentId }, newStudent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var isDeleted = await _studentRepository.DeleteStudentAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentDto student)
        {

            var isUpdated = await _studentRepository.UpdateStudentAsync(student);
            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

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
                return Ok(new { message = "Import thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}