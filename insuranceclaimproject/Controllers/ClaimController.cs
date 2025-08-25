using insuranceclaimproject.Dtos;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using insuranceclaimproject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;


namespace insuranceclaimproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints in this controller require authentication
    public class ClaimController : ControllerBase
    {
        private readonly IClaimService _claimService;
        private readonly IEnhancedClaimService _enhancedClaimService;

        public ClaimController(IClaimService claimService, IEnhancedClaimService enhancedClaimService)
        {
            _claimService = claimService;
            _enhancedClaimService = enhancedClaimService;
        }

        /// <summary>
        /// Submit a basic claim (without document)
        /// </summary>
        /// <param name="claimDto">Basic claim data</param>
        /// <returns>Created claim details</returns>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitClaim([FromBody] SubmitClaimDto claimDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var submittedClaim = await _claimService.SubmitClaimAsync(claimDto);
                return CreatedAtAction(
                    nameof(GetClaimDetails),
                    new { claimId = submittedClaim.ClaimId },
                    submittedClaim
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Submit a claim with document upload (Enhanced Version)
        /// </summary>
        /// <param name="claimData">Claim submission data with document</param>
        /// <returns>Created claim with document details</returns>
        [HttpPost("submit-with-document")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN,POLICYHOLDER")]
        public async Task<IActionResult> SubmitClaimWithDocument([FromForm] SubmitClaimWithDocumentDto claimData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _enhancedClaimService.SubmitClaimWithDocumentAsync(claimData);
                return CreatedAtAction(
                    nameof(GetClaimWithDocument),
                    new { claimId = result.ClaimId },
                    result
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get basic claim details
        /// </summary>
        /// <param name="claimId">Claim ID</param>
        /// <returns>Claim details</returns>
        [HttpGet("{claimId}")]
        public async Task<IActionResult> GetClaimDetails(int claimId)
        {
            try
            {
                var claim = await _claimService.GetClaimDetailsAsync(claimId);
                if (claim == null)
                {
                    return NotFound(new { message = "Claim not found." });
                }
                var responseDto = MapToClaimWithDocumentResponseDto(claim);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get claim details with associated document (Enhanced Version)
        /// </summary>
        /// <param name="claimId">Claim ID</param>
        /// <returns>Claim with document details</returns>
        [HttpGet("{claimId}/with-document")]
        public async Task<IActionResult> GetClaimWithDocument(int claimId)
        {
            try
            {
                var result = await _enhancedClaimService.GetClaimWithDocumentAsync(claimId);

                if (result == null)
                {
                    return NotFound(new { message = "Claim not found." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all claims
        /// </summary>
        /// <returns>List of all claims</returns>
        [HttpGet("all")]
        [Authorize(Roles = "ADMIN,CLAIM_ADJUSTER,POLICYHOLDER")] // Only admins and adjusters can view all claims
        public async Task<IActionResult> GetAllClaims()
        {
            try
            {
                var claims = await _claimService.GetAllClaimsAsync();
                var responseDtos = claims.Select(c => MapToClaimWithDocumentResponseDto(c));
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        // Helper method to map from Claim model to response DTO
        // Helper method to map from Claim model to response DTO
        // Helper method to map from Claim model to response DTO
        private ClaimWithDocumentResponseDto MapToClaimWithDocumentResponseDto(insuranceclaimproject.Models.Claim claim)
        {
            return new ClaimWithDocumentResponseDto
            {
                ClaimId = claim.ClaimId,
                PolicyId = claim.PolicyId,
                ClaimAmount = claim.ClaimAmount,
                ClaimDate = claim.ClaimDate,
                ClaimStatus = claim.ClaimStatus.ToString(),
                AdjusterId = claim.AdjusterId,
                Document = claim.Documents?.FirstOrDefault() != null ? new DocumentResponseDto
                {
                    DocumentId = claim.Documents.FirstOrDefault().DocumentId,
                    DocumentName = claim.Documents.FirstOrDefault().DocumentName,
                    DocumentPath = claim.Documents.FirstOrDefault().DocumentPath,
                    DocumentType = claim.Documents.FirstOrDefault().DocumentType.ToString(), // <-- FIX IS HERE
                    UploadedAt = claim.Documents.FirstOrDefault().UploadedAt
                } : null
            };
        }

        /// <summary>
        /// Get claims with document(s) by Policy ID
        /// </summary>
        /// <param name="policyId">Policy ID</param>
        /// <returns>List of claims with documents</returns>
        [HttpGet("policy/{policyId}/with-documents")]
        [Authorize(Roles = "ADMIN,POLICYHOLDER,CLAIM_ADJUSTER")]
        public async Task<IActionResult> GetClaimsWithDocumentsByPolicy(int policyId)
        {
            try
            {
                var claims = await _enhancedClaimService.GetClaimsWithDocumentsByPolicyAsync(policyId);
                if (claims == null || !claims.Any())
                {
                    return NotFound(new { message = "No claims found for this policy." });
                }

                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get all claims with documents by Policyholder ID
        /// </summary>
        /// <param name="policyholderId">Policyholder ID</param>
        /// <returns>List of claims with documents for the policyholder</returns>
        [HttpGet("policyholder/{policyholderId}/with-documents")]
        [Authorize(Roles = "ADMIN,POLICYHOLDER,CLAIM_ADJUSTER")]
        public async Task<IActionResult> GetClaimsWithDocumentsByPolicyholder(string policyholderId)
        {
            try
            {
                var claims = await _enhancedClaimService.GetClaimsWithDocumentsByPolicyholderAsync(policyholderId);

                if (claims == null || !claims.Any())
                {
                    return NotFound(new { message = "No claims found for this policyholder." });
                }

                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.Message
                });
            }
        }



        /// <summary>
        /// Update claim status (Basic Version)
        /// </summary>
        /// <param name="claimId">Claim ID</param>
        /// <param name="statusDto">Status update data</param>
        /// <returns>Success/failure response</returns>
        [HttpPut("{claimId}/status")]
        [Authorize(Roles = "ADMIN,CLAIM_ADJUSTER")] // Only admins and adjusters can change claim status
        public async Task<IActionResult> UpdateClaimStatus(int claimId, [FromBody] UpdateClaimStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _claimService.UpdateClaimStatusAsync(claimId, statusDto.Status);
                if (!success)
                {
                    return NotFound(new { message = "Claim not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update claim with optional document update (Enhanced Version)
        /// </summary>
        /// <param name="claimId">Claim ID to update</param>
        /// <param name="updateData">Update data with optional document</param>
        /// <returns>Updated claim with document details</returns>
        [HttpPut("{claimId}/update-with-document")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateClaimWithDocument(int claimId, [FromForm] UpdateClaimWithDocumentDto updateData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _enhancedClaimService.UpdateClaimWithDocumentAsync(claimId, updateData);

                if (result == null)
                {
                    return NotFound(new { message = "Claim not found." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Download document associated with a claim
        /// </summary>
        /// <param name="claimId">Claim ID</param>
        /// <returns>Document file</returns>
        [HttpGet("{claimId}/download-document")]
        public async Task<IActionResult> DownloadDocument(int claimId)
        {
            try
            {
                var claimWithDocument = await _enhancedClaimService.GetClaimWithDocumentAsync(claimId);

                if (claimWithDocument?.Document == null)
                {
                    return NotFound(new { message = "Document not found for this claim." });
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", claimWithDocument.Document.DocumentPath);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "Physical file not found." });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(claimWithDocument.Document.DocumentType);

                return File(fileBytes, contentType, claimWithDocument.Document.DocumentName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while downloading the document.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user's own claims (for authenticated users to see their claims)
        /// </summary>
        /// <returns>List of current user's claims</returns>
        [HttpGet("my-claims")]
        public async Task<IActionResult> GetMyClaims()
        {
            try
            {
                // Get user ID from claims (assuming you store user ID in JWT token)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User ID not found in token." });
                }

                // This would require additional implementation in your service layer
                // to filter claims by user ID or implement user-specific logic
                var allClaims = await _claimService.GetAllClaimsAsync();

                // For now, returning all claims, but you should filter by user
                // Example: var userClaims = allClaims.Where(c => c.Policy.UserId == userIdClaim);

                return Ok(allClaims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete a claim (Admin/Adjuster only)
        /// </summary>
        /// <param name="claimId">Claim ID to delete</param>
        /// <returns>Success/failure response</returns>
        [HttpDelete("{claimId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteClaim(int claimId)
        {
            try
            {
                // This would require implementation in your service layer
                // For now, returning method not implemented
                return StatusCode(501, new { message = "Delete functionality not implemented yet." });

                /*
                // Example implementation:
                var success = await _claimService.DeleteClaimAsync(claimId);
                if (!success)
                {
                    return NotFound(new { message = "Claim not found." });
                }
                return NoContent();
                */
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Assign adjuster to a claim
        /// </summary>
        /// <param name="claimId">Claim ID</param>
        /// <param name="assignDto">Adjuster assignment data</param>
        /// <returns>Success/failure response</returns>
        [HttpPut("{claimId}/assign-adjuster")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AssignAdjuster(int claimId, [FromBody] AssignAdjusterDto assignDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // This would require implementation in your service layer
                return StatusCode(501, new { message = "Assign adjuster functionality not implemented yet." });

                /*
                // Example implementation:
                var success = await _claimService.AssignAdjusterAsync(claimId, assignDto.AdjusterId);
                if (!success)
                {
                    return NotFound(new { message = "Claim not found." });
                }
                return NoContent();
                */
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get claim statistics (Admin/Adjuster only)
        /// </summary>
        /// <returns>Claim statistics</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "ADMIN,CLAIM_ADJUSTER")]
        public async Task<IActionResult> GetClaimStatistics()
        {
            try
            {
                var allClaims = await _claimService.GetAllClaimsAsync();

                var statistics = new
                {
                    TotalClaims = allClaims.Count(),
                    PendingClaims = allClaims.Count(c => c.ClaimStatus == ClaimStatus.Pending),
                    ApprovedClaims = allClaims.Count(c => c.ClaimStatus == ClaimStatus.Approved),
                    RejectedClaims = allClaims.Count(c => c.ClaimStatus == ClaimStatus.Rejected),
                    TotalClaimAmount = allClaims.Sum(c => c.ClaimAmount),
                    AverageClaimAmount = allClaims.Any() ? allClaims.Average(c => c.ClaimAmount) : 0,
                    ClaimsByStatus = allClaims.GroupBy(c => c.ClaimStatus)
                                           .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                                           .ToList()
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to get content type for file download
        /// </summary>
        /// <param name="documentType">Document type string</param>
        /// <returns>MIME content type</returns>
        private static string GetContentType(string documentType)
        {
            return documentType.ToUpperInvariant() switch
            {
                "PDF" => "application/pdf",
                "JPG" => "image/jpeg",
                "PNG" => "image/png",
                "DOC" => "application/msword",
                _ => "application/octet-stream"
            };
        }
    }



    // Additional DTOs that might be needed
    public class AssignAdjusterDto
    {
        [Required]
        public string AdjusterId { get; set; } = default!;
    }
}