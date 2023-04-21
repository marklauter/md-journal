namespace MD.Journal.Storage
{
    public readonly record struct Pagination(int Skip, int Take)
    {
        public static Pagination Default { get; } = new(0, Int32.MaxValue);
    }
}
