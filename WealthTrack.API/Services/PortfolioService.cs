using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;

namespace WealthTrack.API.Services
{
    public class PortfolioService
    {
        private readonly WealthTrackDbContext _context;

        public PortfolioService(WealthTrackDbContext context)
        {
            _context = context;
        }

        public async Task<PortfolioSummaryDto?> GetPortfolioSummaryAsync(int accountId)
        {
            var account = await _context.InvestmentAccounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return null;
            }

            var holdings = await _context.Holdings
                .Where(h => h.InvestmentAccountId == accountId)
                .Include(h => h.Security)
                .ToListAsync();

            var holdingDtos = holdings.Select(h =>
            {
                var marketValue     = h.Quantity * h.Security.CurrentPrice;
                var costBasis       = h.Quantity * h.AverageCostBasis;
                var gainLossAmount  = marketValue - costBasis;
                var gainLossPercent = costBasis == 0
                    ? 0
                    : Math.Round((gainLossAmount / costBasis) * 100, 2);

                return new HoldingDto
                {
                    Id                  = h.Id,
                    InvestmentAccountId = h.InvestmentAccountId,
                    SecurityId          = h.SecurityId,
                    Ticker              = h.Security.Ticker,
                    SecurityName        = h.Security.Name,
                    AssetClass          = h.Security.AssetClass,
                    Quantity            = h.Quantity,
                    AverageCostBasis    = h.AverageCostBasis,
                    CurrentPrice        = h.Security.CurrentPrice,
                    MarketValue         = Math.Round(marketValue, 2),
                    GainLossAmount      = Math.Round(gainLossAmount, 2),
                    GainLossPercent     = gainLossPercent,
                };
            }).ToList();

            var totalMarketValue = holdingDtos.Sum(h => h.MarketValue);
            var totalCostBasis   = holdingDtos.Sum(h => h.Quantity * h.AverageCostBasis);
            var totalGainLoss    = totalMarketValue - totalCostBasis;
            var totalGainPercent = totalCostBasis == 0
                ? 0
                : Math.Round((totalGainLoss / totalCostBasis) * 100, 2);

            foreach (var holding in holdingDtos)
            {
                holding.AllocationPercent = totalMarketValue == 0
                    ? 0
                    : Math.Round((holding.MarketValue / totalMarketValue) * 100, 2);
            }

            return new PortfolioSummaryDto
            {
                AccountId            = account.Id,
                AccountNumber        = account.AccountNumber,
                AccountType          = account.AccountType,
                TotalMarketValue     = Math.Round(totalMarketValue, 2),
                AvailableCash        = account.AvailableCash,
                TotalGainLossAmount  = Math.Round(totalGainLoss, 2),
                TotalGainLossPercent = totalGainPercent,
                Holdings             = holdingDtos,
            };
        }

        public async Task<List<PortfolioSummaryDto>> GetPortfoliosForClientAsync(int clientId)
        {
            var accounts = await _context.InvestmentAccounts
                .Where(a => a.ClientId == clientId)
                .Select(a => a.Id)
                .ToListAsync();

            var summaries = new List<PortfolioSummaryDto>();

            foreach (var accountId in accounts)
            {
                var summary = await GetPortfolioSummaryAsync(accountId);

                if (summary != null)
                {
                    summaries.Add(summary);
                }
            }

            return summaries;
        }
    }
}
