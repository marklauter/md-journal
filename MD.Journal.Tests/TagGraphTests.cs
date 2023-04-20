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
                .WithTags("tagone", "tagtwo")
                .Build();

            var path = $@".\{nameof(MapJournalEntryAsync_Creates_Files)}";
            var graph = new TagGraph(path);
            var files = await graph.MapJournalEntryAsync(entry);
            Assert.True(File.Exists(files[0]));
            Assert.True(File.Exists(files[1]));
            Assert.True(File.Exists(Path.Combine(path, "tags", "tags.txt")));

            File.Delete(Path.Combine(path, "tags", "tags.txt"));
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
                .WithTags("tagone", "tagtwo")
                .Build();

            var path = $@".\{nameof(JournalEntriesAsync_Returns_Files)}";
            var graph = new TagGraph(path);
            var files = await graph.MapJournalEntryAsync(entry);
            Assert.True(File.Exists(files[0]));
            Assert.True(File.Exists(files[1]));
            Assert.True(File.Exists(Path.Combine(path, "tags", "tags.txt")));

            var ids = (await graph.JournalEntriesAsync("tagone"))
                .Select(entry => entry.JournalEntryId);
            Assert.Contains(entry.Id, ids);

            ids = (await graph.JournalEntriesAsync("tagtwo"))
                .Select(entry => entry.JournalEntryId);
            Assert.Contains(entry.Id, ids);

            File.Delete(Path.Combine(path, "tags", "tags.txt"));
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
                .WithTags("tagone", "tagtwo")
                .Build();

            var path = $@".\{nameof(Tags_Returns_Tags)}";
            var graph = new TagGraph(path);
            var files = await graph.MapJournalEntryAsync(entry);
            Assert.True(File.Exists(files[0]));
            Assert.True(File.Exists(files[1]));
            Assert.True(File.Exists(Path.Combine(path, "tags", "tags.txt")));

            var tags = await graph.TagsAsync();
            Assert.Contains("tagone", tags);
            Assert.Contains("tagtwo", tags);

            File.Delete(Path.Combine(path, "tags", "tags.txt"));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
