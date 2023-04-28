using System.Collections.Concurrent;

namespace MD.Journal.IO
{
    public interface IResourceStore
    {
        ConcurrentDictionary<string, IEnumerable<string>> Resources { get; }
    }
}

