using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Status;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusRepository _statusRepository;

        public StatusController(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatuses()
        {
            Console.WriteLine("Run here");
            var statuses = await _statusRepository.GetAllStatuses();
            return Ok(statuses);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto status)
        {
            var existingStatus = await _statusRepository.GetStatusById(id);
            if (existingStatus == null)
            {
                return NotFound();
            }
            var updatedStatus = await _statusRepository.UpdateStatus(id, status);
            return Ok(updatedStatus);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStatus([FromBody] CreateStatusDto status)
        {
            var newStatus = await _statusRepository.CreateStatus(status);
            return Ok(newStatus);
        }
        
    }
}