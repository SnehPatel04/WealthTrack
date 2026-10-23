namespace WealthTrack.API.Models
{
    public class Client
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public User User { get; set; } = null!;
        public ICollection<AdvisorClient> AdvisorClients { get; set; } = new List<AdvisorClient>();
        public ICollection<InvestmentAccount> InvestmentAccounts { get; set; } = new List<InvestmentAccount>();
        public ICollection<ClientRequest> ClientRequests { get; set; } = new List<ClientRequest>();
    }
}
