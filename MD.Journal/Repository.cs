namespace MD.Journal
{
    public static class Repository
    {
        public static async Task<Journal> OpenAsync(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            await new RepositoryHistory()
                .AddRecentRepositoryAsync(path);

            return new Journal(path);
        }
    }
}
