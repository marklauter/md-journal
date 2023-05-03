using MD.Journal.IO;
using MD.Journal.IO.Recents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.Journals
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJournal(this IServiceCollection services)
        {
            services.TryAddTransient<Func<ResourceUri, IJournal>>(
                services =>
                uri => new Journal(uri));
            return services;
        }

        public static IServiceCollection AddJournalCatalog(
            this IServiceCollection services)
        {
            return services.AddTransient<IJournalCatalog>(servicesProvider =>
            {
                var createRecentItems = servicesProvider.GetRequiredService<Func<string, string, IRecentItems>>();
                var recentItems = createRecentItems(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "journals.recent");

                return new JournalCatalog(recentItems, servicesProvider);
            });
        }
    }
}
