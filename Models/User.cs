using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class User
    {
        public Guid Id { get; set; }
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
        [Required]
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
