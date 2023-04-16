namespace MD.Journal.Tests
{
    public class TagGraphTests
    {
        [Fact]
        public async Task MapJournalEntryAsync_Creates_Files()
        {
            var entry = JournalEntryBuilder.Create()
                .WithTitle("title")
                .WithAuthor("author")
                .WithBody("body")
                .WithTags($"{nameof(MapJournalEntryAsync_Creates_Files)}tagone", $"{nameof(MapJournalEntryAsync_Creates_Files)}tagtwo")
                .Build();

            var graph = new TagGraph(".");
            var files = await graph.MapJournalEntryAsync(entry);
            Assert.True(File.Exists(files[0]));
            Assert.True(File.Exists(files[1]));

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        [Fact]
        public async Task JournalEntriesAsync_Returns_Files()
        {
            var entry = JournalEntryBuilder.Create()
                .WithTitle("title")
                .WithAuthor("author")
                .WithBody("body")
                .WithTags($"{nameof(JournalEntriesAsync_Returns_Files)}tagone", $"{nameof(JournalEntriesAsync_Returns_Files)}tagtwo")
                .Build();

            var graph = new TagGraph(".");
            var files = await graph.MapJournalEntryAsync(entry);
            Assert.True(File.Exists(files[0]));
            Assert.True(File.Exists(files[1]));

            var ids = await graph.JournalEntriesAsync($"{nameof(JournalEntriesAsync_Returns_Files)}tagone");
            Assert.Contains(entry.Id, ids);

            ids = await graph.JournalEntriesAsync($"{nameof(JournalEntriesAsync_Returns_Files)}tagtwo");
            Assert.Contains(entry.Id, ids);

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        [Fact]
        public async Task Tags_Returns_Tags()
        {
            var entry = JournalEntryBuilder.Create()
                .WithTitle("title")
                .WithAuthor("author")
                .WithBody("body")
                .WithTags($"{nameof(Tags_Returns_Tags)}tagone", $"{nameof(Tags_Returns_Tags)}tagtwo")
                .Build();

            var graph = new TagGraph(".");
            var files = await graph.MapJournalEntryAsync(entry);
            Assert.True(File.Exists(files[0]));
            Assert.True(File.Exists(files[1]));

            var tags = graph.Tags();
            Assert.Contains($"{nameof(Tags_Returns_Tags)}tagone", tags);
            Assert.Contains($"{nameof(Tags_Returns_Tags)}tagtwo", tags);

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
