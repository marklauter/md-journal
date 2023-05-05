using MD.Journal.IO;
using MD.Journal.IO.Indexes;
using MD.Journal.IO.Readers;
using MD.Journal.IO.Recents;
using MD.Journal.IO.Writers;
using MD.Journal.Markdown;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Journals
{
    internal sealed class Journal
        : IJournal
    {
        private readonly IResourceReader resourceReader;
        private readonly IResourceWriter resourceWriter;
        private readonly IServiceProvider serviceProvider;
        private readonly IPropertyGraphIndex tags;
        private readonly IRecentItems recentAuthors;
        private readonly IIndex<DateTime> journalEntriesByDate;
        private readonly IIndex<string> journalEntriesByAuthor;

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

            this.tags = this.serviceProvider.GetNamedPropertyGraphIndex(uri, $"{nameof(this.tags)}.pgi");
            this.journalEntriesByDate = this.serviceProvider.GetNamedIndex<DateTime>(uri, $"{nameof(this.journalEntriesByDate)}.index");
            this.journalEntriesByAuthor = this.serviceProvider.GetNamedIndex<string>(uri, $"{nameof(this.journalEntriesByAuthor)}.index");
        }

        public ResourceUri Uri { get; }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<string>> ReadTagsAsync()
        {
            return (await this.tags.ReadPropertiesAsync())
                .Order();
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<JournalEntry>> ReadAsync()
        {
            var entryIds = (await this.journalEntriesByDate.ReadAsync())
                .OrderDescending()
                .Select(item => item.Key);
            return await this.ReadAsync(entryIds);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<JournalEntry> ReadAsync(string journalEntryId)
        {
            var uri = this.Uri.Combine($"{journalEntryId}.json");
            var json = await this.resourceReader.ReadTextAsync(uri);

            return JsonConvert.DeserializeObject<JournalEntry>(json)!;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<JournalEntry>> ReadAsync(IEnumerable<string> journalEntryIds)
        {
            var tasks = new List<Task<JournalEntry>>();
            foreach (var id in journalEntryIds)
            {
                tasks.Add(this.ReadAsync(id));
            }

            return await Task.WhenAll(tasks);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<JournalEntry>> FindAsync(string tag)
        {
            var entryIds = await this.tags.ReadValuesAsync(tag);
            return (await this.ReadAsync(entryIds))
                .OrderByDescending(item => item.Date);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task WriteAsJson(JournalEntry journalEntry)
        {
            if (journalEntry is null)
            {
                throw new ArgumentNullException(nameof(journalEntry));
            }

            var uri = this.Uri
                .Combine($"{journalEntry.Id}.json");
            var json = JsonConvert.SerializeObject(journalEntry, Formatting.Indented);
            return this.resourceWriter.OverwriteAllTextAsync(uri, json);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task WriteAsMarkdownAsync(JournalEntry journalEntry)
        {
            var uri = this.Uri
                .Combine($"{journalEntry.Id}.md");
            var markdown = journalEntry.ToMarkdownString();
            return this.resourceWriter.OverwriteAllTextAsync(uri, markdown);

        }
    }
}


//    public sealed class Journal
//    {
//        private readonly IResource tableOfContents;
//        private readonly TagGraph tagGraph;

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



