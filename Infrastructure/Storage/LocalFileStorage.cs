using Core.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IOptions<FileStorageOptions> options, IHostEnvironment env)
    {
        // Relative paths resolve against the content root, not the process working directory.
        _root = Path.GetFullPath(Path.Combine(env.ContentRootPath, options.Value.RootPath));
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Guid documentId, string fileName, Stream content, CancellationToken ct = default)
    {
        // Prefix with the id so two uploads with the same name never collide.
        var path = Path.Combine(_root, $"{documentId:N}_{Path.GetFileName(fileName)}");

        await using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await content.CopyToAsync(file, ct);

        return path;
    }

    public Task DeleteAsync(string path, CancellationToken ct = default)
    {
        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }
}
