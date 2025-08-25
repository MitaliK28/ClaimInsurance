using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Add this using directive

namespace insuranceclaimproject.Models
{
    public enum PolicyStatus { Active = 1, Inactive = 2, Cancelled = 3 }

    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }
        [Required, MaxLength(50)]
        public string PolicyNumber { get; set; } = default!;
        public string PolicyholderId { get; set; } = default!;
        [ForeignKey("PolicyholderId")]
        public AppUser? Policyholder { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CoverageAmount { get; set; }
        public PolicyStatus PolicyStatus { get; set; } = PolicyStatus.Active;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Uncomment this to establish the one-to-many relationship
        public ICollection<Claim>? Claims { get; set; }
    }
}