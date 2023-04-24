using Newtonsoft.Json;

namespace MD.Journal.Journals
{
    public sealed class JournalEntry
    {
        private static readonly string ParagraphSeparator = Environment.NewLine + Environment.NewLine;

        public JournalEntry(
            string title,
            string author,
            string summary,
            string rawContent,
            string[] tags)
            : this(String.Empty, title, author, summary, rawContent, tags, DateTime.UtcNow)
        {
        }

        [JsonConstructor]
        public JournalEntry(
            string id,
            string title,
            string author,
            string summary,
            string rawContent,
            string[] tags,
            DateTime date)
        {
            this.Date = date;
            this.Title = title ?? throw new ArgumentNullException(nameof(title));
            this.Author = author ?? throw new ArgumentNullException(nameof(author));
            this.Summary = summary ?? String.Empty;
            this.Tags = tags is not null
                ? tags
                : Array.Empty<string>();

            var idDate = date.ToString("s").Replace(':', '-');
            var idTitle = title.Replace("  ", " ").Replace(' ', '-');
            this.Name = $"{idDate}.{idTitle}";
            this.Id = String.IsNullOrEmpty(id)
                ? $"journal-entry.{this.Name}"
                : id;

            this.RawContent = rawContent ?? throw new ArgumentNullException(nameof(rawContent));
            this.Paragraphs = rawContent.Split(ParagraphSeparator, StringSplitOptions.None);
        }

        public string Name { get; }
        public string Id { get; }
        public string Title { get; }
        public string Author { get; }
        public string Summary { get; }
        public DateTime Date { get; }
        public string[] Tags { get; }
        public string RawContent { get; }

        [JsonIgnore]
        public string[] Paragraphs { get; }

        [JsonIgnore]
        public string ByLine => $"by {this.Author} {this.Date.ToLocalTime().ToShortDateString()}";
    }
}
