using System.ComponentModel.DataAnnotations;

namespace ROOMLY.DTOs.AccountDTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string  Password { get; set; }
    }
}
