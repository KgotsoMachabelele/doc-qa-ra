namespace Core.Domain;

public class Document
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public DateTime UploadedAt { get; set; }
    public long SizeBytes { get; set; }

    // Where the raw file lives on disk. Temporary until blob storage replaces local disk.
    public string StoragePath { get; set; } = string.Empty;
}
