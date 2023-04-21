namespace MD.Journal.Recents
{
    public interface IRecentItems
    {
        Task TouchAsync(RecentItem recentItem);
        Task<IEnumerable<RecentItem>> ReadAsync();
    }
}
