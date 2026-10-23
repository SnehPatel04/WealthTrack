namespace WealthTrack.API.Models
{
    public class InvestmentAccount
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal AvailableCash { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

        public Client Client { get; set; } = null!;
        public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
