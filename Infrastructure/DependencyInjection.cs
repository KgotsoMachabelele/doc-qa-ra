using Core.Services;
using Infrastructure.DataAccess;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Scoped: one DbContext per HTTP request.
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddOptions<FileStorageOptions>()
            .Bind(configuration.GetSection(FileStorageOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.RootPath), "FileStorage:RootPath is required.")
            .Validate(o => o.MaxFileSizeBytes > 0, "FileStorage:MaxFileSizeBytes must be positive.")
            .ValidateOnStart();

        // Scoped because it depends on the scoped DbContext.
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        // Singleton: stateless, holds only the resolved root path.
        services.AddSingleton<IFileStorage, LocalFileStorage>();

        return services;
    }
}
