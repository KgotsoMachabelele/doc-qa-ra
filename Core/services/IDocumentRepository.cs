using Core.Domain;

namespace Core.Services;

public interface IDocumentRepository
{
    Task AddAsync(Document document, CancellationToken ct = default);
    Task<IReadOnlyList<Document>> ListAsync(CancellationToken ct = default);
    Task<Document?> GetAsync(Guid id, CancellationToken ct = default);
}
