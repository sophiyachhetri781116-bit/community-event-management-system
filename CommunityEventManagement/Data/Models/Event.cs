using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Data.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, FutureOrTodayDate]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly Time { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Relationships
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Venue> Venues { get; set; } = new List<Venue>();          // M:N
        public ICollection<Activity> Activities { get; set; } = new List<Activity>(); // M:N
    }
}