using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Models
{
    public enum UserRole
    {
        ADMIN,
        AGENT,
        CLAIM_ADJUSTER,
        POLICYHOLDER
    }
    public class AppUser : IdentityUser
    {
        //[Key]
        //public string UserId { get; set; }

        //[Required, StringLength(50)]
        //public string Username { get; set; }

        //[Required, StringLength(255)]
        //public string Password { get; set; } // Should be encrypted

        [Required]
        public UserRole Role { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }
    }
}