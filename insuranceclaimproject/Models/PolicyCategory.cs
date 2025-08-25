using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace insuranceclaimproject.Models
{
    public enum PolicyCategoryStatus
    {
        Active,
        Pending
    }

    // New model to link policy categories to users
    public class PolicyCategory
    {
        [Key]
        public int PolicyCategoryId { get; set; }

        // This is now a simple string field
        [Required]
        public string CategoryType { get; set; } = default!;

        // Foreign key to the AppUser table
        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public PolicyCategoryStatus Status { get; set; } = PolicyCategoryStatus.Pending;
    }
}
