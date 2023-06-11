using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.IO.Files;

public static class FileSystemServiceCollectionExtensions
{
    public static IServiceCollection AddPhysicalFileSystem(this IServiceCollection services)
    {
        services.TryAddSingleton<IFileSystem, PhysicalFileSystem>();
        return services;
    }

    public static IServiceCollection AddVirtualFileSystem(this IServiceCollection services)
    {
        services.TryAddSingleton<IFileSystem, VirtualFileSystem>();
        return services;
    }
}
