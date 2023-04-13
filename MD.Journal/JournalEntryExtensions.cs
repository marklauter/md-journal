using Grynwald.MarkdownGenerator;
using Newtonsoft.Json;

namespace MD.Journal
{
    public static class JournalEntryExtensions
    {
        public static MdDocument ToMarkdownDocument(this JournalEntry entry)
        {
            if (String.IsNullOrEmpty(entry.Id))
            {
                throw new InvalidOperationException(nameof(JournalEntry.Id));
            }

            if (String.IsNullOrEmpty(entry.Title))
            {
                throw new InvalidOperationException(nameof(JournalEntry.Title));
            }

            if (String.IsNullOrEmpty(entry.Author))
            {
                throw new InvalidOperationException(nameof(JournalEntry.Author));
            }

            if (String.IsNullOrEmpty(entry.RawContent))
            {
                throw new InvalidOperationException(nameof(JournalEntry.RawContent));
            }

            var header = new MdContainerBlock(
                new MdHeading(1, entry.Title),
                new MdParagraph(new MdEmphasisSpan($"By {entry.Author}, {entry.Date:G}")));

            if (entry.Tags.Any())
            {
                header.Add(new MdParagraph(
                    new MdStrongEmphasisSpan("Tags"),
                    new MdRawMarkdownSpan(Environment.NewLine),
                    new MdEmphasisSpan(String.Join(' ', entry.Tags))));
            }

            if (entry.Summary != String.Empty)
            {
                header.Add(new MdParagraph(
                    new MdStrongEmphasisSpan("Summary"),
                    new MdRawMarkdownSpan(Environment.NewLine),
                    new MdEmphasisSpan(entry.Summary)));
            }

            var body = new MdContainerBlock();
            for (var i = 0; i < entry.Paragraphs.Length; i++)
            {
                body.Add(new MdParagraph(new MdRawMarkdownSpan(entry.Paragraphs[i])));
            }

            return new MdDocument(header, body);
        }

        public static MemoryStream ToMarkdownStream(this JournalEntry entry)
        {
            var stream = new MemoryStream();
            entry.ToMarkdownDocument().Save(stream);
            stream.Position = 0;

            return stream;
        }

        public static async Task SaveAsJsonAsync(
            this JournalEntry entry,
            CancellationToken cancellationToken)
        {
            var fileName = $"{entry.Id}.json";
            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var streamWriter = new StreamWriter(file);
            using var jsonWriter = new JsonTextWriter(streamWriter);
            JsonSerializer
                .Create(new JsonSerializerSettings { Formatting = Formatting.Indented })
                .Serialize(jsonWriter, entry);
            await file.FlushAsync(cancellationToken);
        }

        public static async Task SaveAsJsonAsync(
            this JournalEntry entry,
            string path,
            CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            _ = Directory.CreateDirectory(path);
            var fileName = Path.Combine(path, $"{entry.Id}.json");

            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var streamWriter = new StreamWriter(file);
            using var jsonWriter = new JsonTextWriter(streamWriter);
            new JsonSerializer().Serialize(jsonWriter, entry);
            await file.FlushAsync(cancellationToken);
        }

        public static async Task SaveAsMarkdownAsync(
            this JournalEntry entry,
            CancellationToken cancellationToken)
        {
            var fileName = $"{entry.Id}.md";
            using var markdown = entry.ToMarkdownStream();
            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            await markdown.CopyToAsync(file, cancellationToken);
            await file.FlushAsync(cancellationToken);
        }

        public static async Task SaveAsMarkdownAsync(
            this JournalEntry entry,
            string path,
            CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            _ = Directory.CreateDirectory(path);
            var fileName = Path.Combine(path, $"{entry.Id}.md");

            using var markdown = entry.ToMarkdownStream();
            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            await markdown.CopyToAsync(file, cancellationToken);
            await file.FlushAsync(cancellationToken);
        }
    }
}
