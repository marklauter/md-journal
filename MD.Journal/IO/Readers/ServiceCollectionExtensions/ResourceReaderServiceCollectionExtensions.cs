using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.IO.Readers.ServiceCollectionExtensions
{
    public static class ResourceReaderServiceCollectionExtensions
    {
        public static IServiceCollection AddFileResourceReader(this IServiceCollection services)
        {
            services.TryAddSingleton<IResourceReader, FileResourceReader>();
            return services;
        }

        public static IServiceCollection AddMemoryResourceReader(this IServiceCollection services)
        {
            services.TryAddSingleton<IResourceReader, MemoryResourceReader>();
            return services;
        }
    }
}
