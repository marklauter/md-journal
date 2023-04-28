using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;

namespace MD.Journal.IO.Tests.Writers
{
    public abstract class ResourceWriterTests
    {
        public ResourceWriterTests(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        protected abstract IResourceReader GetReader();

        protected abstract IResourceWriter GetWriter();

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(20)]
        public async Task AppendLineAsync_Test(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();
            var writer = this.GetWriter();
            var reader = this.GetReader();

            var expectedLines = Enumerable
                .Range(0, count)
                .Select(i => $"line {i}")
                .ToArray();

            foreach (var line in expectedLines)
            {
                await writer.AppendLineAsync(uri, line);
            }

            var actualLines = await reader.ReadAllLinesAsync(uri);
            Assert.Equal(expectedLines, actualLines);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(20)]
        public async Task AppendTextAsync_Test(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();
            var writer = this.GetWriter();
            var reader = this.GetReader();

            var expectedLines = Enumerable
                .Range(0, count)
                .Select(i => $"line {i}")
                .ToArray();

            foreach (var line in expectedLines)
            {
                await writer.AppendTextAsync(uri, line);
            }

            var actualText = await reader.ReadTextAsync(uri);
            Assert.Equal(String.Join(String.Empty, expectedLines), actualText);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(20)]
        public async Task OverwriteAllLinesAsync_Test(int count)
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();
            var writer = this.GetWriter();
            var reader = this.GetReader();

            var expectedLines = Enumerable
                .Range(0, count)
                .Select(i => $"line {i}")
                .ToArray();

            await writer.OverwriteAllLinesAsync(uri, expectedLines);

            var actualLines = await reader.ReadAllLinesAsync(uri);
            Assert.Equal(expectedLines, actualLines);
        }

        [Fact]
        public async Task OverwriteAllTextAsync_Test()
        {
            var uri = (ResourceUri)Guid.NewGuid().ToString();
            var writer = this.GetWriter();
            var reader = this.GetReader();

            var expectedText = "text";
            await writer.OverwriteAllTextAsync(uri, expectedText);

            var actualText = await reader.ReadTextAsync(uri);
            Assert.Equal(expectedText, actualText);
        }
    }
}
