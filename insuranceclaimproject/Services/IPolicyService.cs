using insuranceclaimproject.Dtos.Policy;
using insuranceclaimproject.Models;

namespace insuranceclaimproject.Interfaces
{
    public interface IPolicyService
    {
        Task<Policy?> GetPolicyByIdAsync(int policyId);
        Task<IEnumerable<Policy>> GetAllPoliciesAsync();
        Task<Policy> CreatePolicyAsync(CreatePolicyDto policyData);
        Task<bool> UpdatePolicyAsync(int policyId, UpdatePolicyDto policyData);
        Task<bool> DeletePolicyAsync(int policyId);
        Task<bool> CancelPolicyAsync(int policyId);
    }
}