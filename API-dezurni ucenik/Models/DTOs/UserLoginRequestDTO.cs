using System.ComponentModel.DataAnnotations;

namespace API_dezurni_ucenik.Models.DTOs
{
    public class UserLoginRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
