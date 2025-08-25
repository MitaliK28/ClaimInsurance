using insuranceclaimproject.Models;
using insuranceclaimproject.Dtos;
using insuranceclaimproject.Controllers;

namespace insuranceclaimproject.Interfaces
{
    public interface IClaimService
    {
        Task<Claim> SubmitClaimAsync(SubmitClaimDto claimData);
        Task<Claim?> GetClaimDetailsAsync(int claimId);
        Task<bool> UpdateClaimStatusAsync(int claimId, ClaimStatus status);
        Task<IEnumerable<Claim>> GetAllClaimsAsync(); // Add this
        Task<IEnumerable<Claim>> GetClaimsByPolicyAsync(int policyId); // Add this
   
    }
}