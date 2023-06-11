namespace MD.Journal.Templates;

public sealed class StreamContentSource
    : IContentSource
{
    public StreamContentSource(Stream stream)
    {
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true, bufferSize: 1024 * 4, leaveOpen: true);
        this.Contents = reader.ReadToEnd();
    }

    public string Contents { get; }
}
