using insuranceclaimproject.Dtos;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using insuranceclaimproject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PolicyCategoryController : ControllerBase
    {
        private readonly IPolicyCategoryService _policyCategoryService;

        public PolicyCategoryController(IPolicyCategoryService policyCategoryService)
        {
            _policyCategoryService = policyCategoryService;
        }

        // POST: api/PolicyCategory
        // Creates a new policy category.
        [HttpPost]
        public async Task<IActionResult> CreatePolicyCategory([FromBody] CreatePolicyCategoryDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCategory = await _policyCategoryService.CreatePolicyCategoryAsync(createDto);

            var categoryDto = new PolicyCategoryDto
            {
                PolicyCategoryId = newCategory.PolicyCategoryId,
                CategoryType = newCategory.CategoryType,
                UserId = newCategory.UserId,
                Status = newCategory.Status.ToString()
            };

            return CreatedAtAction(nameof(GetPolicyCategoriesByUserId), new { userId = categoryDto.UserId }, categoryDto);
        }

        // GET: api/PolicyCategory
        // Gets all policy categories.
        [HttpGet]
        public async Task<IActionResult> GetAllPolicyCategories()
        {
            var categories = await _policyCategoryService.GetAllPolicyCategoriesAsync();
            var categoryDtos = categories.Select(c => new PolicyCategoryDto
            {
                PolicyCategoryId = c.PolicyCategoryId,
                CategoryType = c.CategoryType,
                UserId = c.UserId,
                Username = c.User?.UserName ?? "N/A",
                Status = c.Status.ToString()
            });
            return Ok(categoryDtos);
        }

        // GET: api/PolicyCategory/{userId}
        // Gets all policy categories for a specific user.
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPolicyCategoriesByUserId(string userId)
        {
            var categories = await _policyCategoryService.GetPolicyCategoriesByUserIdAsync(userId);
            if (!categories.Any())
            {
                return NotFound();
            }

            var categoryDtos = categories.Select(c => new PolicyCategoryDto
            {
                PolicyCategoryId = c.PolicyCategoryId,
                CategoryType = c.CategoryType,
                UserId = c.UserId,
                Status = c.Status.ToString()
            });
            return Ok(categoryDtos);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCategoriesAndPolicies(string userId)
        {
            var result = await _policyCategoryService.GetUserCategoriesAndPoliciesAsync(userId);

            if (result.Categories.Count == 0 && result.Policies.Count == 0)
            {
                return NotFound(new { message = "No categories or policies found for this user." });
            }

            return Ok(result);
        }

        // POST: api/PolicyCategory/activate
        [HttpPost("activate-and-create-policy")]
        public async Task<IActionResult> ActivateCategoryAndCreatePolicy([FromBody] ActivateCategoryAndCreatePolicyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Step 1: Activate the policy category
            var updatedCategory = await _policyCategoryService.UpdatePolicyCategoryStatusToActiveAsync(dto.PolicyCategoryId);
            if (updatedCategory == null)
            {
                return NotFound(new { message = "Policy category not found." });
            }

            // Step 2: Create a new policy for the same user
            var newPolicy = await _policyCategoryService.CreatePolicyForUserAsync(updatedCategory.UserId, dto.CoverageAmount);

            // Step 3: Return both updated category and new policy
            var response = new
            {
                Category = new PolicyCategoryDto
                {
                    PolicyCategoryId = updatedCategory.PolicyCategoryId,
                    CategoryType = updatedCategory.CategoryType,
                    UserId = updatedCategory.UserId,
                    Username = updatedCategory.User?.UserName ?? "N/A",
                    Status = updatedCategory.Status.ToString()
                },
                Policy = new PolicyDto
                {
                    PolicyId = newPolicy.PolicyId,
                    PolicyNumber = newPolicy.PolicyNumber,
                    CoverageAmount = newPolicy.CoverageAmount,
                    PolicyStatus = newPolicy.PolicyStatus.ToString(),
                    CreatedDate = newPolicy.CreatedDate
                }
            };

            return Ok(response);
        }


        // GET: api/PolicyCategory/pending
        // Gets all policy categories with a status of 'Pending'.
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPolicyCategories()
        {
            var pendingCategories = await _policyCategoryService.GetPendingPolicyCategoriesAsync();
            var categoryDtos = pendingCategories.Select(c => new PolicyCategoryDto
            {
                PolicyCategoryId = c.PolicyCategoryId,
                CategoryType = c.CategoryType,
                UserId = c.UserId,
                Status = c.Status.ToString()
            });
            return Ok(categoryDtos);
        }
    }
}