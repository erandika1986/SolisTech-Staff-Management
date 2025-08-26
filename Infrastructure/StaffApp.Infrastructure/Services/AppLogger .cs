using Microsoft.Extensions.Logging;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class AppLogger : IAppLogger
    {
        private readonly ILogger<AppLogger> _logger;
        public AppLogger(ILogger<AppLogger> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message = "")
        {
            _logger.LogError(ex, message ?? ex.Message);
        }
    }
}
