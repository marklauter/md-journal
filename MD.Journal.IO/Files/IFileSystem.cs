namespace MD.Journal.IO.Files;

public interface IFileSystem
{
    void CreateDirectory(string path);
    IEnumerable<string> EnumerateFiles(string path);

    bool DirectoryExists(string path);
    bool FileExists(string path);

    byte[] ReadAllBytes(string path);
    string ReadAllText(string path);
    string[] ReadAllLines(string path);

    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken);
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken);
    Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken);

    void WriteAllText(string path, string? contents);
    void WriteAllBytes(string path, byte[] bytes);
    void WriteAllLines(string path, IEnumerable<string> lines);
    void AppendAllLines(string path, IEnumerable<string> lines);
    void AppendLine(string path, string line);

    Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken);
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken);
    Task WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken);
    Task AppendAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken);
    Task AppendLineAsync(string path, string line, CancellationToken cancellationToken);
}
