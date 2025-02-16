using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Faculty;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyRepository _facultyRepository;

        public FacultyController(IFacultyRepository facultyRepository)
        {
            _facultyRepository = facultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaculties()
        {
            Console.WriteLine("Run here");
            var faculties = await _facultyRepository.GetFaculties();
            return Ok(faculties);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaculty(int id, [FromBody] UpdateFacultyDto faculty)
        {
            var existingFaculty = await _facultyRepository.GetFacultyById(id);
            if (existingFaculty == null)
            {
                return NotFound();
            }
            var updatedFaculty = await _facultyRepository.UpdateFaculty(id, faculty);
            return Ok(updatedFaculty);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto faculty)
        {
            var newFaculty = await _facultyRepository.CreateFaculty(faculty);
            return Ok(newFaculty);
        }
    }
}