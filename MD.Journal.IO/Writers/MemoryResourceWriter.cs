using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Writers
{
    internal sealed class MemoryResourceWriter
        : IResourceWriter
    {
        private readonly IResourceStore resourceStore;
        private readonly ILogger<MemoryResourceWriter> logger;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryResourceWriter(
            IResourceStore resourceStore,
            ILogger<MemoryResourceWriter> logger)
        {
            this.resourceStore = resourceStore;
            this.logger = logger;
        }

        public Task AppendLineAsync(ResourceUri uri, string line)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(AppendLineAsync), (string)uri);

            _ = this.resourceStore
                .Resources
                .AddOrUpdate(uri, new string[] { line }, (uri, lines) => lines.Append(line));

            return Task.CompletedTask;
        }

        public async Task AppendTextAsync(ResourceUri uri, string text)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(AppendTextAsync), (string)uri);

            var newLines = text.Split(Environment.NewLine);

            var initialValue = 0;
            if (this.resourceStore.Resources.TryGetValue(uri, out var lines))
            {
                initialValue = 1;
                var linesArray = lines.ToArray();
                linesArray[^1] += newLines[0];
                await this.OverwriteAllLinesAsync(uri, linesArray);
            }


            for (var i = initialValue; i < newLines.Length; ++i)
            {
                var line = newLines[i];
                await this.AppendLineAsync(uri, line);
            }
        }

        public Task OverwriteAllLinesAsync(ResourceUri uri, IEnumerable<string> lines)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(OverwriteAllLinesAsync), (string)uri);

            _ = this.resourceStore
                .Resources
                .AddOrUpdate(uri, lines, (key, value) => lines);

            return Task.CompletedTask;
        }

        public Task OverwriteAllTextAsync(ResourceUri uri, string text)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(OverwriteAllTextAsync), (string)uri);

            return this.OverwriteAllLinesAsync(uri, text.Split(Environment.NewLine));
        }
    }
}
