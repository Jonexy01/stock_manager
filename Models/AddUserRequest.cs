using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class AddUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Roles = ["manager", "staff"]
        /// </summary>
        public string Role { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
