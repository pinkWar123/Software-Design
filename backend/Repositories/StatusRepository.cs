using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Status;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public StatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Status> CreateStatus(CreateStatusDto status)
        {
            var statusEntity = new Status
            {
                Name = status.Name,
                OutgoingTransitions = status.OutgoingTransitions.Select(id => new StatusTransition { SourceStatusId = id }).ToList()
            };
            _context.Statuses.Add(statusEntity);
            await _context.SaveChangesAsync();
            return statusEntity;
        }

        public async Task<IEnumerable<Status>> GetAllStatuses()
        {
            return await _context
            .Statuses
            .Include(s => s.OutgoingTransitions)
                .ThenInclude(st => st.TargetStatus)
            .ToListAsync();
        }

        public async Task<Status> GetStatusById(int id)
        {
            return await _context
            .Statuses
            .Include(s => s.OutgoingTransitions)
                .ThenInclude(st => st.TargetStatus)
            .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Status> UpdateStatus(int id, UpdateStatusDto status)
        {
            var statusEntity = await _context
                .Statuses
                .Include(s => s.OutgoingTransitions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (statusEntity == null)
            {
                throw new Exception("Status not found");
            }

            statusEntity.Name = status.Name;

            await _context.SaveChangesAsync();
            return statusEntity;
        }
    }
}