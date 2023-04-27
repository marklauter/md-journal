namespace MD.Journal.IO
{
    public interface IResourceStoreGroup
    {
        string Path { get; }
        IDocument this[string name] { get; }
        IEnumerable<IDocument> Stores { get; }
        bool Contains(string name);
    }
}
