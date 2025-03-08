using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;

namespace backend.Services
{
    public interface IStatusService
    {
        Task<bool> DeleteStatus(int id);
        Task<Status?> GetStatusById(int id);
    }
}