using MD.Journal.IO;
using MD.Journal.IO.Recents;

namespace MD.Journal.Journals
{
    public interface IJournalCatalog
    {
        Task<IJournal> OpenJournalAsync(ResourceUri uri);
        Task<IEnumerable<RecentItem>> ReadAsync();
    }
}
