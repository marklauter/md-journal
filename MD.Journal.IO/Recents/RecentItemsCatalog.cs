using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Recents
{
    internal sealed class RecentItemsCatalog
        : IRecentItemsCatalog
    {
        private readonly IServiceProvider serviceProvider;

        public RecentItemsCatalog(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IRecentItems Open(ResourceUri uri)
        {
            var reader = this.serviceProvider.GetRequiredService<IResourceReader>();
            var writer = this.serviceProvider.GetRequiredService<IResourceWriter>();
            var logger = this.serviceProvider.GetRequiredService<ILogger<RecentItems>>();
            var options = this.serviceProvider.GetRequiredService<IOptions<RecentItemsOptions>>();

            return new RecentItems(
                uri,
                reader,
                writer,
                options,
                logger);
        }
    }
}
