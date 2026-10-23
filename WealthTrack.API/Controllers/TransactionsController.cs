using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthTrack.API.Services;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : BaseController
    {
        private readonly TransactionService _transactionService;
        private readonly AccountService     _accountService;

        public TransactionsController(TransactionService transactionService, AccountService accountService)
        {
            _transactionService = transactionService;
            _accountService     = accountService;
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetTransactionsForAccount(int accountId)
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

            var transactions = await _transactionService.GetTransactionsForAccountAsync(accountId);
            return Ok(transactions);
        }
    }
}
