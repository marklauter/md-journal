using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;

namespace MD.Journal.IO.Resources
{
    internal sealed class Resource<T>
        : IResource<T>
    {
        private readonly IResourceReader reader;
        private readonly IResourceWriter writer;

        public Resource(
            ResourceUri uri,
            IResourceReader reader,
            IResourceWriter writer)
        {
            this.Uri = uri;
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public ResourceUri Uri { get; }
    }
}
