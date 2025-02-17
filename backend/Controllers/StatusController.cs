using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Status;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusRepository _statusRepository;
        private readonly ILoggingService _loggingService;

        public StatusController(IStatusRepository statusRepository, ILoggingService loggingService)
        {
            _statusRepository = statusRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _statusRepository.GetAllStatuses();
                await _loggingService.LogAsync("GetStatuses", "Retrieved all statuses");
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to get statuses: {ex.Message}");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto status)
        {
            try
            {
                var existingStatus = await _statusRepository.GetStatusById(id);
                if (existingStatus == null)
                {
                    await _loggingService.LogAsync("Error", $"Status not found: {id}");
                    return NotFound();
                }
                var updatedStatus = await _statusRepository.UpdateStatus(id, status);
                await _loggingService.LogAsync("UpdateStatus", $"Updated status: {id}");
                return Ok(updatedStatus);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to update status {id}: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStatus([FromBody] CreateStatusDto status)
        {
            try
            {
                var newStatus = await _statusRepository.CreateStatus(status);
                await _loggingService.LogAsync("CreateStatus", $"Created new status: {newStatus.Name}");
                return Ok(newStatus);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error", $"Failed to create status: {ex.Message}");
                throw;
            }
        }
    }
}