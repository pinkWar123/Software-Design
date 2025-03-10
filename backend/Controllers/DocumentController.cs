using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Services;
using backend.Services.DocumentGeneration;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IReportService _reportService;
        public DocumentController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet]
        public async Task<IActionResult> GenerateDocumentForStudent(
            [FromQuery] int studentId, 
            [FromQuery] DocumentType format, 
            [FromQuery] string reason,
            [FromQuery] string expiredDate)
        {
            var reportFile = await _reportService.GenerateReportForStudent(studentId, format, reason, expiredDate);
            return File(reportFile.Content, reportFile.ContentType, reportFile.FileName);        }
    }
}