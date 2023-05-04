using MD.Journal.IO;
using MD.Journal.IO.Readers;
using MD.Journal.IO.Recents;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Journals
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJournal(this IServiceCollection services)
        {
            services.TryAddTransient<Func<ResourceUri, IJournal>>(
                services =>
                uri => new Journal(
                    uri,
                    services.GetRequiredService<IResourceReader>(),
                    services.GetRequiredService<IResourceWriter>(),
                    services));

            return services;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IJournal GetJournal(
            this IServiceProvider serviceProvider,
            ResourceUri uri)
        {
            return serviceProvider.GetRequiredService<Func<ResourceUri, IJournal>>()(uri);
        }

        public static IServiceCollection AddJournalCatalog(this IServiceCollection services)
        {
            return services.AddTransient<Func<string, string, IJournalCatalog>>(
                servicesProvider =>
                (path, name) =>
                {
                    var recentItems = servicesProvider.GetRecentItems(path, name);
                    return new JournalCatalog(recentItems, servicesProvider);
                });
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IJournalCatalog GetJournalCatalog(
            this IServiceProvider serviceProvider,
            string recentItemsPath,
            string recentItemsName)
        {
            return serviceProvider.GetRequiredService<Func<string, string, IJournalCatalog>>()(recentItemsPath, recentItemsName);
        }
    }
}
