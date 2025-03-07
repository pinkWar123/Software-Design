using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Entities
{
    public enum NotificationType
    {
        Email = 1,
        SMS = 2,
        Zalo = 3
    }
    public class StudentNotification
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public required Student Student { get; set; }
        public NotificationType Type { get; set; }
    }
}