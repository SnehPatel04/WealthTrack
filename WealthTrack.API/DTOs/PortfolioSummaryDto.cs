namespace WealthTrack.API.DTOs
{
    public class PortfolioSummaryDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal TotalMarketValue { get; set; }
        public decimal AvailableCash { get; set; }
        public decimal TotalGainLossAmount { get; set; }
        public decimal TotalGainLossPercent { get; set; }
        public List<HoldingDto> Holdings { get; set; } = new List<HoldingDto>();
    }
}

