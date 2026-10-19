using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Data.Models
{
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        // Foreign keys
        public int EventId { get; set; }
        public int ParticipantId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";   // Pending / Confirmed / Cancelled

        // Relationships
        public Event? Event { get; set; }
        public Participant? Participant { get; set; }
    }
}