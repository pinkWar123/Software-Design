using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Configuration;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IApplicationDbContext _context;
        public ConfigurationRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ActivateConfiguration(string key)
        {
            var hasConfigurationExisted = await GetConfigurationByKeyAsync(key);
            if(hasConfigurationExisted == null) return false;
            hasConfigurationExisted.IsActive = true;
            return true;
        }

        public async Task CreateConfiguration(CreateConfigurationDto configuration)
        {
            var hasKeyExisted = await GetConfigurationByKeyAsync(configuration.Key);
            if(hasKeyExisted != null) throw new Exception("This key has existed");
            var newConfiguration = new Configuration
            {
                Key = configuration.Key,
                Value = configuration.Value,
                IsActive = true
            };
            await _context.Configurations.AddAsync(newConfiguration);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeactivateConfiguration(string key)
        {
            var hasConfigurationExisted = await GetConfigurationByKeyAsync(key);
            if(hasConfigurationExisted == null) return false;
            hasConfigurationExisted.IsActive = false;
            return true;
        }

        public async Task<List<Configuration>> GetAllConfigurationsAsync()
        {
            return await _context.Configurations.ToListAsync();
        }

        public async Task<Configuration?> GetConfigurationByKeyAsync(string key)
        {
            return await _context.Configurations.FirstOrDefaultAsync(c => c.Key == key);
        }

        public async Task<bool> UpdateConfiguration(Configuration configuration)
        {
            var hasConfigurationExisted = await GetConfigurationByKeyAsync(configuration.Key);
            if(hasConfigurationExisted == null) return false;
            hasConfigurationExisted.Value = configuration.Value;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}