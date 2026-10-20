namespace Core.Services;

public interface IFileStorage
{
    /// <summary>Saves the stream and returns the path it was stored under.</summary>
    Task<string> SaveAsync(Guid documentId, string fileName, Stream content, CancellationToken ct = default);
    Task DeleteAsync(string path, CancellationToken ct = default);
}
