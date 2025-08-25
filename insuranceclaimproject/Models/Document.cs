using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace insuranceclaimproject.Models
{
    public enum DocumentType { Pdf = 1, Jpg = 2, Png = 3, Doc = 4 }

    public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        [Required]
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }
        [Required, MaxLength(100)]
        public string DocumentName { get; set; } = default!;
        [Required, MaxLength(255)]
        public string DocumentPath { get; set; } = default!;
        public DocumentType DocumentType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
