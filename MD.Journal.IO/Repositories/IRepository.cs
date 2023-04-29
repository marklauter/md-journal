using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.IO.Repositories
{
    public interface IResourceCatalog
    {
        IResource Open(ResourceUri uri);
    }

    public interface IResource
    {

    }

    internal sealed class Resource
        : IResource
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

    public interface IRepository
    {
        IResource Open(ResourceUri uri);
    }

    internal sealed class Repository
        : IRepository
    {
        private readonly IServiceProvider serviceProvider;

        public Repository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IResource Open(ResourceUri uri)
        {
            var reader = this.serviceProvider.GetRequiredService<IResourceReader>();
            var writer = this.serviceProvider.GetRequiredService<IResourceWriter>();
            return new Resource(uri, reader, writer);
        }
    }
}
