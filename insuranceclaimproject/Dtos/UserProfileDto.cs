using insuranceclaimproject.Models;

namespace insuranceclaimproject.Dtos
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public UserRole Role { get; set; }
    }
}