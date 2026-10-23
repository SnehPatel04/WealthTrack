using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;
using WealthTrack.API.Models;

namespace WealthTrack.API.Services
{
    public class AccountService
    {
        private readonly WealthTrackDbContext _context;

        public AccountService(WealthTrackDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountDto>> GetAccountsForClientAsync(int clientId)
        {
            return await _context.InvestmentAccounts
                .Where(a => a.ClientId == clientId)
                .Select(a => new AccountDto
                {
                    Id            = a.Id,
                    ClientId      = a.ClientId,
                    AccountNumber = a.AccountNumber,
                    AccountType   = a.AccountType,
                    Balance       = a.Balance,
                    AvailableCash = a.AvailableCash,
                    Status        = a.Status,
                    OpenedAt      = a.OpenedAt,
                })
                .ToListAsync();
        }

        public async Task<AccountDto?> GetAccountByIdAsync(int accountId)
        {
            return await _context.InvestmentAccounts
                .Where(a => a.Id == accountId)
                .Select(a => new AccountDto
                {
                    Id            = a.Id,
                    ClientId      = a.ClientId,
                    AccountNumber = a.AccountNumber,
                    AccountType   = a.AccountType,
                    Balance       = a.Balance,
                    AvailableCash = a.AvailableCash,
                    Status        = a.Status,
                    OpenedAt      = a.OpenedAt,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AdvisorOwnsClientAsync(int advisorUserId, int clientId)
        {
            return await _context.AdvisorClients
                .AnyAsync(ac =>
                    ac.Advisor.UserId == advisorUserId &&
                    ac.ClientId == clientId);
        }

        public async Task<Models.Client?> GetClientByUserIdAsync(int userId)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Models.Advisor?> GetAdvisorByUserIdAsync(int userId)
        {
            return await _context.Advisors
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}
