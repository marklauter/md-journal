namespace MD.Journal.IO.Recents
{
    public interface IRecentItems
    {
        Task TouchAsync(RecentItem recentItem);
        Task<IEnumerable<RecentItem>> ReadAsync();
    }
}
