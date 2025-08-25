using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}