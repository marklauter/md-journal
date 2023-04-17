namespace MD.Journal
{
    public readonly record struct RepositoryHistoryEntry(string Path, DateTime LastAccessUtc);
}
