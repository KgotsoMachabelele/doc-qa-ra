using Core.Domain;
using Core.Services;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace doc_qa_ra.Controllers;

[ApiController]
[Route("documents")]
public class DocumentsController(IDocumentService documents, IOptions<FileStorageOptions> storageOptions) : ControllerBase
{
    private static readonly string[] AllowedExtensions = [".pdf"];

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<DocumentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return Problem("A non-empty file is required.", statusCode: StatusCodes.Status400BadRequest);

        var maxBytes = storageOptions.Value.MaxFileSizeBytes;
        if (file.Length > maxBytes)
            return Problem($"File exceeds the {maxBytes / (1024 * 1024)} MB limit.", statusCode: StatusCodes.Status400BadRequest);

        if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
            return Problem("Only PDF files are supported.", statusCode: StatusCodes.Status400BadRequest);

        await using var stream = file.OpenReadStream();
        var document = await documents.UploadAsync(file.FileName, file.Length, stream, ct);

        return CreatedAtAction(nameof(Get), new { id = document.Id }, DocumentResponse.From(document));
    }

    [HttpGet]
    public async Task<IEnumerable<DocumentResponse>> List(CancellationToken ct) =>
        (await documents.ListAsync(ct)).Select(DocumentResponse.From);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<DocumentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var document = await documents.GetAsync(id, ct);
        return document is null ? NotFound() : Ok(DocumentResponse.From(document));
    }
}

// API contract kept separate from the entity so StoragePath never leaks to clients.
public record DocumentResponse(Guid Id, string FileName, string Status, DateTime UploadedAt, long SizeBytes)
{
    public static DocumentResponse From(Document d) =>
        new(d.Id, d.FileName, d.Status.ToString(), d.UploadedAt, d.SizeBytes);
}
