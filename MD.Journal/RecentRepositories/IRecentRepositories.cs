using MD.Journal.IO.Recents;

namespace MD.Journal.RecentRepositories;

public interface IRecentRepositories
{
    Task<IEnumerable<RecentRepository>> ReadAsync(CancellationToken cancellationToken);
    Task TouchAsync(RecentRepository recentRepository, CancellationToken cancellationToken);
}
