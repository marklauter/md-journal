using Microsoft.Extensions.Logging;

namespace MD.Journal.IO.Writers
{
    internal sealed class ResourceWriter
        : IResourceWriter
    {
        private readonly ILogger<ResourceWriter> logger;

        public ResourceWriter(ILogger<ResourceWriter> logger)
        {
            this.logger = logger;
        }

        public Task AppendLineAsync(ResourceUri uri, string line)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(AppendLineAsync), (string)uri);
            return File.AppendAllLinesAsync(uri, new string[] { line });
        }

        public Task AppendTextAsync(ResourceUri uri, string text)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(AppendTextAsync), (string)uri);
            return File.AppendAllTextAsync(uri, text);
        }

        public Task OverwriteAllLinesAsync(ResourceUri uri, IEnumerable<string> lines)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(OverwriteAllLinesAsync), (string)uri);
            return File.WriteAllLinesAsync(uri, lines);
        }

        public Task OverwriteAllTextAsync(ResourceUri uri, string text)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(OverwriteAllTextAsync), (string)uri);
            return File.WriteAllTextAsync(uri, text);
        }
    }
}
