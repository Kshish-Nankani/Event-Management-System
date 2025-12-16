using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
