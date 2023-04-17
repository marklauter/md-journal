using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal
{
    public sealed class TagGraph
    {
        private const string TagsFileName = "tags.txt";
        private readonly string path;

        public TagGraph(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            this.path = path;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string FileName(string tag)
        {
            var tagid = (TagId)tag;
            return Path.Combine(this.path, $"tag-map.{tagid}.txt");
        }

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
                var fileName = this.FileName(tag);
                var exists = File.Exists(fileName);
                if (!exists)
                {
                    await this.AppendTagsAsync(tag);
                }

                using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                using var writer = new StreamWriter(file);
                _ = writer.BaseStream.Seek(0, SeekOrigin.End);
                await writer.WriteLineAsync(journalEntry.Id);
                await writer.FlushAsync();
                fileNames[i++] = fileName;
            }

            return fileNames;
        }

        private async Task AppendTagsAsync(string tag)
        {
            var fileName = Path.Combine(this.path, TagsFileName);
            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            using var writer = new StreamWriter(file);
            _ = writer.BaseStream.Seek(0, SeekOrigin.End);
            await writer.WriteLineAsync(tag);
            await writer.FlushAsync();
        }

        public async Task<string[]> JournalEntriesAsync(string tag)
        {
            if (String.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or whitespace.", nameof(tag));
            }

            var fileName = this.FileName(tag);
            return !File.Exists(fileName)
                ? Array.Empty<string>()
                : await File.ReadAllLinesAsync(fileName);
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
