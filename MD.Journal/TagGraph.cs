using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal
{
    internal sealed class TagGraph
    {
        private const string TagsFolderName = "tags";
        private const string TagsFileName = "tags.txt";
        private readonly string path;

        public TagGraph(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            path = Path.Combine(path, TagsFolderName);
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            this.path = path;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GenerateTagFileName(string tag)
        {
            var tagid = (TagId)tag;
            return Path.Combine(this.path, $"tag-map.{tagid}.txt");
        }

        // returns the filenames of the new tag graph indexes - useful only for unit testing right now
        public async Task<string[]> MapJournalEntryAsync(JournalEntry journalEntry)
        {
            if (journalEntry is null)
            {
                throw new ArgumentNullException(nameof(journalEntry));
            }

            var fileNames = new string[journalEntry.Tags.Length];
            var i = 0;
            foreach (var tag in journalEntry.Tags)
            {
                var fileName = this.GenerateTagFileName(tag);
                var exists = File.Exists(fileName);
                if (!exists)
                {
                    await this.AppendTagsAsync(tag);
                }

                var newTagEntryLine = JsonConvert.SerializeObject(new JournalIndexEntry(journalEntry.Id, journalEntry.Date));
                await File.AppendAllTextAsync(fileName, newTagEntryLine);
                fileNames[i++] = fileName;
            }

            return fileNames;
        }

        private async Task AppendTagsAsync(string tag)
        {
            var fileName = Path.Combine(this.path, TagsFileName);
            await File.AppendAllTextAsync(fileName, tag);
        }

        public async Task<JournalIndexEntry[]> JournalEntriesAsync(string tag)
        {
            if (String.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or whitespace.", nameof(tag));
            }

            var fileName = this.GenerateTagFileName(tag);
            return !File.Exists(fileName)
                ? Array.Empty<JournalIndexEntry>()
                : (await File.ReadAllLinesAsync(fileName))
                    .Select(JsonConvert.DeserializeObject<JournalIndexEntry>)
                    .ToArray();
        }

        public async Task<string[]> TagsAsync()
        {
            var fileName = Path.Combine(this.path, TagsFileName);
            return !File.Exists(fileName)
                ? Array.Empty<string>()
                : await File.ReadAllLinesAsync(fileName);
        }
    }
}
