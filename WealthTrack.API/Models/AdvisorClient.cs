namespace WealthTrack.API.Models
{
    public class AdvisorClient
    {
        public int Id { get; set; }
        public int AdvisorId { get; set; }
        public int ClientId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public Advisor Advisor { get; set; } = null!;
        public Client Client { get; set; } = null!;
    }
}
