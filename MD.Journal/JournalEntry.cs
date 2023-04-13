namespace MD.Journal
{
    public sealed class JournalEntry
    {
        private static readonly string ParagraphSeparator = Environment.NewLine + Environment.NewLine;

        public JournalEntry(
            string title,
            string author,
            string summary,
            string rawBody,
            IEnumerable<string> tags)
            : this(String.Empty, title, author, summary, rawBody, tags, DateTime.UtcNow)
        {
        }

        public JournalEntry(
            string id,
            string title,
            string author,
            string summary,
            string rawBody,
            IEnumerable<string> tags,
            DateTime date)
        {
            this.Date = date;
            this.Title = title ?? throw new ArgumentNullException(nameof(title));
            this.Author = author ?? throw new ArgumentNullException(nameof(author));
            this.Summary = summary ?? String.Empty;
            this.Tags = tags is not null
                ? tags.Order().Distinct()
                : Enumerable.Empty<string>();

            var idDate = date.ToString("s").Replace(':', '-');
            var idTitle = title.Replace("  ", " ").Replace(' ', '-');
            this.Name = $"{idDate}.{idTitle}";
            this.Id = String.IsNullOrEmpty(id)
                ? $"journal-entry.{this.Name}.{Guid.NewGuid()}"
                : id;

            this.RawContent = rawBody ?? throw new ArgumentNullException(nameof(rawBody));
            this.Paragraphs = rawBody.Split(ParagraphSeparator, StringSplitOptions.None);
        }

        public string Name { get; }
        public string Id { get; }
        public string Title { get; }
        public string Author { get; }
        public string Summary { get; }
        public DateTime Date { get; }
        public IEnumerable<string> Tags { get; }
        public string RawContent { get; }
        public string[] Paragraphs { get; }
    }
}
