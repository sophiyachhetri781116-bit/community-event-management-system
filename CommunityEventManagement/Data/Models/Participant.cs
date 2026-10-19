using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Data.Models
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        // Links this Participant record to the logged-in Identity account that owns it.
        // Null for participants added manually by an Admin who have no login account.
        public string? ApplicationUserId { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}