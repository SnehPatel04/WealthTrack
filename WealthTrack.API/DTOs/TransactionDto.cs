namespace WealthTrack.API.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int InvestmentAccountId { get; set; }
        public int? SecurityId { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string SecurityName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal PricePerShare { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}
