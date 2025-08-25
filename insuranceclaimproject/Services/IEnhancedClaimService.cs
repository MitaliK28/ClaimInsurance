using insuranceclaimproject.Dtos;
using insuranceclaimproject.Models;

namespace insuranceclaimproject.Services
{
    public interface IEnhancedClaimService
    {
        Task<ClaimWithDocumentResponseDto> SubmitClaimWithDocumentAsync(SubmitClaimWithDocumentDto claimData);
        Task<ClaimWithDocumentResponseDto?> UpdateClaimWithDocumentAsync(int claimId, UpdateClaimWithDocumentDto updateData);
        Task<ClaimWithDocumentResponseDto?> GetClaimWithDocumentAsync(int claimId);
        Task<IEnumerable<ClaimWithDocumentResponseDto>> GetClaimsWithDocumentsByPolicyAsync(int policyId);
        Task<IEnumerable<ClaimWithDocumentResponseDto>> GetClaimsWithDocumentsByPolicyholderAsync(string policyholderId);


    }
}
