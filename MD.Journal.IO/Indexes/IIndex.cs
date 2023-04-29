namespace MD.Journal.IO.Indexes
{
    public interface IIndex<TValue>
        where TValue : IComparable<TValue>
    {
        Task PackAsync();
        Task<IEnumerable<IndexEntry<TValue>>> ReadAsync(string key);
        Task WriteAsync(IndexEntry<TValue> entry);
    }
}
