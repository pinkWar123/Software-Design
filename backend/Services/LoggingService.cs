using System;
using System.Threading.Tasks;
using Serilog;

namespace backend.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly Serilog.ILogger _logger;

        public LoggingService()
        {
            // Create a logger for this class using Serilog.
            _logger = Log.ForContext<LoggingService>();
        }

        public Task LogAsync(string action, string details, string userId = "system")
        {
            // Log the message using the same format as before:
            // "yyyy-MM-dd HH:mm:ss|{Action}|{UserId}|{Details}"
            _logger.Information("{Timestamp:yyyy-MM-dd HH:mm:ss}|{Action}|{UserId}|{Details}", 
                DateTime.Now, action, userId, details);
            return Task.CompletedTask;
        }
    }
}
