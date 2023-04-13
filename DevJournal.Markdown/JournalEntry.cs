namespace DevJournal.Markdown
{
    internal class JournalEntry
    {
        public JournalEntry(string title, string author, string rawContent, string[] tags)
            : this(title, author, rawContent, tags, DateTime.UtcNow)
        {
        }

        public JournalEntry(string title, string author, string rawContent, string[] tags, DateTime date)
        {
            this.Title = title ?? throw new ArgumentNullException(nameof(title));
            this.Author = author ?? throw new ArgumentNullException(nameof(author));
            this.RawContent = rawContent ?? throw new ArgumentNullException(nameof(rawContent));
            this.Tags = tags ?? throw new ArgumentNullException(nameof(tags));
            this.Date = date;
            // 2009-06-15T13:45:30
            var idDate = date.ToString("s").Replace(':', '-');
            var idTitle = title.Replace("  ", " ").Replace(' ', '-');
            this.Id = $"{idDate}.{idTitle}";
            this.Paragraphs = rawContent.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.None);
        }

        public string Id { get; }
        public string Title { get; }
        public string Author { get; }
        public string RawContent { get; }
        public DateTime Date { get; }
        public string[] Tags { get; }
        public string[] Paragraphs { get; }
    }
}
