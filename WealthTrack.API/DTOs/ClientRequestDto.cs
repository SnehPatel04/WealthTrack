using System.ComponentModel.DataAnnotations;

namespace WealthTrack.API.DTOs
{
    public class ClientRequestDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int AdvisorId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AdvisorResponse { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class CreateClientRequestDto
    {
        [Required]
        public string RequestType { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }

    public class RespondToRequestDto
    {
        [Required]
        public string AdvisorResponse { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
