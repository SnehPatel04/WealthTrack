using Microsoft.EntityFrameworkCore;
using WealthTrack.API.Data;
using WealthTrack.API.DTOs;

namespace WealthTrack.API.Services
{
    public class TransactionService
    {
        private readonly WealthTrackDbContext _context;

        public TransactionService(WealthTrackDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> GetTransactionsForAccountAsync(int accountId)
        {
            return await _context.Transactions
                .Where(t => t.InvestmentAccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    Id                  = t.Id,
                    InvestmentAccountId = t.InvestmentAccountId,
                    SecurityId          = t.SecurityId,
                    Ticker              = t.Security != null ? t.Security.Ticker : string.Empty,
                    SecurityName        = t.Security != null ? t.Security.Name  : string.Empty,
                    TransactionType     = t.TransactionType,
                    Quantity            = t.Quantity,
                    PricePerShare       = t.PricePerShare,
                    TotalAmount         = t.TotalAmount,
                    Notes               = t.Notes,
                    TransactionDate     = t.TransactionDate,
                })
                .ToListAsync();
        }

    }
}