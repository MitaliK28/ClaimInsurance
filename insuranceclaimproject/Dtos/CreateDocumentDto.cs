using System.ComponentModel.DataAnnotations;
using insuranceclaimproject.Models;

namespace insuranceclaimproject.Dtos.Document
{
    public class CreateDocumentDto
    {
        [Required(ErrorMessage = "Claim ID is required.")]
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Document name is required.")]
        [StringLength(100)]
        public string DocumentName { get; set; } = default!;

        [Required(ErrorMessage = "Document path is required.")]
        [StringLength(255)]
        public string DocumentPath { get; set; } = default!;

        public DocumentType DocumentType { get; set; }
    }
}