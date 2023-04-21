namespace MD.Journal.Storage
{
    public abstract class Store
        : IStore
    {
        public Store(string path, string name)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            this.Uri = System.IO.Path.Combine(path, name);
            this.Path = path;
            this.Name = name;
        }

        public string Path { get; }
        public string Name { get; }
        public string Uri { get; }

        public abstract Task OverWriteAllLinesAsync(IEnumerable<string> lines);
        public abstract Task OverWriteAllTextAsync(string value);
        public abstract Task<IEnumerable<string>> ReadAllLinesAsync();
        public abstract Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination);
        public abstract Task<string> ReadTextAsync();
        public abstract Task WriteLineAsync(string value);
        public abstract Task WriteTextAsync(string value);
    }
}
