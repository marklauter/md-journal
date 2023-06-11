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

    public static IServiceCollection AddVirtualFileSystemAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<IFileSystem, VirtualFileSystem>();
        return services;
    }

    public static IServiceCollection AddVirtualFileSystemAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<IFileSystem, VirtualFileSystem>();
        return services;
    }
}
