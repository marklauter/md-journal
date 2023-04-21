namespace MD.Journal.Journals
{
    public sealed class JournalEntryBuilder
    {
        private string title = String.Empty;
        private string summary = String.Empty;
        private string author = String.Empty;
        private string body = String.Empty;
        private IEnumerable<string> tags = Enumerable.Empty<string>();

        public static JournalEntryBuilder Create()
        {
            return new JournalEntryBuilder();
        }

        private JournalEntryBuilder() { }

        public JournalEntryBuilder WithTitle(string title)
        {
            if (String.IsNullOrEmpty(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
            }

            this.title = title;
            return this;
        }

        public JournalEntryBuilder WithAuthor(string author)
        {
            if (String.IsNullOrEmpty(author))
            {
                throw new ArgumentException($"'{nameof(author)}' cannot be null or empty.", nameof(author));
            }

            this.author = author;
            return this;
        }

        public JournalEntryBuilder WithBody(string body)
        {
            if (String.IsNullOrEmpty(body))
            {
                throw new ArgumentException($"'{nameof(body)}' cannot be null or empty.", nameof(body));
            }

            this.body = body;
            return this;
        }

        public JournalEntryBuilder WithSummary(string summary)
        {
            if (String.IsNullOrEmpty(summary))
            {
                throw new ArgumentException($"'{nameof(summary)}' cannot be null or empty.", nameof(summary));
            }

            this.summary = summary;
            return this;
        }

        public JournalEntryBuilder WithTag(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or empty.", nameof(tag));
            }

            this.tags = this.tags.Append(tag);
            return this;
        }

        public JournalEntryBuilder WithTags(params string[] tags)
        {
            if (tags is null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            this.tags = this.tags.Union(tags);
            return this;
        }

        public JournalEntry Build()
        {
            return new JournalEntry(
                this.title,
                this.author,
                this.summary,
                this.body,
                this.tags.Order().Distinct().ToArray());
        }
    }
}
