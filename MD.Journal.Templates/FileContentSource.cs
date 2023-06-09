namespace MD.Journal.Templates;

public sealed class FileContentSource
    : IContentSource
{
    public FileContentSource(string path)
    {
        using var reader = new StreamReader(path, System.Text.Encoding.UTF8, true, 1024 * 4);
        this.Content = reader.ReadToEnd();
    }

    public string Content { get; }
}
