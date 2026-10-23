namespace WealthTrack.API.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal AvailableCash { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OpenedAt { get; set; }
    }
}
