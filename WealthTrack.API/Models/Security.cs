namespace WealthTrack.API.Models
{
    public class Security
    {
        public int Id { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AssetClass { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }

        public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
