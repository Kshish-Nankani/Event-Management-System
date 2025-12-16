using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string Location { get; set; }

        public int CreatedBy { get; set; } // Admin User Id
    }
}
