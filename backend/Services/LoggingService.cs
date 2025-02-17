using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace backend.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly string _logPath = "Logs/app.log";

        public LoggingService()
        {
            var logDirectory = Path.GetDirectoryName(_logPath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public async Task LogAsync(string action, string details, string userId = "system")
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Action = action,
                Details = details,
                UserId = userId,
                IpAddress = "" // Có thể thêm IP address nếu cần
            };

            var logLine = $"{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss}|{logEntry.Action}|{logEntry.UserId}|{logEntry.Details}\n";
            await File.AppendAllTextAsync(_logPath, logLine);
        }
    }
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
    }
}