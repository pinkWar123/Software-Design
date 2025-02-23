using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Entities;

namespace backend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed Status
            if (!context.Statuses.Any())
            {
                var statuses = new Status[]
                {
                    new Status { Name = "Pending" },
                    new Status { Name = "Approved" },
                    new Status { Name = "Rejected" }
                };
                context.Statuses.AddRange(statuses);
                context.SaveChanges();
            }

            // Seed StudyPrograms
            if (!context.StudyPrograms.Any())
            {
                var programs = new StudyProgram[]
                {
                    new StudyProgram { Name = "Computer Science" },
                    new StudyProgram { Name = "Information Technology" },
                    new StudyProgram { Name = "Software Engineering" },
                    new StudyProgram { Name = "Data Science" },
                    new StudyProgram { Name = "Artificial Intelligence" }
                };
                context.StudyPrograms.AddRange(programs);
                context.SaveChanges();
            }

            if (!context.Faculties.Any())
        {
            var faculties = new List<Faculty>
            {
                new Faculty 
                { 
                    Name = "Công nghệ thông tin",
                },
                new Faculty 
                { 
                    Name = "Kinh tế",
                },
                new Faculty 
                { 
                    Name = "Kỹ thuật điện",
                },
                new Faculty 
                { 
                    Name = "Cơ khí",
                },
                new Faculty 
                { 
                    Name = "Xây dựng",
                }
            };

            context.Faculties.AddRange(faculties);
            context.SaveChanges();
        }

            // Seed Users
            if (!context.Students.Any())
            {
                
            }
        }
    }
}