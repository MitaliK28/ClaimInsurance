using insuranceclaimproject.Dtos.Document;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly InsuranceContext _context;

        public DocumentService(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<Document> UploadDocumentAsync(CreateDocumentDto documentData)
        {
            var claimExists = await _context.Claims.AnyAsync(c => c.ClaimId == documentData.ClaimId);
            if (!claimExists)
            {
                throw new InvalidOperationException("Claim not found for the given ClaimId.");
            }

            var newDocument = new Document
            {
                ClaimId = documentData.ClaimId,
                DocumentName = documentData.DocumentName,
                DocumentPath = documentData.DocumentPath,
                DocumentType = documentData.DocumentType,
                UploadedAt = DateTime.UtcNow
            };
            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();
            return newDocument;
        }

        public async Task<IEnumerable<Document>> GetDocumentsByClaimIdAsync(int claimId)
        {
            return await _context.Documents
                .Where(d => d.ClaimId == claimId)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            return await _context.Documents.FindAsync(documentId);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
            {
                return false;
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
