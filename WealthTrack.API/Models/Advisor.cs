namespace WealthTrack.API.Models
{
    public class Advisor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        public User User { get; set; } = null!;
        public ICollection<AdvisorClient> AdvisorClients { get; set; } = new List<AdvisorClient>();
    }
}
