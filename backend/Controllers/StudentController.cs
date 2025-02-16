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

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepository _studentRepository;

        public StudentController(ApplicationDbContext context, IStudentRepository studentRepository)
        {
            _context = context;
            _studentRepository = studentRepository;
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
    }
}