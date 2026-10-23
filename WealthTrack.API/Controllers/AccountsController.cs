using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthTrack.API.Services;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyAccounts()
        {
            var clientId = await GetClientIdForCurrentUserAsync();

            if (clientId == 0)
            {
                return NotFound(new { message = "Client profile not found." });
            }

            var accounts = await _accountService.GetAccountsForClientAsync(clientId);
            return Ok(accounts);
        }

        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> GetAccountsForClient(int clientId)
        {
            var isAssigned = await _accountService.AdvisorOwnsClientAsync(
                GetCurrentUserId(), clientId);

            if (!isAssigned)
            {
                return Forbid();
            }

            var accounts = await _accountService.GetAccountsForClientAsync(clientId);
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);

            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            var role = GetCurrentUserRole();

            if (role == "Customer")
            {
                var clientId = await GetClientIdForCurrentUserAsync();

                if (account.ClientId != clientId)
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

            return Ok(account);
        }

        private async Task<int> GetClientIdForCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var client = await _accountService.GetClientByUserIdAsync(userId);
            return client?.Id ?? 0;
        }
    }
}
