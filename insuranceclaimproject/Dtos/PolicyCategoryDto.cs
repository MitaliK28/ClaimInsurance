using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos
{
    public class PolicyCategoryDto
    {
        public int PolicyCategoryId { get; set; }
        public string CategoryType { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Status { get; set; } = default!;
    }

    public class UpdatePolicyCategoryStatusDto
    {
        [Required(ErrorMessage = "Policy Category ID is required.")]
        public int PolicyCategoryId { get; set; }
    }
    public class UserPoliciesAndCategoriesDto
    {
        public string UserId { get; set; } = default!;
        public List<PolicyCategoryDto> Categories { get; set; } = new();
        public List<PolicyDto> Policies { get; set; } = new();
    }

    public class PolicyDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = default!;
        public decimal CoverageAmount { get; set; }
        public string PolicyStatus { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
    }

    public class ActivateCategoryAndCreatePolicyDto
    {
        [Required(ErrorMessage = "Policy Category ID is required.")]
        public int PolicyCategoryId { get; set; }

        [Required(ErrorMessage = "Coverage Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Coverage amount must be greater than zero.")]
        public decimal CoverageAmount { get; set; }
    }

}
