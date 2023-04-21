using MD.Journal.Journals;
using MD.Journal.Storage;
using System.Runtime.CompilerServices;

namespace MD.Journal.Tags
{
    internal sealed class TagGraph
    {
        private readonly IStoreSet stores;
        private readonly IStore tagStore;

        public TagGraph(IStoreSet stores)
        {
            this.stores = stores ?? throw new ArgumentNullException(nameof(stores));
            this.tagStore = this.stores["tags.txt"];
        }

        public async Task MapAsync(JournalEntry journalEntry)
        {
            if (journalEntry is null)
            {
                throw new ArgumentNullException(nameof(journalEntry));
            }

            var tags = await this.tagStore.ReadAllLinesAsync();
            foreach (var tag in journalEntry.Tags)
            {
                if (!tags.Contains(tag))
                {
                    await this.tagStore.WriteLineAsync(tag);
                }

                await this.stores[$"{(TagId)tag}.txt"]
                    .WriteLineAsync(journalEntry.Id);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<string>> ReadJournalIdsAsync(string tag, Pagination pagination)
        {
            return String.IsNullOrWhiteSpace(tag)
                ? throw new ArgumentException($"'{nameof(tag)}' cannot be null or whitespace.", nameof(tag))
                : await this.stores[$"{(TagId)tag}.txt"].ReadLinesAsync(pagination);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<IEnumerable<string>> ReadTagsAsync()
        {
            return this.tagStore.ReadAllLinesAsync();
        }
    }
}
