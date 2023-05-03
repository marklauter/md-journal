using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Writers
{
    internal sealed class ResourceWriter
        : IResourceWriter
    {
        private readonly ILogger<ResourceWriter> logger;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ResourceWriter(ILogger<ResourceWriter> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task AppendLineAsync(ResourceUri uri, string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                throw new ArgumentException($"'{nameof(line)}' cannot be null or empty.", nameof(line));
            }

            this.logger.LogInformation("{MethodName}({Uri})", nameof(AppendLineAsync), (string)uri);
            return File.AppendAllLinesAsync(uri, new string[] { line });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task AppendTextAsync(ResourceUri uri, string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            this.logger.LogInformation("{MethodName}({Uri})", nameof(AppendTextAsync), (string)uri);
            return File.AppendAllTextAsync(uri, text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task OverwriteAllLinesAsync(ResourceUri uri, IEnumerable<string> lines)
        {
            if (lines is null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            this.logger.LogInformation("{MethodName}({Uri})", nameof(OverwriteAllLinesAsync), (string)uri);
            return File.WriteAllLinesAsync(uri, lines);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task OverwriteAllTextAsync(ResourceUri uri, string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            this.logger.LogInformation("{MethodName}({Uri})", nameof(OverwriteAllTextAsync), (string)uri);
            return File.WriteAllTextAsync(uri, text);
        }
    }
}
