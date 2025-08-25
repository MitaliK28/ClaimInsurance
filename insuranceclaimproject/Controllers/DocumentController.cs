using insuranceclaimproject.Dtos.Document;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace insuranceclaimproject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // POST api/Document/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromBody] CreateDocumentDto documentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newDocument = await _documentService.UploadDocumentAsync(documentDto);
                return CreatedAtAction(
                    nameof(GetDocumentById),
                    new { documentId = newDocument.DocumentId },
                    newDocument
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/Document/claim/{claimId}
        [HttpGet("claim/{claimId}")]
        public async Task<IActionResult> GetDocumentsByClaimId(int claimId)
        {
            var documents = await _documentService.GetDocumentsByClaimIdAsync(claimId);
            return Ok(documents);
        }

        // GET api/Document/{documentId}
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById(int documentId)
        {
            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                return NotFound(new { message = "Document not found." });
            }

            // Map the model to the DTO before returning
            var documentDto = new DocumentDto
            {
                DocumentId = document.DocumentId,
                ClaimId = document.ClaimId,
                DocumentName = document.DocumentName,
                DocumentPath = document.DocumentPath,
                DocumentType = document.DocumentType.ToString(),
                UploadedAt = document.UploadedAt
            };
            return Ok(documentDto);
        }

        // DELETE api/Document/{documentId}
        [HttpDelete("{documentId}")]
        [Authorize(Roles = "ADMIN, AGENT, CLAIM_ADJUSTER")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var deleted = await _documentService.DeleteDocumentAsync(documentId);
            if (!deleted)
            {
                return NotFound(new { message = "Document not found." });
            }
            return NoContent();
        }
    }
}
