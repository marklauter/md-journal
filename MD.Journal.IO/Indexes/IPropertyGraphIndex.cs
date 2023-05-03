namespace MD.Journal.IO.Indexes
{
    public interface IPropertyGraphIndex
    {
        Task MapAsync(string key, string value);
        Task MapAsync(IEnumerable<string> keys, string value);
        Task<IEnumerable<string>> ReadValuesAsync(string key);
        Task<IEnumerable<string>> ReadPropertiesAsync();
    }
}
