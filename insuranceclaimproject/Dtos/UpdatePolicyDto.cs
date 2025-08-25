using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos.Policy
{
    public class UpdatePolicyDto
    {
        [Required(ErrorMessage = "Coverage amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Coverage amount must be a positive number.")]
        public decimal CoverageAmount { get; set; }

        public PolicyStatus PolicyStatus { get; set; }
    }
}