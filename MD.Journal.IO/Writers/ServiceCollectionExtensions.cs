using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.IO.Writers
{
    public static class ResourceIOServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceWriter(this IServiceCollection services)
        {
            services.TryAddTransient<IResourceWriter, ResourceWriter>();
            return services;
        }
    }
}
