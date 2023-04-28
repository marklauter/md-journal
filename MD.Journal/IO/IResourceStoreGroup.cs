namespace MD.Journal.IO
{
    public interface IResourceStoreGroup
    {
        string Path { get; }
        IResource this[string name] { get; }
        IEnumerable<IResource> Stores { get; }
        bool Contains(string name);
    }
}
