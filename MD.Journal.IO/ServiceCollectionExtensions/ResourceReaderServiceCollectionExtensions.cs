using MD.Journal.IO.Readers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.IO.ServiceCollectionExtensions
{
    public static class ResourceReaderServiceCollectionExtensions
    {
        public static IServiceCollection AddFileResourceReader(this IServiceCollection services)
        {
            services.TryAddTransient<IResourceReader, FileResourceReader>();
            return services;
        }

        public static IServiceCollection AddMemoryResourceReader(this IServiceCollection services)
        {
            services.TryAddTransient<IResourceReader, MemoryResourceReader>();
            return services;
        }

        public static IServiceCollection AddResourceStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IResourceStore, ResourceStore>();
            return services;
        }
    }
}
