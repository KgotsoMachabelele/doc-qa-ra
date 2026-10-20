using Core.Domain;

namespace Core.Services;

public class DocumentService(IDocumentRepository repository, IFileStorage storage) : IDocumentService
{
    public async Task<Document> UploadAsync(string fileName, long sizeBytes, Stream content, CancellationToken ct = default)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            FileName = Path.GetFileName(fileName),
            SizeBytes = sizeBytes,
            Status = DocumentStatus.Uploaded,
            UploadedAt = DateTime.UtcNow
        };

        document.StoragePath = await storage.SaveAsync(document.Id, document.FileName, content, ct);

        try
        {
            await repository.AddAsync(document, ct);
        }
        catch
        {
            // Don't leave an orphaned file on disk if the metadata write fails.
            await storage.DeleteAsync(document.StoragePath, CancellationToken.None);
            throw;
        }

        return document;
    }

    public Task<IReadOnlyList<Document>> ListAsync(CancellationToken ct = default) => repository.ListAsync(ct);

    public Task<Document?> GetAsync(Guid id, CancellationToken ct = default) => repository.GetAsync(id, ct);
}
