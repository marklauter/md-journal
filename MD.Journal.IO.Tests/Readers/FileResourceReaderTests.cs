using MD.Journal.IO.Pagination;
using MD.Journal.IO.Readers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests.Readers
{
    public sealed class FileResourceReaderTests
    {
        private readonly IOptions<PaginationOptions> options;
        private readonly ILogger<FileResourceReader> logger;

        public FileResourceReaderTests(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.options = serviceProvider.GetRequiredService<IOptions<PaginationOptions>>();
            this.logger = serviceProvider.GetRequiredService<ILogger<FileResourceReader>>();
        }

        private static async Task<string[]> AddResourceAsync(ResourceUri uri, int count)
        {
            var lines = Enumerable
                .Range(0, count)
                .Select(i => $"line {i}")
                .ToArray();

            await File.WriteAllLinesAsync(uri, lines);
            return lines;
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
            var expectedLines = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options, this.logger);

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
            var expectedLines = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options, this.logger);

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

            _ = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options, this.logger);

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

            _ = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options, this.logger);

            var actualResponse = await reader.ReadLinesAsync(uri);
            actualResponse = await reader.ReadLinesAsync(actualResponse.PaginationToken);

            Assert.Equal(this.options.Value.PageSize, actualResponse.Lines.Length);
            Assert.Equal(this.options.Value.PageSize * 2, actualResponse.PaginationToken.NextPageStart);
        }
    }
}
