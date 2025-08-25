using insuranceclaimproject.Dtos.Document;
using insuranceclaimproject.Models;

namespace insuranceclaimproject.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(CreateDocumentDto documentData);
        Task<IEnumerable<Document>> GetDocumentsByClaimIdAsync(int claimId);
        Task<Document?> GetDocumentByIdAsync(int documentId);
        Task<bool> DeleteDocumentAsync(int documentId);
    }
}