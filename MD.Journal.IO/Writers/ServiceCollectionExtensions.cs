using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.IO.Writers
{
    public static class ResourceWriterCollectionExtensions
    {
        public static IServiceCollection AddResourceWriter(this IServiceCollection services)
        {
            services.TryAddSingleton<IResourceWriter, ResourceWriter>();
            return services;
        }
    }
}
