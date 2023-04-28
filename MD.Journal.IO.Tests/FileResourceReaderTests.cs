using MD.Journal.IO.Readers;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests
{
    public class FileResourceReaderTests
    {
        private readonly IOptions<ResourceReaderOptions> options;

        public FileResourceReaderTests(
            IOptions<ResourceReaderOptions> options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
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
            var uri = (ResourceUri)$"{nameof(ReadAllLinesAsync_Returns_All_Lines)}-{count}";
            var expectedLines = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options);

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
            var uri = (ResourceUri)$"{nameof(ReadLinesAsync_Returns_Correct_Lines)}-{count}";
            var expectedLines = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options);

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
            var uri = (ResourceUri)$"{nameof(ReadLinesAsync_Returns_PaginationToken_With_Correct_Eof_Value)}-{count}";
            var expectedLines = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options);

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
            var uri = (ResourceUri)$"{nameof(ReadNextLinesAsync_Returns_Correct_Array_Length)}-{count}";
            var expectedLines = await AddResourceAsync(uri, count);

            var reader = new FileResourceReader(this.options);

            var actualResponse = await reader.ReadLinesAsync(uri);
            actualResponse = await reader.ReadLinesAsync(actualResponse.PaginationToken);

            Assert.Equal(this.options.Value.PageSize, actualResponse.Lines.Length);
            Assert.Equal(this.options.Value.PageSize * 2, actualResponse.PaginationToken.NextPageStart);
        }
    }
}
