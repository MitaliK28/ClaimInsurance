using insuranceclaimproject.Models;

namespace insuranceclaimproject.Services
{
    public interface IFileHelperService
    {
        DocumentType DetectDocumentType(IFormFile file);
        Task<string> SaveFileAsync(IFormFile file, string fileName);
        bool IsValidDocumentType(IFormFile file);
        void DeleteFile(string filePath);
    }

    public class FileHelperService : IFileHelperService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadPath;

        public FileHelperService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "documents");

            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public DocumentType DetectDocumentType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => DocumentType.Pdf,
                ".jpg" or ".jpeg" => DocumentType.Jpg,
                ".png" => DocumentType.Png,
                ".doc" or ".docx" => DocumentType.Doc,
                _ => throw new NotSupportedException($"File type '{extension}' is not supported. Supported types: PDF, JPG, PNG, DOC, DOCX")
            };
        }

        public bool IsValidDocumentType(IFormFile file)
        {
            try
            {
                DetectDocumentType(file);
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string fileName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", "documents", uniqueFileName);
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}