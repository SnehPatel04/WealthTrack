namespace WealthTrack.API.Models
{
    public class Holding
    {
        public int Id { get; set; }
        public int InvestmentAccountId { get; set; }
        public int SecurityId { get; set; }
        public decimal Quantity { get; set; }
        public decimal AverageCostBasis { get; set; }

        public InvestmentAccount InvestmentAccount { get; set; } = null!;
        public Security Security { get; set; } = null!;
    }
}
