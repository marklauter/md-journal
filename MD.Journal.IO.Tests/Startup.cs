using MD.Journal.IO.Pagination;
using MD.Journal.IO.ServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Tests
{
    public sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddResourceStore()
                .TryAddTransient(serviceProvider => Options.Create(new PaginationOptions { PageSize = 5 }));
        }
    }
}
