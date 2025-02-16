using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudyProgramController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudyProgramController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStudyPrograms()
        {
            var studyPrograms = await _context.StudyPrograms.ToListAsync();
            return Ok(studyPrograms);
        }
    }
}