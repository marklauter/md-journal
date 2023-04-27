using System.Collections.Concurrent;

namespace MD.Journal.IO.Internal
{
    internal static class DocumentStore
    {
        // first string is document uri, string collection is the content
        public static ConcurrentDictionary<string, IEnumerable<string>> Documents { get; } = new();
    }
}

