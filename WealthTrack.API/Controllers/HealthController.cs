using Microsoft.AspNetCore.Mvc;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            var response = new
            {
                status  = "Healthy",
                app     = "WealthTrack API",
                version = "1.0",
                time    = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}

