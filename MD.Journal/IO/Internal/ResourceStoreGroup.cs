using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Internal
{
    internal sealed class ResourceStoreGroup<T>
        : IResourceStoreGroup
        where T : ResourceStore
    {
        private readonly Dictionary<string, IDocument> stores = new();

        public ResourceStoreGroup(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            this.Path = path;
        }

        public IDocument this[string name] => this.GetStore(name);

        public string Path { get; }

        public IEnumerable<IDocument> Stores => this.stores.Values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(string name)
        {
            return this.stores.ContainsKey(name);
        }

        private IDocument GetStore(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!this.stores.TryGetValue(name, out var store))
            {
                store = Activator.CreateInstance(typeof(T), this.Path, name) as IDocument;
                this.stores.Add(name, store);
            }

            return store;
        }
    }
}
