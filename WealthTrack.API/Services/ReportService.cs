using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;

namespace WealthTrack.API.Services
{
    public class ReportService
    {
        private readonly WealthTrackDbContext _context;

        public ReportService(WealthTrackDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerReportDto?> GetCustomerReportAsync(int clientId)
        {
            var accounts = await _context.InvestmentAccounts
                .Where(a => a.ClientId == clientId)
                .ToListAsync();

            if (!accounts.Any())
            {
                return null;
            }

            var accountIds = accounts.Select(a => a.Id).ToList();

            var holdings = await _context.Holdings
                .Include(h => h.Security)
                .Where(h => accountIds.Contains(h.InvestmentAccountId))
                .ToListAsync();

            var totalValue    = holdings.Sum(h => h.Quantity * h.Security.CurrentPrice);
            var totalCost     = holdings.Sum(h => h.Quantity * h.AverageCostBasis);
            var totalGainLoss = totalValue - totalCost;
            var gainPercent   = totalCost == 0 ? 0 : Math.Round((totalGainLoss / totalCost) * 100, 2);

            var accountSummaries = accounts.Select(a => new AccountSummaryDto
            {
                AccountId     = a.Id,
                AccountNumber = a.AccountNumber,
                AccountType   = a.AccountType,
                Balance       = a.Balance,
                AvailableCash = a.AvailableCash,
            }).ToList();

            return new CustomerReportDto
            {
                TotalPortfolioValue  = Math.Round(totalValue, 2),
                TotalGainLoss        = Math.Round(totalGainLoss, 2),
                TotalGainLossPercent = gainPercent,
                TotalAccounts        = accounts.Count,
                Accounts             = accountSummaries,
            };
        }

        public async Task<AdvisorReportDto> GetAdvisorReportAsync(int advisorUserId)
        {
            var assignments = await _context.AdvisorClients
                .Include(ac => ac.Client)
                    .ThenInclude(c => c.User)
                .Include(ac => ac.Client)
                    .ThenInclude(c => c.InvestmentAccounts)
                .Where(ac => ac.Advisor.UserId == advisorUserId)
                .ToListAsync();

            var advisorId = assignments.FirstOrDefault()?.AdvisorId ?? 0;

            var pendingRequests = await _context.ClientRequests
                .CountAsync(r => r.AdvisorId == advisorId && r.Status == "Pending");

            var clientSummaries = assignments.Select(ac =>
            {
                var totalValue = ac.Client.InvestmentAccounts.Sum(a => a.Balance);

                return new ClientSummaryDto
                {
                    ClientId           = ac.ClientId,
                    ClientName         = $"{ac.Client.User.FirstName} {ac.Client.User.LastName}",
                    TotalPortfolioValue = totalValue,
                    AccountCount       = ac.Client.InvestmentAccounts.Count,
                };
            }).ToList();

            return new AdvisorReportDto
            {
                TotalManagedAssets = clientSummaries.Sum(c => c.TotalPortfolioValue),
                TotalClients       = clientSummaries.Count,
                PendingRequests    = pendingRequests,
                Clients            = clientSummaries,
            };
        }

        public async Task<AdminReportDto> GetAdminReportAsync()
        {
            var totalUsers     = await _context.Users.CountAsync();
            var totalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer");
            var totalAdvisors  = await _context.Users.CountAsync(u => u.Role == "Advisor");
            var activeUsers    = await _context.Users.CountAsync(u => u.IsActive);
            var totalAccounts  = await _context.InvestmentAccounts.CountAsync();

            return new AdminReportDto
            {
                TotalUsers     = totalUsers,
                TotalCustomers = totalCustomers,
                TotalAdvisors  = totalAdvisors,
                ActiveUsers    = activeUsers,
                InactiveUsers  = totalUsers - activeUsers,
                TotalAccounts  = totalAccounts,
            };
        }
    }
}
