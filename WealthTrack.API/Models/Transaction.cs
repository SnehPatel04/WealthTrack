namespace WealthTrack.API.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int InvestmentAccountId { get; set; }
        public int? SecurityId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal PricePerShare { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public InvestmentAccount InvestmentAccount { get; set; } = null!;
        public Security? Security { get; set; }
    }
}
