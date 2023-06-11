using MD.Journal.IO.Files;
using MD.Journal.RecentRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.Tests;

public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        _ = services
            .AddVirtualFileSystemAsScoped()
            .AddRecentRepositoryService();
    }
}
