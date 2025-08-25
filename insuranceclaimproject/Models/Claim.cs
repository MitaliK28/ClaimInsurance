using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace insuranceclaimproject.Models
{
    public enum ClaimStatus { Pending = 1, Approved = 2, Rejected = 3 }
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        [Required]
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ClaimAmount { get; set; }
        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;
        public ClaimStatus ClaimStatus { get; set; } = ClaimStatus.Pending;
        public string? AdjusterId { get; set; } // optional until assigned
        public AppUser? Adjuster { get; set; }
        public ICollection<Document>? Documents { get; set; }
    }

}
