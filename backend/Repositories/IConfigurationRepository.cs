using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;

namespace backend.Repositories
{
    public interface IConfigurationRepository
    {
        Task<Configuration?> GetConfigurationByKeyAsync(string key);
        Task<bool> UpdateConfiguration(Configuration configuration);
        Task<bool> ActivateConfiguration(string key);
        Task<bool> DeactivateConfiguration(string key);
        Task<List<Configuration>> GetAllConfigurationsAsync();
    }
}