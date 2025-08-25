using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos.Policy
{
    public class CreatePolicyDto
    {
        //[Required(ErrorMessage = "Policy number is required.")]
        //[StringLength(50)]
        //public string PolicyNumber { get; set; } = default!;

        [Required(ErrorMessage = "Policyholder's user ID is required.")]
        public string? PolicyholderId { get; set; } = default!;

        [Required(ErrorMessage = "Coverage amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Coverage amount must be a positive number.")]
        public decimal CoverageAmount { get; set; }
    }
}