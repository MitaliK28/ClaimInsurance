using insuranceclaimproject.Dtos;
//using insuranceclaimproject.Dtos.PolicyCategory;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public class PolicyCategoryService : IPolicyCategoryService
    {
        private readonly InsuranceContext _context;

        public PolicyCategoryService(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<PolicyCategory> CreatePolicyCategoryAsync(CreatePolicyCategoryDto dto)
        {
            var newCategory = new PolicyCategory
            {
                CategoryType = dto.CategoryType,
                UserId = dto.UserId
            };

            _context.PolicyCategories.Add(newCategory);
            await _context.SaveChangesAsync();
            return newCategory;
        }

        public async Task<IEnumerable<PolicyCategory>> GetAllPolicyCategoriesAsync()
        {
            return await _context.PolicyCategories
                .Include(c => c.User) // 👈 This ensures User data is loaded
                .ToListAsync();
        }




        public async Task<IEnumerable<PolicyCategory>> GetPolicyCategoriesByUserIdAsync(string userId)
        {
            return await _context.PolicyCategories
                                 .Where(pc => pc.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<UserPoliciesAndCategoriesDto> GetUserCategoriesAndPoliciesAsync(string userId)
        {
            var categories = await _context.PolicyCategories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var policies = await _context.Policies
                .Where(p => p.PolicyholderId == userId && p.PolicyStatus == PolicyStatus.Active)
                .ToListAsync(); // ✅ Only fetch active policies

            return new UserPoliciesAndCategoriesDto
            {
                UserId = userId,
                Categories = categories.Select(c => new PolicyCategoryDto
                {
                    PolicyCategoryId = c.PolicyCategoryId,
                    CategoryType = c.CategoryType,
                    UserId = c.UserId
                }).ToList(),
                Policies = policies.Select(p => new PolicyDto
                {
                    PolicyId = p.PolicyId,
                    PolicyNumber = p.PolicyNumber,
                    CoverageAmount = p.CoverageAmount,
                    PolicyStatus = p.PolicyStatus.ToString(),
                    CreatedDate = p.CreatedDate
                }).ToList()
            };
        }


        // New method to update policy category status to Active
        public async Task<PolicyCategory?> UpdatePolicyCategoryStatusToActiveAsync(int policyCategoryId)
        {
            var category = await _context.PolicyCategories.FindAsync(policyCategoryId);
            if (category == null)
            {
                return null;
            }

            category.Status = PolicyCategoryStatus.Active;
            await _context.SaveChangesAsync();
            return category;
        }

        // New method to get all policy categories with Pending status
        public async Task<IEnumerable<PolicyCategory>> GetPendingPolicyCategoriesAsync()
        {
            return await _context.PolicyCategories
              .Where(pc => pc.Status == PolicyCategoryStatus.Pending)
              .ToListAsync();
        }


        public async Task<Policy> CreatePolicyForUserAsync(string userId, decimal coverageAmount)
        {
            var policy = new Policy
            {
                PolicyholderId = userId,
                CoverageAmount = coverageAmount,
                PolicyNumber = $"POL-{Guid.NewGuid().ToString().Substring(0, 8)}", // Example format
                PolicyStatus = PolicyStatus.Active,
                CreatedDate = DateTime.UtcNow
            };

            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();
            return policy;
        }

    }
}
