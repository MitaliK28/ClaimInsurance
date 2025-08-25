using insuranceclaimproject.Dtos.Policy;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PolicyController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPolicyService _policyService;

        public PolicyController(UserManager<AppUser> userManager, IPolicyService policyService)
        {
            _userManager = userManager;
            _policyService = policyService;
        }

        // POST api/Policy
        [HttpPost]
        [Authorize(Roles = "ADMIN, AGENT")]
        public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyDto createPolicyDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Verify the referenced user exists
            var user = await _userManager.FindByIdAsync(createPolicyDto.PolicyholderId);
            if (user == null)
                return BadRequest($"Invalid PolicyholderId: '{createPolicyDto.PolicyholderId}'. User not found.");

            var newPolicy = await _policyService.CreatePolicyAsync(createPolicyDto);
            return CreatedAtAction(nameof(GetPolicyDetails),
                new { policyId = newPolicy.PolicyId }, newPolicy);
        }

        // GET api/Policy/{policyId}
        [HttpGet("{policyId}")]
        [Authorize(Roles = "ADMIN, AGENT, CLAIM_ADJUSTER, POLICYHOLDER")]
        public async Task<IActionResult> GetPolicyDetails(int policyId)
        {
            var policy = await _policyService.GetPolicyByIdAsync(policyId);
            if (policy == null)
            {
                return NotFound(new { message = "Policy not found." });
            }

            return Ok(new PolicyDto
            {
                PolicyId = policy.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                PolicyholderId = policy.PolicyholderId,
                CoverageAmount = policy.CoverageAmount,
                PolicyStatus = policy.PolicyStatus.ToString(),
                CreatedDate = policy.CreatedDate
            });
        }

        // GET api/Policy
        [HttpGet]
        [Authorize(Roles = "ADMIN, AGENT, CLAIM_ADJUSTER")]
        public async Task<IActionResult> GetAllPolicies()
        {
            var policies = await _policyService.GetAllPoliciesAsync();
            var policyDtos = policies.Select(p => new PolicyDto
            {
                PolicyId = p.PolicyId,
                PolicyNumber = p.PolicyNumber,
                PolicyholderId = p.PolicyholderId,
                PolicyholderUsername = p.Policyholder?.UserName ?? "N/A",
                CoverageAmount = p.CoverageAmount,
                PolicyStatus = p.PolicyStatus.ToString(),
                CreatedDate = p.CreatedDate
            });
            return Ok(policyDtos);
        }

        // PUT api/Policy/{policyId}
        [HttpPut("{policyId}")]
        [Authorize(Roles = "ADMIN, AGENT")]
        public async Task<IActionResult> UpdatePolicy(int policyId, [FromBody] UpdatePolicyDto updatePolicyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _policyService.UpdatePolicyAsync(policyId, updatePolicyDto);
            if (!updated)
            {
                return NotFound(new { message = "Policy not found." });
            }
            return Ok(new { message = "Policy updated successfully." });
        }

        // DELETE api/Policy/{policyId}
        [HttpDelete("{policyId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeletePolicy(int policyId)
        {
            var deleted = await _policyService.DeletePolicyAsync(policyId);
            if (!deleted)
            {
                return NotFound(new { message = "Policy not found." });
            }
            return Ok(new { message = "Policy deleted successfully." });
        }

        // DELETE api/Policy/{policyId}
        [HttpPut("{policyId}/cancel")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CancelPolicy(int policyId)
        {
            // Call the updated service method to perform a "soft delete"
            var success = await _policyService.CancelPolicyAsync(policyId);

            if (!success)
            {
                return NotFound(new { message = "Policy not found." });
            }

            // Return a success message indicating the policy was cancelled
            return Ok(new { message = "Policy cancelled successfully." });
        }
    }
}