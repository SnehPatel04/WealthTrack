using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthTrack.API.DTOs;
using WealthTrack.API.Services;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ClientRequestsController : BaseController
    {
        private readonly ClientRequestService _requestService;
        private readonly AccountService       _accountService;

        public ClientRequestsController(ClientRequestService requestService, AccountService accountService)
        {
            _requestService = requestService;
            _accountService = accountService;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyRequests()
        {
            var client = await _accountService.GetClientByUserIdAsync(GetCurrentUserId());

            if (client == null)
            {
                return NotFound(new { message = "Client profile not found." });
            }

            var requests = await _requestService.GetRequestsForClientAsync(client.Id);
            return Ok(requests);
        }

        [HttpGet("inbox")]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> GetAdvisorInbox()
        {
            var advisor = await GetAdvisorIdForCurrentUserAsync();

            if (advisor == 0)
            {
                return NotFound(new { message = "Advisor profile not found." });
            }

            var requests = await _requestService.GetRequestsForAdvisorAsync(advisor);
            return Ok(requests);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmitRequest([FromBody] CreateClientRequestDto dto)
        {
            var client = await _accountService.GetClientByUserIdAsync(GetCurrentUserId());

            if (client == null)
            {
                return NotFound(new { message = "Client profile not found." });
            }

            var result = await _requestService.SubmitRequestAsync(client.Id, dto);

            if (result == null)
            {
                return BadRequest(new { message = "No advisor assigned to this client." });
            }

            return Ok(result);
        }

        [HttpPut("{id}/respond")]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> RespondToRequest(int id, [FromBody] RespondToRequestDto dto)
        {
            var advisorId = await GetAdvisorIdForCurrentUserAsync();

            var success = await _requestService.RespondToRequestAsync(id, advisorId, dto);

            if (!success)
            {
                return NotFound(new { message = "Request not found or not assigned to you." });
            }

            return NoContent();
        }

        private async Task<int> GetAdvisorIdForCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var advisor = await _accountService.GetAdvisorByUserIdAsync(userId);
            return advisor?.Id ?? 0;
        }
    }
}
