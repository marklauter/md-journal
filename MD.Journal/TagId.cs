namespace MD.Journal
{
    public readonly struct TagId
    {
        private readonly string value;

        private TagId(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or empty.", nameof(tag));
            }

            this.value = tag
                .ToLowerInvariant()
                .Replace(" ", String.Empty);
        }

        public override string ToString()
        {
            return this.value;
        }

        public static explicit operator string(TagId tagId)
        {
            return tagId.value;
        }

        public static explicit operator TagId(string tag)
        {
            return new TagId(tag);
        }
    }
}
