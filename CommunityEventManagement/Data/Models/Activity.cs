using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Data.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Type { get; set; }   // e.g. Workshop, Talk, Game

        public ICollection<Event> Events { get; set; } = new List<Event>(); // M:N
    }
}