using System.ComponentModel.DataAnnotations;

namespace WealthTrack.API.DTOs
{
    public class AssignAdvisorDto
    {
        [Required]
        public int AdvisorId { get; set; }

        [Required]
        public int ClientId { get; set; }
    }
}
