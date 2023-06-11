namespace MD.Journal.IO.Files;

internal sealed class PhysicalFileSystem
    : IFileSystem
{
    public void AppendAllLines(string path, IEnumerable<string> lines)
    {
        File.AppendAllLines(path, lines);
    }

    public Task AppendAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        return File.AppendAllLinesAsync(path, lines, cancellationToken);
    }

    public void AppendLine(string path, string line)
    {
        File.AppendAllLines(path, new string[] { line });
    }

    public Task AppendLineAsync(string path, string line, CancellationToken cancellationToken)
    {
        return File.AppendAllLinesAsync(path, new string[] { line }, cancellationToken);
    }

    public void CreateDirectory(string path)
    {
        _ = Directory.CreateDirectory(path);
    }

    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
    {
        return File.ReadAllBytesAsync(path, cancellationToken);
    }

    public string[] ReadAllLines(string path)
    {
        return File.ReadAllLines(path);
    }

    public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken)
    {
        return File.ReadAllLinesAsync(path, cancellationToken);
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        return File.ReadAllTextAsync(path, cancellationToken);
    }

    public IEnumerable<string> EnumerateFiles(string path)
    {
        return Directory.EnumerateFiles(path);
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken)
    {
        return File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    public void WriteAllLines(string path, IEnumerable<string> lines)
    {
        File.WriteAllLines(path, lines);
    }

    public Task WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        return File.WriteAllLinesAsync(path, lines, cancellationToken);
    }

    public void WriteAllText(string path, string? contents)
    {
        File.WriteAllText(path, contents);
    }

    public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken)
    {
        return File.WriteAllTextAsync(path, contents, cancellationToken);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }
}
