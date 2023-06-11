using System.Text;

namespace MD.Journal.IO.Files;

internal sealed class VirtualFileSystem
    : IFileSystem
{
    private readonly Dictionary<string, byte[]> files = new();

    public void AppendAllLines(string path, IEnumerable<string> lines)
    {
        var contents = this.ReadAllText(path);
        if (contents is not null)
        {
            lines = new string[] { contents }
                .Union(lines);
        }

        this.WriteAllLines(path, lines);
    }

    public Task AppendAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        this.AppendAllLines(path, lines);
        return Task.CompletedTask;
    }

    public void AppendLine(string path, string line)
    {
        this.AppendAllLines(path, new string[] { line });
    }

    public Task AppendLineAsync(string path, string line, CancellationToken cancellationToken)
    {
        this.AppendLine(path, line);
        return Task.CompletedTask;
    }

    public void CreateDirectory(string path)
    {
        // nothing to do
    }

    public bool DirectoryExists(string path)
    {
        return this.files.Any(kvp => kvp.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<string> EnumerateFiles(string path)
    {
        return this.files
            .Keys
            .Where(key => key.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    public bool FileExists(string path)
    {
        return this.files.ContainsKey(path);
    }

    public byte[] ReadAllBytes(string path)
    {
        return this.files.TryGetValue(path, out var bytes)
            ? bytes
            : Array.Empty<byte>();
    }

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.ReadAllBytes(path));
    }

    public string[] ReadAllLines(string path)
    {
        var contents = this.ReadAllText(path);
        return contents is null
            ? Array.Empty<string>()
            : contents.Split(Environment.NewLine);
    }

    public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.ReadAllLines(path));
    }

    public string ReadAllText(string path)
    {
        var contents = this.files.TryGetValue(path, out var bytes)
            ? bytes
            : Array.Empty<byte>();

        return Encoding.UTF8.GetString(contents);
    }

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.ReadAllText(path));
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        this.files[path] = bytes;
    }

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken)
    {
        this.WriteAllBytes(path, bytes);
        return Task.CompletedTask;
    }

    public void WriteAllLines(string path, IEnumerable<string> lines)
    {
        var contents = String.Join(Environment.NewLine, lines);
        this.WriteAllText(path, contents);
    }

    public Task WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        this.WriteAllLines(path, lines);
        return Task.CompletedTask;
    }

    public void WriteAllText(string path, string? contents)
    {
        var bytes = contents is null
            ? Array.Empty<byte>()
            : Encoding.UTF8.GetBytes(contents);

        this.WriteAllBytes(path, bytes);
    }

    public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken)
    {
        this.WriteAllText(path, contents);
        return Task.CompletedTask;
    }
}
