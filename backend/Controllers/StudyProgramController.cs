using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.StudyProgram;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudyProgramController : ControllerBase
    {
        private readonly IStudyProgramRepository _studyProgramRepository;

        public StudyProgramController(IStudyProgramRepository studyProgramRepository)
        {
            _studyProgramRepository = studyProgramRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStudyPrograms()
        {
            var studyPrograms = await _studyProgramRepository.GetAllStudyPrograms();
            return Ok(studyPrograms);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyProgram(int id, [FromBody] UpdateStudyProgramDto studyProgram)
        {
            var existingStudyProgram = await _studyProgramRepository.GetStudyProgramById(id);
            if (existingStudyProgram == null)
            {
                return NotFound();
            }
            var updatedStudyProgram = await _studyProgramRepository.UpdateStudyProgram(id, studyProgram);
            return Ok(updatedStudyProgram);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyProgram([FromBody] CreateStudyProgramDto studyProgram)
        {
            var newStudyProgram = await _studyProgramRepository.CreateStudyProgram(studyProgram);
            return Ok(newStudyProgram);
        }
    }
}