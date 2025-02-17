using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var buildTime = System.IO.File.GetLastWriteTime(assembly.Location);

            var versionInfo = new
            {
                Version = $"v{version.Major}.{version.Minor}.{version.Build}",
                BuildTime = buildTime.ToString("dd/MM/yyyy HH:mm:ss"),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            return Ok(versionInfo);
        }
    }
} 