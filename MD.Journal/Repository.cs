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

            await RepositoryHistory
                .AddRecentRepositoryAsync(path);

            return new Journal(path);
        }

        public static void Save(Journal journal)
        {
            var git = new Git(journal.Path);
            git.Stage();
            git.Commit();
            git.Push();
        }
    }
}
