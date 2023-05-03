using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        private static IRecentItems CreateNamedRecentItems(
            this IServiceProvider serviceProvider,
            string path,
            string name)
        {
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
