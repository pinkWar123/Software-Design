using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Entities;

namespace backend.Services
{
    public class StatusService : IStatusService
    {
        private readonly IApplicationDbContext _context;
        public StatusService(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> DeleteStatus(int id)
        {
            var status = await GetStatusById(id);
            if (status == null)
            {
                return false;
            }

            if(_context.Students.Any(s => s.StatusId == id)) return false;
            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Status?> GetStatusById(int id)
        {
            return await _context.Statuses.FindAsync(id);
        }
    }
}