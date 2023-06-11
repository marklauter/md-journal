using MD.Journal.IO.Files;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.RecentRepositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRecentRepositoryService(
        this IServiceCollection services)
    {
        return services.AddTransient<Func<string, IRecentRepositoryService>>(
            serviceProvider =>
            (path) => serviceProvider.CreateRecentRepositoryService(path));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IRecentRepositoryService GetRecentRepositoryService(
        this IServiceProvider serviceProvider,
        string path)
    {
        return serviceProvider.GetRequiredService<Func<string, IRecentRepositoryService>>()(path);
    }

    private static IRecentRepositoryService CreateRecentRepositoryService(
        this IServiceProvider serviceProvider,
        string path)
    {
        if (String.IsNullOrEmpty(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
        }

        var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();

        return new RecentRepositoryService(
            path,
            fileSystem);
    }
}
