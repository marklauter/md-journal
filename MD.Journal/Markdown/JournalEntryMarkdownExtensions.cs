using Grynwald.MarkdownGenerator;
using MD.Journal.Journals;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Markdown
{
    public static class JournalEntryMarkdownExtensions
    {
        [Pure]
        public static MdDocument ToMarkdownResource(this JournalEntry entry)
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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemoryStream ToMarkdownStream(this JournalEntry entry)
        {
            var stream = new MemoryStream();
            entry.ToMarkdownResource().Save(stream);
            stream.Position = 0;

            return stream;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToMarkdownString(this JournalEntry entry)
        {
            return entry.ToMarkdownResource().ToString();
        }
    }
}
