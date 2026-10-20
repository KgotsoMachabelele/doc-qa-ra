using Core.Domain;
using Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public class DocumentRepository(AppDbContext db) : IDocumentRepository
{
    public async Task AddAsync(Document document, CancellationToken ct = default)
    {
        db.Documents.Add(document);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Document>> ListAsync(CancellationToken ct = default) =>
        await db.Documents.AsNoTracking().OrderByDescending(d => d.UploadedAt).ToListAsync(ct);

    public Task<Document?> GetAsync(Guid id, CancellationToken ct = default) =>
        db.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);
}
