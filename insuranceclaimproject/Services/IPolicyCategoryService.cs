using insuranceclaimproject.Dtos;
using insuranceclaimproject.Models;

namespace insuranceclaimproject.Services
{
    public interface IPolicyCategoryService
    {
        Task<PolicyCategory> CreatePolicyCategoryAsync(CreatePolicyCategoryDto dto);
        Task<IEnumerable<PolicyCategory>> GetAllPolicyCategoriesAsync();
        Task<IEnumerable<PolicyCategory>> GetPolicyCategoriesByUserIdAsync(string userId);

        Task<UserPoliciesAndCategoriesDto> GetUserCategoriesAndPoliciesAsync(string userId);
        Task<PolicyCategory?> UpdatePolicyCategoryStatusToActiveAsync(int policyCategoryId);
        Task<IEnumerable<PolicyCategory>> GetPendingPolicyCategoriesAsync();
        Task<Policy> CreatePolicyForUserAsync(string userId, decimal coverageAmount);
    }
}
