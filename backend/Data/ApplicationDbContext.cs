using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary key
            modelBuilder.Entity<StatusTransition>()
                .HasKey(st => new { st.SourceStatusId, st.TargetStatusId });

            // Configure relationship for SourceStatus
            modelBuilder.Entity<StatusTransition>()
                .HasOne(st => st.SourceStatus)
                .WithMany(s => s.OutgoingTransitions)
                .HasForeignKey(st => st.SourceStatusId)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete if desired

            // Configure relationship for TargetStatus.
            // If you have an IncomingTransitions navigation on Status, you could use it here.
                // Otherwise, leave the navigation out.
                modelBuilder.Entity<StatusTransition>()
                    .HasOne(st => st.TargetStatus)
                    .WithMany() // or .WithMany(s => s.IncomingTransitions) if you add that navigation property
                    .HasForeignKey(st => st.TargetStatusId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Student>()
                    .HasMany(st => st.SubscribeToNotifications)
                    .WithOne(sn => sn.Student)
                    .IsRequired(false);

                modelBuilder.Entity<StudentNotification>()
                .HasOne(sn => sn.Student)
                .WithMany(s => s.SubscribeToNotifications)
                .HasForeignKey(sn => sn.StudentId)
                .OnDelete(DeleteBehavior.Cascade);


                modelBuilder.Entity<Configuration>(c => 
                {
                    c.HasData(
                        new Configuration { Id = 1, Key = "AllowedEmailDomain", Value = "student.university.edu.vn", IsActive = true },
                        new Configuration { Id = 2, Key = "AllowedPhonePhoneNumberPattern", Value = "^0[35789]\\d{8}$", IsActive = true },
                        new Configuration { Id = 3, Key = "AllowedStudentStatusTransition", Value = "", IsActive = true }
                    );
                });
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<StudyProgram> StudyPrograms { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<StatusTransition> StatusTransitions { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
    }
}