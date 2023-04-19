namespace MD.Journal
{
    public readonly record struct RecentJournalEntry(string Path, DateTime LastAccessUtc)
    {
        public string LastAccessedDateLocal => this.LastAccessUtc.ToLocalTime().ToShortDateString();
    }
}
