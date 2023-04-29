using MD.Journal.IO.Pagination;
using MD.Journal.IO.Readers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests.Readers
{
    public sealed class MemoryResourceReaderTests
    {
        private readonly IResourceStore resourceStore;
        private readonly IOptions<PaginationOptions> options;
        private readonly ILogger<MemoryResourceReader> logger;

        public MemoryResourceReaderTests(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.resourceStore = serviceProvider.GetRequiredService<IResourceStore>();
            this.options = serviceProvider.GetRequiredService<IOptions<PaginationOptions>>();
            this.logger = serviceProvider.GetRequiredService<ILogger<MemoryResourceReader>>();
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(20)]
        public async Task ReadAllLinesAsync_Returns_All_Lines(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();
            var expectedLines = this.AddResource(uri, count);

            var reader = new MemoryResourceReader(
                this.resourceStore,
                this.options,
                this.logger);

            var actualLines = await reader.ReadAllLinesAsync(uri);

            Assert.Equal(count, actualLines.Count());
            Assert.Equal(expectedLines, actualLines);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(20)]
        public async Task ReadLinesAsync_Returns_Correct_Lines(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();
            var expectedLines = this.AddResource(uri, count).ToArray();

            var reader = new MemoryResourceReader(
                this.resourceStore,
                this.options,
                this.logger);

            var actualResponse = await reader.ReadLinesAsync(uri);

            var length = count < this.options.Value.PageSize
                ? count
                : this.options.Value.PageSize;
            Assert.Equal(length, actualResponse.Lines.Length);
            Assert.Equal(expectedLines[..length], actualResponse.Lines);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        [InlineData(20)]
        public async Task ReadLinesAsync_Returns_PaginationToken_With_Correct_Eof_Value(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();

            _ = this.AddResource(uri, count).ToArray();

            var reader = new MemoryResourceReader(
                this.resourceStore,
                this.options,
                this.logger);

            var actualResponse = await reader.ReadLinesAsync(uri);

            if (count < this.options.Value.PageSize)
            {
                Assert.True(actualResponse.PaginationToken.EndOfFile);
            }
            else
            {
                Assert.False(actualResponse.PaginationToken.EndOfFile);
            }
        }

        [Theory]
        [InlineData(8)]
        [InlineData(20)]
        public async Task ReadNextLinesAsync_Returns_Correct_Array_Length(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();

            _ = this.AddResource(uri, count).ToArray();

            var reader = new MemoryResourceReader(
                this.resourceStore,
                this.options,
                this.logger);

            var actualResponse = await reader.ReadLinesAsync(uri);
            actualResponse = await reader.ReadLinesAsync(actualResponse.PaginationToken);

            Assert.Equal(this.options.Value.PageSize, actualResponse.Lines.Length);
            Assert.Equal(this.options.Value.PageSize * 2, actualResponse.PaginationToken.NextPageStart);
        }
    }
}
