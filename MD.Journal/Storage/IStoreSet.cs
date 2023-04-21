namespace MD.Journal.Storage
{
    public interface IStoreSet
    {
        string Path { get; }
        IStore this[string name] { get; }
        IEnumerable<IStore> Stores { get; }
        bool Contains(string name);
    }
}
