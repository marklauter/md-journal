using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Recents
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRecentItems(
            this IServiceCollection services)
        {
            _ = services
                .AddResourceReader()
                .AddResourceWriter();

            return services.AddTransient<Func<string, string, IRecentItems>>(
                serviceProvider =>
                (path, name) => serviceProvider.CreateNamedRecentItems(path, name));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRecentItems GetRecentItems(
            this IServiceProvider serviceProvider,
            string path,
            string name)
        {
            return serviceProvider.GetRequiredService<Func<string, string, IRecentItems>>()(path, name);
        }

        private static IRecentItems CreateNamedRecentItems(
            this IServiceProvider serviceProvider,
            string path,
            string name)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            var reader = serviceProvider.GetRequiredService<IResourceReader>();
            var writer = serviceProvider.GetRequiredService<IResourceWriter>();
            var logger = serviceProvider.GetRequiredService<ILogger<RecentItems>>();
            var options = Options.Create(new RecentItemsOptions { Name = name, Path = path });

            return new RecentItems(
                reader,
                writer,
                options,
                logger);
        }
    }
}
