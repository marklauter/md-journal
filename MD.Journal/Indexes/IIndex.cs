using MD.Journal.Storage;

namespace MD.Journal.Indexes
{
    public interface IIndex<TValue>
        where TValue : IComparable<TValue>
    {
        Task<IEnumerable<IndexEntry<TValue>>> FindAsync(string key);
        Task PackAsync();
        Task<IEnumerable<IndexEntry<TValue>>> ReadAsync(Pagination pagination);
        Task WriteAsync(IndexEntry<TValue> entry);
    }
}
