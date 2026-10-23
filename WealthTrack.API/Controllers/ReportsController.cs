using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthTrack.API.Services;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : BaseController
    {
        private readonly ReportService  _reportService;
        private readonly AccountService _accountService;

        public ReportsController(ReportService reportService, AccountService accountService)
        {
            _reportService  = reportService;
            _accountService = accountService;
        }

        [HttpGet("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerReport()
        {
            var client = await _accountService.GetClientByUserIdAsync(GetCurrentUserId());

            if (client == null)
            {
                return NotFound(new { message = "Client profile not found." });
            }

            var report = await _reportService.GetCustomerReportAsync(client.Id);

            if (report == null)
            {
                return NotFound(new { message = "No account data found." });
            }

            return Ok(report);
        }

        [HttpGet("advisor")]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> GetAdvisorReport()
        {
            var report = await _reportService.GetAdvisorReportAsync(GetCurrentUserId());
            return Ok(report);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminReport()
        {
            var report = await _reportService.GetAdminReportAsync();
            return Ok(report);
        }
    }
}
