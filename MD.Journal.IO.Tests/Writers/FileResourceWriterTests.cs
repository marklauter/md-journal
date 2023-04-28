using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests.Writers
{
    public sealed class FileResourceWriterTests
        : ResourceWriterTests
    {
        public FileResourceWriterTests(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override IResourceReader GetReader()
        {
            var options = this.ServiceProvider.GetRequiredService<IOptions<ResourceReaderOptions>>();
            var logger = this.ServiceProvider.GetRequiredService<ILogger<FileResourceReader>>();

            return new FileResourceReader(options, logger);
        }

        protected override IResourceWriter GetWriter()
        {
            var logger = this.ServiceProvider.GetRequiredService<ILogger<FileResourceWriter>>();

            return new FileResourceWriter(logger);
        }
    }
}
