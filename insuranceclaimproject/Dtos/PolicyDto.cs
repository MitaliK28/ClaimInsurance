using insuranceclaimproject.Models;

namespace insuranceclaimproject.Dtos.Policy
{
    public class PolicyDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = default!;
        public string PolicyholderId { get; set; } = default!;
        public string PolicyholderUsername { get; set; } = default!;
        public decimal CoverageAmount { get; set; }
        public string PolicyStatus { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
    }
}