using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MD.Journal.IO.Readers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceReader(this IServiceCollection services)
        {
            services.TryAddTransient<IResourceReader, ResourceReader>();
            return services;
        }
    }
}
