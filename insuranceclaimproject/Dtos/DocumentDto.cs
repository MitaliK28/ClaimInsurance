using insuranceclaimproject.Models;

namespace insuranceclaimproject.Dtos.Document
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public int ClaimId { get; set; }
        public string DocumentName { get; set; } = default!;
        public string DocumentPath { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }
}