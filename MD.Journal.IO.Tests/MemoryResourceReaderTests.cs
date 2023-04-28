using MD.Journal.IO.Readers;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests
{
    public class MemoryResourceReaderTests
    {
        private readonly IResourceStore resourceStore;
        private readonly IOptions<ResourceReaderOptions> options;

        public MemoryResourceReaderTests(
            IResourceStore resourceStore,
            IOptions<ResourceReaderOptions> options)
        {
            this.resourceStore = resourceStore ?? throw new ArgumentNullException(nameof(resourceStore));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        private IEnumerable<string> AddResource(ResourceUri uri, int count)
        {
            var lines = Enumerable
                .Range(0, count)
                .Select(i => $"line {i}");

            return this.resourceStore
                .Resources
                .AddOrUpdate(uri, lines, (uri, lines) => lines);
        }

        [Fact]
        public async Task ReadAllLinesAsync_Test()
        {
            var uri = (ResourceUri)nameof(ReadAllLinesAsync_Test);
            var expectedLines = this.AddResource(uri, 100);

            var reader = new MemoryResourceReader(this.resourceStore, this.options);

            var actualLines = await reader.ReadAllLinesAsync(uri);

            Assert.Equal(expectedLines, actualLines);
        }
    }
}
