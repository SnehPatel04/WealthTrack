using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthTrack.API.Services;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PortfoliosController : BaseController
    {
        private readonly PortfolioService _portfolioService;
        private readonly AccountService   _accountService;

        public PortfoliosController(PortfolioService portfolioService, AccountService accountService)
        {
            _portfolioService = portfolioService;
            _accountService   = accountService;
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetPortfolioByAccount(int accountId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            var role = GetCurrentUserRole();

            if (role == "Customer")
            {
                var client = await _accountService.GetClientByUserIdAsync(GetCurrentUserId());

                if (client == null || account.ClientId != client.Id)
                {
                    return Forbid();
                }
            }

            if (role == "Advisor")
            {
                var isAssigned = await _accountService.AdvisorOwnsClientAsync(
                    GetCurrentUserId(), account.ClientId);

                if (!isAssigned)
                {
                    return Forbid();
                }
            }

            var summary = await _portfolioService.GetPortfolioSummaryAsync(accountId);
            return Ok(summary);
        }

        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> GetPortfoliosForClient(int clientId)
        {
            var isAssigned = await _accountService.AdvisorOwnsClientAsync(
                GetCurrentUserId(), clientId);

            if (!isAssigned)
            {
                return Forbid();
            }

            var summaries = await _portfolioService.GetPortfoliosForClientAsync(clientId);
            return Ok(summaries);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPortfolios()
        {
            var client = await _accountService.GetClientByUserIdAsync(GetCurrentUserId());

            if (client == null)
            {
                return NotFound(new { message = "Client profile not found." });
            }

            var summaries = await _portfolioService.GetPortfoliosForClientAsync(client.Id);
            return Ok(summaries);
        }
    }
}
