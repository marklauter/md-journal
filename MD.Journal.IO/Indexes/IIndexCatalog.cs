namespace MD.Journal.IO.Indexes
{
    public interface IIndexCatalog
    {
        IIndex<TValue> Open<TValue>(ResourceUri uri) where TValue : IComparable<TValue>;
    }
}
