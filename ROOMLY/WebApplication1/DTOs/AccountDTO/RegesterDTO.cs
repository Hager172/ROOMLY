using System.ComponentModel.DataAnnotations;

namespace ROOMLY.DTOs.AccountDTO
{
    public class RegesterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string  FullName { get; set; }

        [Required]
        [Phone]
        public string  PhoneNumber { get; set; }

        [Required]
       
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
     ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]

        public string Password { get; set; }

    }
}
