using MD.Journal.Documents;
using MD.Journal.Documents;

namespace MD.Journal.Tests
{
    public sealed class StoreSetTests
    {
        [Fact]
        public void Constructor_Sets_Path()
        {
            var storeSet = new ResourceStoreGroup<MemoryResourceStore>(nameof(Constructor_Sets_Path));
            Assert.Equal(nameof(Constructor_Sets_Path), storeSet.Path);
        }

        [Fact]
        public void Indexer_Returns_Store()
        {
            var storeSet = new ResourceStoreGroup<MemoryResourceStore>(nameof(Indexer_Returns_Store));
            var store = storeSet[nameof(Indexer_Returns_Store)];
            Assert.NotNull(store);
            Assert.Equal(typeof(MemoryResourceStore), store.GetType());
        }

        [Fact]
        public void Indexer_Returns_Same_Store_Twice()
        {
            var storeSet = new ResourceStoreGroup<MemoryResourceStore>(nameof(Indexer_Returns_Same_Store_Twice));
            var store1 = storeSet[nameof(Indexer_Returns_Same_Store_Twice)];
            var store2 = storeSet[nameof(Indexer_Returns_Same_Store_Twice)];
            Assert.Equal(store1, store2);
        }

        [Fact]
        public void Contains_Returns_True_When_Key_Present()
        {
            var storeSet = new ResourceStoreGroup<MemoryResourceStore>(nameof(Contains_Returns_True_When_Key_Present));
            var store = storeSet[nameof(Contains_Returns_True_When_Key_Present)];
            Assert.True(storeSet.Contains(nameof(Contains_Returns_True_When_Key_Present)));
        }

        [Fact]
        public void Contains_Returns_False_When_Key_NotPresent()
        {
            var storeSet = new ResourceStoreGroup<MemoryResourceStore>(nameof(Contains_Returns_False_When_Key_NotPresent));
            var store = storeSet[nameof(Contains_Returns_False_When_Key_NotPresent)];
            Assert.False(storeSet.Contains("not-found"));
        }

        [Fact]
        public void Stores_Returns_All_Stores()
        {
            var storeSet = new ResourceStoreGroup<MemoryResourceStore>(nameof(Indexer_Returns_Same_Store_Twice));
            var store1 = storeSet["one"];
            var store2 = storeSet["two"];
            Assert.NotEqual(store1, store2);
            var stores = storeSet.Stores;
            Assert.Equal(2, stores.Count());
            Assert.Contains(store1, stores);
            Assert.Contains(store2, stores);
        }
    }
}
