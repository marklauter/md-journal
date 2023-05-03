using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.IO.Pagination
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaginationOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return configuration is null
                ? throw new ArgumentNullException(nameof(configuration))
                : services.Configure<PaginationOptions>(configuration.GetSection(nameof(PaginationOptions)));
        }
    }
}
