using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;
using backend.Dtos.Status;
namespace backend.Repositories
{
    public interface IStatusRepository
    {
        Task<IEnumerable<Status>> GetAllStatuses();
        Task<Status> GetStatusById(int id);
        Task<Status> CreateStatus(CreateStatusDto status);
        Task<Status> UpdateStatus(int id, UpdateStatusDto status);
    }
}