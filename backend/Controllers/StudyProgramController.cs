using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.StudyProgram;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudyProgramController : ControllerBase
    {
        private readonly IStudyProgramRepository _studyProgramRepository;
        private readonly ILoggingService _loggingService;

        public StudyProgramController(IStudyProgramRepository studyProgramRepository, ILoggingService loggingService)
        {
            _studyProgramRepository = studyProgramRepository;
            _loggingService = loggingService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStudyPrograms()
        {
            try
            {
                var studyPrograms = await _studyProgramRepository.GetAllStudyPrograms();
                await _loggingService.LogAsync("GetStudyPrograms", "Retrieved all study programs");
                return Ok(studyPrograms);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to get study programs: {ex.Message}");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyProgram(int id, [FromBody] UpdateStudyProgramDto studyProgram)
        {
            try
            {
                var existingStudyProgram = await _studyProgramRepository.GetStudyProgramById(id);
                if (existingStudyProgram == null)
                {
                    await _loggingService.LogAsync("Error", $"Study program not found: {id}");
                    return NotFound();
                }
                var updatedStudyProgram = await _studyProgramRepository.UpdateStudyProgram(id, studyProgram);
                await _loggingService.LogAsync("UpdateStudyProgram", $"Updated study program: {id}");
                return Ok(updatedStudyProgram);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to update study program {id}: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyProgram([FromBody] CreateStudyProgramDto studyProgram)
        {
            try
            {
                var newStudyProgram = await _studyProgramRepository.CreateStudyProgram(studyProgram);
                await _loggingService.LogAsync("CreateStudyProgram", $"Created new study program: {newStudyProgram.Name}");
                return Ok(newStudyProgram);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to create study program: {ex.Message}");
                throw;
            }
        }
    }
}