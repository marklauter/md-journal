using MD.Journal.IO;
using MD.Journal.IO.Indexes;
using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Journals
{
    public sealed class Journal
        : IJournal
    {
        private readonly IResourceReader resourceReader;
        private readonly IResourceWriter resourceWriter;
        private readonly IServiceProvider serviceProvider;
        private readonly IPropertyGraphIndex tagPGI;
        private readonly IIndex<DateTime> journalEntriesByDateIndex;
        private readonly IIndex<string> journalEntriesByAuthorIndex;

        public Journal(
            ResourceUri uri,
            IResourceReader resourceReader,
            IResourceWriter resourceWriter,
            IServiceProvider serviceProvider)
        {
            this.Uri = uri;
            this.resourceReader = resourceReader ?? throw new ArgumentNullException(nameof(resourceReader));
            this.resourceWriter = resourceWriter ?? throw new ArgumentNullException(nameof(resourceWriter));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            this.tagPGI = this.serviceProvider.GetNamedPropertyGraphIndex(uri, "tags.pgi");
            this.journalEntriesByDateIndex = this.serviceProvider.GetNamedIndex<DateTime>(uri, "journalEntriesByDate.index");
            this.journalEntriesByAuthorIndex = this.serviceProvider.GetNamedIndex<string>(uri, "journalEntriesByAuthor.index");
        }

        public ResourceUri Uri { get; }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<string>> ReadTagsAsync()
        {
            return (await this.tagPGI.ReadPropertiesAsync())
                .Order();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<JournalEntry>> ReadAsync()
        {
            var entryIds = (await this.journalEntriesByDateIndex.ReadAsync())
                .Select(item => item.Key);
            return await this.ReadAsync(entryIds);
        }

        public async Task<JournalEntry> ReadAsync(string journalEntryId)
        {
            var uri = this.Uri.WithPath($"{journalEntryId}.json");
            var json = await this.resourceReader.ReadTextAsync(uri);

            return JsonConvert.DeserializeObject<JournalEntry>(json)!;
        }

        public async Task<IEnumerable<JournalEntry>> ReadAsync(IEnumerable<string> journalEntryIds)
        {
            var tasks = new List<Task<JournalEntry?>>();
            foreach (var id in journalEntryIds)
            {
                tasks.Add(this.ReadAsync(id));
            }

            return await Task.WhenAll(tasks);
        }
    }
}

//    public sealed class Journal
//    {
//        private readonly IIndexCatalog indexCatalog;
//        private readonly IRecentItemsCatalog recentItemsCatalog;
//        private readonly IRepositoryCatalog repositoryCatalog;
//        private readonly IRecentItems recentAuthors;

//        private Journal(
//            string path,
//            IIndexCatalog indexCatalog,
//            IRecentItemsCatalog recentItemsCatalog,
//            IRepositoryCatalog repositoryCatalog)
//        {
//            if (String.IsNullOrEmpty(path))
//            {
//                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
//            }

//            this.Path = path;
//            this.indexCatalog = indexCatalog ?? throw new ArgumentNullException(nameof(indexCatalog));
//            this.recentItemsCatalog = recentItemsCatalog ?? throw new ArgumentNullException(nameof(recentItemsCatalog));
//            this.repositoryCatalog = repositoryCatalog ?? throw new ArgumentNullException(nameof(repositoryCatalog));

//            this.recentAuthors = this.recentItemsCatalog.Open("recent-authors.json");
//        }

//        public string Path { get; }
//    }

//    public sealed class Journal
//    {
//        public static Journal Open<TStore>(string path)
//            where TStore : MemoryResourceStore
//        {
//            return new Journal(path, new ResourceStoreGroup<TStore>(path));
//        }

//        private readonly IResourceStoreGroup stores;
//        private readonly IRecentItems recentAuthors;
//        private readonly IIndex<DateTime> journalByDateIndex;
//        private readonly IIndex<string> journalByAuthorIndex;
//        private readonly IResource tableOfContents;
//        private readonly TagGraph tagGraph;

//        public string Path { get; }
//        public string Name { get; }

//        private Journal(string path, IResourceStoreGroup stores)
//        {
//            if (String.IsNullOrWhiteSpace(path))
//            {
//                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
//            }

//            this.Path = path;
//            this.Name = System.IO.Path.GetFileName(path);
//            this.stores = stores;
//            this.tagGraph = new TagGraph(stores);
//            this.recentAuthors = new RecentItems(stores["recent-authors.json"], Options.Create(new RecentItemsOptions()));
//            this.journalByDateIndex = new Index<DateTime>(stores["journal-date-index.json"]);
//            this.journalByAuthorIndex = new Index<string>(stores["author-journal-index.json"]);
//            this.tableOfContents = stores["toc.md"];
//        }

//        public async Task<JournalEntry?> ReadAsync(string journalEntryId)
//        {
//            var lines = await this.stores[$"{journalEntryId}.json"].ReadAllLinesAsync();
//            return lines.Any()
//                ? JsonConvert.DeserializeObject<JournalEntry>(String.Join(Environment.NewLine, lines))
//                : null;
//        }

//        public async Task<IEnumerable<JournalEntry?>> ReadAsync(Pagination pagination)
//        {
//            var journalEntryIds = (await this.journalByDateIndex.ReadAsync(pagination))
//                .Select(entry => entry.Key);
//            return await this.ReadAsync(journalEntryIds);
//        }

//        public async Task<IEnumerable<JournalEntry?>> ReadAsync(IEnumerable<string> journalEntryIds)
//        {
//            var tasks = new List<Task<JournalEntry?>>();
//            foreach (var id in journalEntryIds)
//            {
//                tasks.Add(this.ReadAsync(id));
//            }

//            return await Task.WhenAll(tasks);
//        }

//        public async Task<IEnumerable<JournalEntry?>> FindAsync(string tag, Pagination pagination)
//        {
//            var journalEntryIds = await this.tagGraph
//                .ReadJournalIdsAsync(tag, pagination);
//            return await this.ReadAsync(journalEntryIds);
//        }

//        public Task WriteAsync(
//            JournalEntry journalEntry)
//        {
//            return Task.WhenAll(
//                this.WriteAsJson(journalEntry),
//                this.WriteAsMarkdownAsync(journalEntry),
//                this.tagGraph.MapAsync(journalEntry),
//                this.journalByDateIndex.WriteAsync(new IndexEntry<DateTime>(journalEntry.Id, journalEntry.Date)),
//                this.recentAuthors.TouchAsync(new RecentItem(journalEntry.Author, journalEntry.Date)),
//                this.journalByAuthorIndex.WriteAsync(new IndexEntry<string>(journalEntry.Author, journalEntry.Id)),
//                this.WriteTocAsync(journalEntry));
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private Task WriteTocAsync(JournalEntry journalEntry)
//        {
//            // todo: create a toc class that can be converted to markdown with urls and then add that here instead of just the id
//            return this.tableOfContents.AppendLineAsync(journalEntry.Id);
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private Task WriteAsJson(JournalEntry journalEntry)
//        {
//            return this.stores[$"{journalEntry.Id}.json"].OverwriteAllTextAsync(JsonConvert.SerializeObject(journalEntry, Formatting.Indented));
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private Task WriteAsMarkdownAsync(JournalEntry journalEntry)
//        {
//            return this.stores[$"{journalEntry.Id}.md"].OverwriteAllTextAsync(journalEntry.ToMarkdownString());
//        }


//    }



