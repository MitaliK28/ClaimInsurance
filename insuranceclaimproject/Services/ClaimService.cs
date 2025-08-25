using insuranceclaimproject.Dtos;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public class ClaimService : IClaimService
    {
        private readonly InsuranceContext _context;

        public ClaimService(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<Claim> SubmitClaimAsync(SubmitClaimDto claimData)
        {
            // You should first check if the policy exists before creating the claim
            var policyExists = await _context.Policies.AnyAsync(p => p.PolicyId == claimData.PolicyId);
            if (!policyExists)
            {
                // This is a business logic error, handle it appropriately
                throw new InvalidOperationException("Policy not found for the given PolicyId.");
            }

            var newClaim = new Claim
            {
                PolicyId = claimData.PolicyId,
                ClaimAmount = claimData.ClaimAmount,
                ClaimDate = DateTime.UtcNow,
                ClaimStatus = ClaimStatus.Pending
            };

            _context.Claims.Add(newClaim);
            await _context.SaveChangesAsync();
            return newClaim;
        }

        public async Task<Claim?> GetClaimDetailsAsync(int claimId)
        {
            return await _context.Claims
                                 .Include(c => c.Policy)
                                 .Include(c => c.Documents)
                                 .FirstOrDefaultAsync(c => c.ClaimId == claimId);
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, ClaimStatus status)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return false;
            }

            claim.ClaimStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
        {
            return await _context.Claims.Include(c => c.Policy).Include(c => c.Documents).ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByPolicyAsync(int policyId)
        {
            return await _context.Claims
                                 .Where(c => c.PolicyId == policyId)
                                 .Include(c => c.Policy)
                                 .Include(c => c.Documents)
                                 .ToListAsync();
        }
    }
}