using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MD.Journal.IO.Indexes
{
    internal sealed class IndexCatalog
        : IIndexCatalog
    {
        private readonly IServiceProvider serviceProvider;

        public IndexCatalog(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        public IIndex<TValue> Open<TValue>(ResourceUri uri) where TValue : IComparable<TValue>
        {
            var reader = this.serviceProvider.GetRequiredService<IResourceReader>();
            var writer = this.serviceProvider.GetRequiredService<IResourceWriter>();
            var logger = this.serviceProvider.GetRequiredService<ILogger<Index>>();
            return new Index<TValue>(
                uri,
                reader,
                writer,
                logger);
        }
    }
}
