using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests.Writers
{
    public sealed class MemoryResourceWriterTests
        : ResourceWriterTests
    {
        public MemoryResourceWriterTests(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override IResourceReader GetReader()
        {
            var resourceStore = this.ServiceProvider.GetRequiredService<IResourceStore>();
            var options = this.ServiceProvider.GetRequiredService<IOptions<ResourceReaderOptions>>();
            var logger = this.ServiceProvider.GetRequiredService<ILogger<MemoryResourceReader>>();

            return new MemoryResourceReader(resourceStore, options, logger);
        }

        protected override IResourceWriter GetWriter()
        {
            var resourceStore = this.ServiceProvider.GetRequiredService<IResourceStore>();
            var logger = this.ServiceProvider.GetRequiredService<ILogger<MemoryResourceWriter>>();

            return new MemoryResourceWriter(resourceStore, logger);
        }
    }
}
