using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Configuration;
using backend.Entities;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IApplicationDbContext _context;
        public ConfigurationController(IConfigurationRepository configurationRepository, IApplicationDbContext context)
        {
            _configurationRepository = configurationRepository;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetConfigurations()
        {
            var configurations = await _configurationRepository.GetAllConfigurationsAsync();
            return Ok(configurations);
        }

        [HttpPut("{key}/activate")]
        public async Task<IActionResult> ActivateConfiguration(string key)
        {
            var isActivated = await _configurationRepository.ActivateConfiguration(key);
            await _context.SaveChangesAsync();
            if (!isActivated) return NotFound();
            return Ok();
        }

        [HttpPut("{key}/deactivate")]
        public async Task<IActionResult> DeactivateConfiguration(string key)
        {
            var isDeactivated = await _configurationRepository.DeactivateConfiguration(key);
            await _context.SaveChangesAsync();
            if (!isDeactivated) return NotFound();
            return Ok();
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateConfigurationValue([FromRoute] string key, [FromBody] string newValue)
        {
            Console.WriteLine("New value: " + newValue);
            var configuration = await _configurationRepository.GetConfigurationByKeyAsync(key);
            if(configuration == null) return NotFound();
            configuration.Value = newValue;
            var isUpdated = await _configurationRepository.UpdateConfiguration(configuration);
            if (!isUpdated) return Conflict();
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateConfiguration([FromBody] CreateConfigurationDto createConfigurationDto)
        {
            try
            {
                await _configurationRepository.CreateConfiguration(createConfigurationDto);
                return Created();
            }
            catch (System.Exception)
            {
                throw;
            }
        }   
    }
}