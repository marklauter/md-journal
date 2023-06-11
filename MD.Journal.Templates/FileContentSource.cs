namespace MD.Journal.Templates;

public sealed class FileContentSource
    : IContentSource
{
    public FileContentSource(string path)
    {
        using var reader = new StreamReader(path, System.Text.Encoding.UTF8, true, 1024 * 4);
        this.Contents = reader.ReadToEnd();
    }

    public string Contents { get; }
}
