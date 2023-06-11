using MD.Journal.IO.Files;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.RecentRepositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRecentRepositories(
        this IServiceCollection services)
    {
        return services.AddTransient<Func<string, IRecentRepositories>>(
            serviceProvider =>
            (path) => serviceProvider.CreateRecentRepositories(path));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IRecentRepositories GetRecentRepositories(
        this IServiceProvider serviceProvider,
        string path)
    {
        return serviceProvider.GetRequiredService<Func<string, IRecentRepositories>>()(path);
    }

    private static IRecentRepositories CreateRecentRepositories(
        this IServiceProvider serviceProvider,
        string path)
    {
        if (String.IsNullOrEmpty(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
        }

        var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();

        return new RecentRepositories(
            path,
            fileSystem);
    }
}
