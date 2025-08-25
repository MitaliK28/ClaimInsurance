using insuranceclaimproject.Dtos.Policy;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly InsuranceContext _context;

        public PolicyService(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<Policy?> GetPolicyByIdAsync(int policyId)
        {
            return await _context.Policies
                                 .Include(p => p.Policyholder)
                                 .FirstOrDefaultAsync(p => p.PolicyId == policyId);
        }

        public async Task<IEnumerable<Policy>> GetAllPoliciesAsync()
        {
            return await _context.Policies
                .Include(p => p.Policyholder)
                .Where(p => p.PolicyStatus != PolicyStatus.Cancelled)
                .ToListAsync();
        }


        public async Task<Policy> CreatePolicyAsync(CreatePolicyDto policyData)
        {
            // --- New Logic to Auto-Generate Policy Number ---
            var lastPolicy = await _context.Policies.OrderByDescending(p => p.PolicyId).FirstOrDefaultAsync();
            var nextNumber = (lastPolicy?.PolicyId ?? 0) + 1;
            var newPolicyNumber = $"POL-{DateTime.UtcNow.Year}-{nextNumber:D6}";
            // --- End of New Logic ---

            var policy = new Policy
            {
                PolicyNumber = newPolicyNumber, // Assign the auto-generated number
                PolicyholderId = policyData.PolicyholderId,
                CoverageAmount = policyData.CoverageAmount,
                PolicyStatus = PolicyStatus.Active,
                CreatedDate = DateTime.UtcNow
            };
            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();
            return policy;
        }

        public async Task<bool> UpdatePolicyAsync(int policyId, UpdatePolicyDto policyData)
        {
            var policy = await _context.Policies.FindAsync(policyId);
            if (policy == null)
            {
                return false;
            }

            policy.CoverageAmount = policyData.CoverageAmount;
            policy.PolicyStatus = policyData.PolicyStatus;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePolicyAsync(int policyId)
        {
            // First, check if there are any claims associated with this policy.
            var hasClaims = await _context.Claims.AnyAsync(c => c.PolicyId == policyId);

            if (hasClaims)
            {
                // If claims exist, we cannot delete the policy.
                // Throw a custom exception that can be caught in the controller
                // to return a user-friendly message.
                throw new InvalidOperationException("This policy cannot be deleted because it has associated claims.");
            }

            // If there are no claims, proceed with deletion.
            var policy = await _context.Policies.FindAsync(policyId);
            if (policy == null)
            {
                return false; // Policy not found
            }

            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPolicyAsync(int policyId)
        {
            // Find the policy that needs to be cancelled.
            var policy = await _context.Policies.FindAsync(policyId);

            if (policy == null)
            {
                // The policy was not found in the database.
                return false;
            }

            // Update the status of the policy to "Cancelled" instead of deleting it.
            // This assumes your Policy entity has a string property named 'PolicyStatus'.
            policy.PolicyStatus = (PolicyStatus)3;


            // Mark the entity as modified and save the changes.
            _context.Policies.Update(policy);
            await _context.SaveChangesAsync();

            // Return true to indicate the operation was successful.
            return true;
        }
    }
}