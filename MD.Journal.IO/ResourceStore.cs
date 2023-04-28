using System.Collections.Concurrent;

namespace MD.Journal.IO
{
    internal sealed class ResourceStore
        : IResourceStore
    {
        public ConcurrentDictionary<string, IEnumerable<string>> Resources { get; } = new();
    }
}

