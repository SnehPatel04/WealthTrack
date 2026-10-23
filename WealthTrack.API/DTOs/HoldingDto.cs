namespace WealthTrack.API.DTOs
{
    public class HoldingDto
    {
        public int Id { get; set; }
        public int InvestmentAccountId { get; set; }
        public int SecurityId { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string SecurityName { get; set; } = string.Empty;
        public string AssetClass { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal AverageCostBasis { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MarketValue { get; set; }
        public decimal GainLossAmount { get; set; }
        public decimal GainLossPercent { get; set; }
        public decimal AllocationPercent { get; set; }
    }
}
