namespace MD.Journal
{
    public class GitRepository
    {
        public Journal Open(string path)
        {
            return String.IsNullOrWhiteSpace(path)
                ? throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path))
                : !GitProcess.IsGitRepository(path)
                    ? throw new InvalidOperationException("path must contain git repository")
                    : new Journal(path);
        }

        public void Commit(Journal journal)
        {
            var git = new GitProcess(journal.Path);
            _ = git.Stage();
            _ = git.Commit();
            _ = git.Push();
        }
    }
}
