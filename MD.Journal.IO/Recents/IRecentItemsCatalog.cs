namespace MD.Journal.IO.Recents
{
    public interface IRecentItemsCatalog
    {
        IRecentItems Open(ResourceUri uri);
    }
}
