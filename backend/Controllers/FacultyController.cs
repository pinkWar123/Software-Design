using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Faculty;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly ILoggingService _loggingService;

        public FacultyController(IFacultyRepository facultyRepository, ILoggingService loggingService)
        {
            _facultyRepository = facultyRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaculties()
        {
            try
            {
                var faculties = await _facultyRepository.GetFaculties();
                await _loggingService.LogAsync("GetFaculties", "Retrieved all faculties");
                return Ok(faculties);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to get faculties: {ex.Message}");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaculty(int id, [FromBody] UpdateFacultyDto faculty)
        {
            try
            {
                var existingFaculty = await _facultyRepository.GetFacultyById(id);
                if (existingFaculty == null)
                {
                    await _loggingService.LogAsync("Error", $"Faculty not found: {id}");
                    return NotFound();
                }
                var updatedFaculty = await _facultyRepository.UpdateFaculty(id, faculty);
                await _loggingService.LogAsync("UpdateFaculty", $"Updated faculty: {id}");
                return Ok(updatedFaculty);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to update faculty {id}: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto faculty)
        {
            try
            {
                var newFaculty = await _facultyRepository.CreateFaculty(faculty);
                await _loggingService.LogAsync("CreateFaculty", $"Created new faculty: {newFaculty.Name}");
                return Ok(newFaculty);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to create faculty: {ex.Message}");
                throw;
            }
        }
    }
}