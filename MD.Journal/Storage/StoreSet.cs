using System.Runtime.CompilerServices;

namespace MD.Journal.Storage
{
    public sealed class StoreSet<T>
        : IStoreSet
        where T : Store
    {
        private readonly Dictionary<string, IStore> stores = new();

        public StoreSet(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            this.Path = path;
        }

        public IStore this[string name] => this.GetStore(name);

        public string Path { get; }

        public IEnumerable<IStore> Stores => this.stores.Values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(string name)
        {
            return this.stores.ContainsKey(name);
        }

        private IStore GetStore(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!this.stores.TryGetValue(name, out var store))
            {
                store = Activator.CreateInstance(typeof(T), this.Path, name) as IStore;
#pragma warning disable CS8604 // Possible null reference argument.
                this.stores.Add(name, store);
#pragma warning restore CS8604 // Possible null reference argument.
            }

            return store;
        }
    }
}
