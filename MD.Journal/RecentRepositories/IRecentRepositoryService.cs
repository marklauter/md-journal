namespace MD.Journal.RecentRepositories;

public interface IRecentRepositoryService
{
    string Path { get; }
    Task<IEnumerable<RecentRepository>> ReadAsync(CancellationToken cancellationToken);
    Task TouchAsync(RecentRepository recentRepository, CancellationToken cancellationToken);
}
