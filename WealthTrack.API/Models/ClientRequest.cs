namespace WealthTrack.API.Models
{
    public class ClientRequest
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int AdvisorId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AdvisorResponse { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        public Client Client { get; set; } = null!;
        public Advisor Advisor { get; set; } = null!;
    }
}
