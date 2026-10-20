namespace Infrastructure.Storage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string RootPath { get; set; } = "uploads";
    public long MaxFileSizeBytes { get; set; } = 20 * 1024 * 1024;
}
