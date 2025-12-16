using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.ViewModels
{
    public class EventVM
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
