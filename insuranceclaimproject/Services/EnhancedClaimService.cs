using insuranceclaimproject.Dtos;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public class EnhancedClaimService : IEnhancedClaimService
    {
        private readonly InsuranceContext _context;
        private readonly IFileHelperService _fileHelperService;

        public EnhancedClaimService(InsuranceContext context, IFileHelperService fileHelperService)
        {
            _context = context;
            _fileHelperService = fileHelperService;
        }

        public async Task<ClaimWithDocumentResponseDto> SubmitClaimWithDocumentAsync(SubmitClaimWithDocumentDto claimData)
        {
            // Validate policy exists
            var policyExists = await _context.Policies.AnyAsync(p => p.PolicyId == claimData.PolicyId);
            if (!policyExists)
            {
                throw new InvalidOperationException("Policy not found for the given PolicyId.");
            }


            // Step 1: Get the policy
            var policy = await _context.Policies
                .Include(p => p.Claims)
                .FirstOrDefaultAsync(p => p.PolicyId == claimData.PolicyId);

            if (policy == null)
            {
                throw new InvalidOperationException("Policy not found for the given PolicyId.");
            }

            if (policy.PolicyStatus != PolicyStatus.Active)
            {
                throw new InvalidOperationException(
                    $"Claims can only be submitted for active policies. Current status: {policy.PolicyStatus}"
                );
            }


            // Step 2: Calculate total claimed amount so far
            decimal existingClaimAmountTotal = policy.Claims?.Sum(c => c.ClaimAmount) ?? 0;

            // Step 3: Calculate remaining coverage
            decimal remainingCoverage = policy.CoverageAmount - existingClaimAmountTotal;

            // Step 4: Validate new claim amount
            if (claimData.ClaimAmount > remainingCoverage)
            {
                throw new InvalidOperationException(
                    $"Claim amount exceeds remaining coverage. You can claim up to {remainingCoverage:C2}."
                );
            }


            // Validate document type
            if (!_fileHelperService.IsValidDocumentType(claimData.DocumentFile))
            {
                throw new InvalidOperationException("Invalid document type. Supported types: PDF, JPG, PNG, DOC, DOCX");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create claim
                var newClaim = new Claim
                {
                    PolicyId = claimData.PolicyId,
                    ClaimAmount = claimData.ClaimAmount,
                    ClaimDate = DateTime.UtcNow,
                    ClaimStatus = ClaimStatus.Pending
                };

                _context.Claims.Add(newClaim);
                await _context.SaveChangesAsync();

                // Save document file
                var documentPath = await _fileHelperService.SaveFileAsync(claimData.DocumentFile, claimData.DocumentName);

                // Create document record
                var document = new Document
                {
                    ClaimId = newClaim.ClaimId,
                    DocumentName = claimData.DocumentName,
                    DocumentPath = documentPath,
                    DocumentType = _fileHelperService.DetectDocumentType(claimData.DocumentFile),
                    UploadedAt = DateTime.UtcNow
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return response DTO
                return new ClaimWithDocumentResponseDto
                {
                    ClaimId = newClaim.ClaimId,
                    PolicyId = newClaim.PolicyId,
                    ClaimAmount = newClaim.ClaimAmount,
                    ClaimDate = newClaim.ClaimDate,
                    ClaimStatus = newClaim.ClaimStatus.ToString(),
                    AdjusterId = newClaim.AdjusterId,
                    Document = new DocumentResponseDto
                    {
                        DocumentId = document.DocumentId,
                        DocumentName = document.DocumentName,
                        DocumentPath = document.DocumentPath,
                        DocumentType = document.DocumentType.ToString(),
                        UploadedAt = document.UploadedAt
                    }
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ClaimWithDocumentResponseDto?> UpdateClaimWithDocumentAsync(int claimId, UpdateClaimWithDocumentDto updateData)
        {
            var claim = await _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
            {
                return null;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update claim amount if provided
                if (updateData.ClaimAmount.HasValue)
                {
                    claim.ClaimAmount = updateData.ClaimAmount.Value;
                }

                Document? document = claim.Documents?.FirstOrDefault();

                // Handle document update
                if (updateData.DocumentFile != null)
                {
                    // Validate document type
                    if (!_fileHelperService.IsValidDocumentType(updateData.DocumentFile))
                    {
                        throw new InvalidOperationException("Invalid document type. Supported types: PDF, JPG, PNG, DOC, DOCX");
                    }

                    // Delete old file if exists
                    if (document != null)
                    {
                        _fileHelperService.DeleteFile(document.DocumentPath);
                    }

                    // Save new file
                    var documentName = updateData.DocumentName ?? document?.DocumentName ?? "updated_document";
                    var documentPath = await _fileHelperService.SaveFileAsync(updateData.DocumentFile, documentName);

                    if (document != null)
                    {
                        // Update existing document
                        document.DocumentName = documentName;
                        document.DocumentPath = documentPath;
                        document.DocumentType = _fileHelperService.DetectDocumentType(updateData.DocumentFile);
                        document.UploadedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new document
                        document = new Document
                        {
                            ClaimId = claim.ClaimId,
                            DocumentName = documentName,
                            DocumentPath = documentPath,
                            DocumentType = _fileHelperService.DetectDocumentType(updateData.DocumentFile),
                            UploadedAt = DateTime.UtcNow
                        };
                        _context.Documents.Add(document);
                    }
                }
                else if (updateData.DocumentName != null && document != null)
                {
                    // Update only document name
                    document.DocumentName = updateData.DocumentName;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ClaimWithDocumentResponseDto
                {
                    ClaimId = claim.ClaimId,
                    PolicyId = claim.PolicyId,
                    ClaimAmount = claim.ClaimAmount,
                    ClaimDate = claim.ClaimDate,
                    ClaimStatus = claim.ClaimStatus.ToString(),
                    AdjusterId = claim.AdjusterId,
                    Document = document != null ? new DocumentResponseDto
                    {
                        DocumentId = document.DocumentId,
                        DocumentName = document.DocumentName,
                        DocumentPath = document.DocumentPath,
                        DocumentType = document.DocumentType.ToString(),
                        UploadedAt = document.UploadedAt
                    } : null
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<ClaimWithDocumentResponseDto>> GetClaimsWithDocumentsByPolicyAsync(int policyId)
        {
            var claims = await _context.Claims
                .Where(c => c.PolicyId == policyId)
                .Include(c => c.Documents)
                .ToListAsync();

            return claims.Select(c => new ClaimWithDocumentResponseDto
            {
                ClaimId = c.ClaimId,
                PolicyId = c.PolicyId,
                ClaimAmount = c.ClaimAmount,
                ClaimDate = c.ClaimDate,
                ClaimStatus = c.ClaimStatus.ToString(),
                AdjusterId = c.AdjusterId,
                Document = c.Documents?.FirstOrDefault() != null ? new DocumentResponseDto
                {
                    DocumentId = c.Documents.First().DocumentId,
                    DocumentName = c.Documents.First().DocumentName,
                    DocumentPath = c.Documents.First().DocumentPath,
                    DocumentType = c.Documents.First().DocumentType.ToString(),
                    UploadedAt = c.Documents.First().UploadedAt
                } : null
            });
        }

        public async Task<IEnumerable<ClaimWithDocumentResponseDto>> GetClaimsWithDocumentsByPolicyholderAsync(string policyholderId)
        {
            var claims = await _context.Claims
                .Include(c => c.Documents)
                .Include(c => c.Policy)
                .Where(c => c.Policy.PolicyholderId == policyholderId)
                .ToListAsync();

            return claims.Select(c => new ClaimWithDocumentResponseDto
            {
                ClaimId = c.ClaimId,
                PolicyId = c.PolicyId,
                ClaimAmount = c.ClaimAmount,
                ClaimDate = c.ClaimDate,
                ClaimStatus = c.ClaimStatus.ToString(),
                AdjusterId = c.AdjusterId,
                Document = c.Documents?.FirstOrDefault() != null ? new DocumentResponseDto
                {
                    DocumentId = c.Documents.First().DocumentId,
                    DocumentName = c.Documents.First().DocumentName,
                    DocumentPath = c.Documents.First().DocumentPath,
                    DocumentType = c.Documents.First().DocumentType.ToString(),
                    UploadedAt = c.Documents.First().UploadedAt
                } : null
            });
        }

        public async Task<ClaimWithDocumentResponseDto?> GetClaimWithDocumentAsync(int claimId)
        {
            var claim = await _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
            {
                return null;
            }

            var document = claim.Documents?.FirstOrDefault();

            return new ClaimWithDocumentResponseDto
            {
                ClaimId = claim.ClaimId,
                PolicyId = claim.PolicyId,
                ClaimAmount = claim.ClaimAmount,
                ClaimDate = claim.ClaimDate,
                ClaimStatus = claim.ClaimStatus.ToString(),
                AdjusterId = claim.AdjusterId,
                Document = document != null ? new DocumentResponseDto
                {
                    DocumentId = document.DocumentId,
                    DocumentName = document.DocumentName,
                    DocumentPath = document.DocumentPath,
                    DocumentType = document.DocumentType.ToString(),
                    UploadedAt = document.UploadedAt
                } : null
            };
        }
    }
}