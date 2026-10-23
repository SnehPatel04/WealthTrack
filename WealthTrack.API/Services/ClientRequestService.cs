using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;
using WealthTrack.API.Models;

namespace WealthTrack.API.Services
{
    public class ClientRequestService
    {
        private readonly WealthTrackDbContext _context;

        public ClientRequestService(WealthTrackDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientRequestDto>> GetRequestsForClientAsync(int clientId)
        {
            return await _context.ClientRequests
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.SubmittedAt)
                .Select(r => new ClientRequestDto
                {
                    Id              = r.Id,
                    ClientId        = r.ClientId,
                    ClientName      = r.Client.User.FirstName + " " + r.Client.User.LastName,
                    AdvisorId       = r.AdvisorId,
                    RequestType     = r.RequestType,
                    Subject         = r.Subject,
                    Message         = r.Message,
                    Status          = r.Status,
                    AdvisorResponse = r.AdvisorResponse,
                    SubmittedAt     = r.SubmittedAt,
                    ResolvedAt      = r.ResolvedAt,
                })
                .ToListAsync();
        }

        public async Task<List<ClientRequestDto>> GetRequestsForAdvisorAsync(int advisorId)
        {
            return await _context.ClientRequests
                .Where(r => r.AdvisorId == advisorId)
                .OrderByDescending(r => r.SubmittedAt)
                .Select(r => new ClientRequestDto
                {
                    Id              = r.Id,
                    ClientId        = r.ClientId,
                    ClientName      = r.Client.User.FirstName + " " + r.Client.User.LastName,
                    AdvisorId       = r.AdvisorId,
                    RequestType     = r.RequestType,
                    Subject         = r.Subject,
                    Message         = r.Message,
                    Status          = r.Status,
                    AdvisorResponse = r.AdvisorResponse,
                    SubmittedAt     = r.SubmittedAt,
                    ResolvedAt      = r.ResolvedAt,
                })
                .ToListAsync();
        }

        public async Task<ClientRequestDto?> SubmitRequestAsync(int clientId, CreateClientRequestDto dto)
        {
            var assignment = await _context.AdvisorClients
                .Include(ac => ac.Advisor)
                .FirstOrDefaultAsync(ac => ac.ClientId == clientId);

            if (assignment == null)
            {
                return null;
            }

            var request = new ClientRequest
            {
                ClientId        = clientId,
                AdvisorId       = assignment.AdvisorId,
                RequestType     = dto.RequestType,
                Subject         = dto.Subject,
                Message         = dto.Message,
                Status          = "Pending",
                AdvisorResponse = string.Empty,
                SubmittedAt     = DateTime.UtcNow,
            };

            _context.ClientRequests.Add(request);
            await _context.SaveChangesAsync();

            var client = await _context.Clients
                .Include(c => c.User)
                .FirstAsync(c => c.Id == clientId);

            return new ClientRequestDto
            {
                Id              = request.Id,
                ClientId        = request.ClientId,
                ClientName      = $"{client.User.FirstName} {client.User.LastName}",
                AdvisorId       = request.AdvisorId,
                RequestType     = request.RequestType,
                Subject         = request.Subject,
                Message         = request.Message,
                Status          = request.Status,
                AdvisorResponse = request.AdvisorResponse,
                SubmittedAt     = request.SubmittedAt,
            };
        }

        public async Task<bool> RespondToRequestAsync(int requestId, int advisorId, RespondToRequestDto dto)
        {
            var request = await _context.ClientRequests
                .FirstOrDefaultAsync(r => r.Id == requestId && r.AdvisorId == advisorId);

            if (request == null)
            {
                return false;
            }

            request.AdvisorResponse = dto.AdvisorResponse;
            request.Status          = dto.Status;

            if (dto.Status == "Resolved")
            {
                request.ResolvedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
