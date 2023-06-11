using MD.Journal.IO.Files;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.RecentRepositories;

public sealed class RecentRepositoryService
    : IRecentRepositoryService
{
    private const int EntryLimit = 20;
    private readonly IFileSystem fileSystem;

    //Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    public RecentRepositoryService(
        string rootPath,
        IFileSystem fileSystem)
    {
        if (String.IsNullOrEmpty(rootPath))
        {
            throw new ArgumentException($"'{nameof(rootPath)}' cannot be null or empty.", nameof(rootPath));
        }

        this.Path = System.IO.Path.Combine(
            rootPath,
            "MD.Journal",
            "RecentRepositories.json");

        this.fileSystem = fileSystem;
        fileSystem.CreateDirectory(this.Path);
    }

    public string Path { get; }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<IEnumerable<RecentRepository>> ReadAsync(CancellationToken cancellationToken)
    {
        return this.fileSystem.FileExists(this.Path)
            ? (await this.fileSystem.ReadAllLinesAsync(this.Path, cancellationToken))
                .Where(line => !String.IsNullOrWhiteSpace(line))
                .Select(line => (RecentRepository)line)
                .OrderByDescending(item => item.LastAccessUtc)
            : Enumerable.Empty<RecentRepository>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task TouchAsync(RecentRepository recentRepository, CancellationToken cancellationToken)
    {
        var items = (await this.ReadAsync(cancellationToken)).ToArray();

        // replace the entry if found
        var found = false;
        for (var i = 0; i < items.Length; ++i)
        {
            if (items[i].CompareTo(recentRepository) == 0)
            {
                items[i] = recentRepository;
                found = true;
                break;
            }
        }

        if (!found)
        {
            // replace the oldest entry if not found and list is full or append a new entry
            if (items.Length == EntryLimit)
            {
                items[^1] = recentRepository;
            }
            else
            {
                items = items
                    .Append(recentRepository)
                    .ToArray();
            }
        }

        await this.fileSystem.WriteAllLinesAsync(
            this.Path,
            items.Select(item => (string)item),
            cancellationToken);
    }
}

