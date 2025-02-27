using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Student> Students { get; set; }
        DbSet<Faculty> Faculties { get; set; }
        DbSet<StudyProgram> StudyPrograms { get; set; }
        DbSet<Status> Statuses { get; set; }
        DbSet<StatusTransition> StatusTransitions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}