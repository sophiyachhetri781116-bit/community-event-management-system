using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Data.Models
{
    public class Venue
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>(); // M:N
    }
}