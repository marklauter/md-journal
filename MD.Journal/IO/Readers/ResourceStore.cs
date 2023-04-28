using System.Collections.Concurrent;

namespace MD.Journal.IO.Readers
{
    internal static class ResourceStore
    {
        public static ConcurrentDictionary<string, IEnumerable<string>> Resources { get; } = new();
    }
}

