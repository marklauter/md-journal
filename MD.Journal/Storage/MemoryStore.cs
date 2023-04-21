using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace MD.Journal.Storage
{
    public sealed class MemoryStore
        : Store
    {
        // first string is filename, second string is the content
        private static readonly ConcurrentDictionary<string, IEnumerable<string>> StoredItems = new();

        public MemoryStore(string path, string resourceName)
            : base(path, resourceName)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task OverwriteAllLinesAsync(IEnumerable<string> lines)
        {
            _ = StoredItems.AddOrUpdate(this.Uri, lines, (fileName, lines) => lines);
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task AppendLineAsync(string value)
        {
            _ = StoredItems.AddOrUpdate(this.Uri, new string[] { value }, (fileName, lines) => lines.Append(value));
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<IEnumerable<string>> ReadAllLinesAsync()
        {
            return this.ReadLinesAsync(Pagination.Default);
        }

        public override Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination)
        {
            return Task.FromResult(!StoredItems.ContainsKey(this.Uri)
                ? Enumerable.Empty<string>()
                : !StoredItems.TryGetValue(this.Uri, out var lines)
                    ? Enumerable.Empty<string>()
                    : pagination.Take == Int32.MaxValue && pagination.Skip == 0
                        ? lines.Where(line => line is not null)
                        : lines.Skip(pagination.Skip).Take(pagination.Take));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task AppendTextAsync(string value)
        {
            var lines = value.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                await this.AppendLineAsync(line);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task OverwriteAllTextAsync(string value)
        {
            return this.OverwriteAllLinesAsync(value.Split(Environment.NewLine));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<string> ReadTextAsync()
        {
            return String.Join(Environment.NewLine, await this.ReadAllLinesAsync());
        }
    }
}
