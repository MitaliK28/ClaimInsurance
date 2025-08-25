using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos
{
    public class CreatePolicyCategoryDto
    {
        [Required(ErrorMessage = "Category type is required.")]
        public string CategoryType { get; set; } = default!;

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = default!;
    }
}
