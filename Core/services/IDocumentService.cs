using Core.Domain;

namespace Core.Services;

public interface IDocumentService
{
    Task<Document> UploadAsync(string fileName, long sizeBytes, Stream content, CancellationToken ct = default);
    Task<IReadOnlyList<Document>> ListAsync(CancellationToken ct = default);
    Task<Document?> GetAsync(Guid id, CancellationToken ct = default);
}
