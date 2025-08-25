using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace insuranceclaimproject.Dtos
{

    public class SubmitClaimDto
    {
        [Required]
        public int PolicyId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Claim amount must be greater than zero.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ClaimAmount { get; set; }
    }
    // DTO for submitting a claim with document
    public class SubmitClaimWithDocumentDto
    {
        [Required(ErrorMessage = "Policy ID is required")]
        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Claim amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Claim amount must be greater than 0")]
        public decimal ClaimAmount { get; set; }

        [Required(ErrorMessage = "Document name is required")]
        [MaxLength(100, ErrorMessage = "Document name cannot exceed 100 characters")]
        public string DocumentName { get; set; } = default!;

        [Required(ErrorMessage = "Document file is required")]
        public IFormFile DocumentFile { get; set; } = default!;
    }

    // DTO for updating claim with document
    public class UpdateClaimWithDocumentDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Claim amount must be greater than 0")]
        public decimal? ClaimAmount { get; set; }

        [MaxLength(100, ErrorMessage = "Document name cannot exceed 100 characters")]
        public string? DocumentName { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }

    // Response DTO for claim with document
    public class ClaimWithDocumentResponseDto
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }
        public decimal ClaimAmount { get; set; }
        public DateTime ClaimDate { get; set; }
        public string ClaimStatus { get; set; } = default!;
        public string? AdjusterId { get; set; }
        public DocumentResponseDto? Document { get; set; }
    }

    public class DocumentResponseDto
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; } = default!;
        public string DocumentPath { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }
}