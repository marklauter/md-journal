using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.IO.Pagination
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceReader(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.Configure<PaginationOptions>(configuration.GetSection(nameof(PaginationOptions)));
        }
    }
}
