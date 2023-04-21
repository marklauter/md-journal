using System.Runtime.CompilerServices;

namespace MD.Journal.Storage
{
    public sealed class FileStore
        : Store
    {
        public FileStore(string path, string fileName)
            : base(path, fileName)
        {
            _ = Directory.CreateDirectory(this.Path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task OverWriteAllLinesAsync(IEnumerable<string> lines)
        {
            return File.WriteAllLinesAsync(this.Uri, lines);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task OverWriteAllTextAsync(string value)
        {
            return File.WriteAllTextAsync(this.Uri, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task WriteLineAsync(string value)
        {
            return File.AppendAllLinesAsync(this.Uri, new string[] { value });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<IEnumerable<string>> ReadAllLinesAsync()
        {
            return this.ReadLinesAsync(Pagination.Default);
        }

        public override async Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination)
        {
            if (!File.Exists(this.Uri))
            {
                return Enumerable.Empty<string>();
            }

            using var stream = File.OpenRead(this.Uri);
            using var reader = new StreamReader(stream);

            if (pagination.Take == Int32.MaxValue && pagination.Skip == 0)
            {
                return (await File.ReadAllLinesAsync(this.Uri))
                    .Where(line => line is not null);
            }

            var linenumber = 0;
            var start = pagination.Skip;
            var end = pagination.Skip + pagination.Take;
            // warning: pagination.Take could be Int32.MaxValue, so resist the temptation to make values an array of size pagination.Take
            var values = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (linenumber >= start && linenumber < end && line is not null)
                {
                    values.Add(line);
                }

                ++linenumber;
            }

            return values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task WriteTextAsync(string value)
        {
            return File.AppendAllTextAsync(this.Uri, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<string> ReadTextAsync()
        {
            return File.ReadAllTextAsync(this.Uri);
        }
    }
}
