using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
