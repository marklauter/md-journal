using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.IO.Resources
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResource<T>(
            this IServiceCollection services)
        {
            _ = services
                .AddResourceReader()
                .AddResourceWriter();

            return services.AddTransient<Func<ResourceUri, IResource<T>>>(
                serviceProvider =>
                (uri) => serviceProvider.CreateNamedResource<T>(uri));
        }

        private static IResource<T> CreateNamedResource<T>(
            this IServiceProvider serviceProvider,
            ResourceUri uri)
        {
            var reader = serviceProvider.GetRequiredService<IResourceReader>();
            var writer = serviceProvider.GetRequiredService<IResourceWriter>();
            return new Resource<T>(uri, reader, writer);
        }
    }
}
