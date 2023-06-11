using MD.Journal.IO.Files;
using MD.Journal.RecentRepositories;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.Tests.RecentRepositories;

public class RecentRepositorServiceTests
{
    private readonly IServiceProvider serviceProvider;

    public RecentRepositorServiceTests(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [Fact]
    public void ServiceProvider_GetRecentRepositories_Returns_RecentRepositories()
    {
        var recentRepositories = this.serviceProvider.GetRecentRepositoryService("path");
        Assert.NotNull(recentRepositories);
    }

    [Fact]
    public async Task TouchAsync_Creates_File()
    {
        var path = "path";
        var time = DateTime.UtcNow;

        using var scope = this.serviceProvider.CreateScope();
        var fileSystem = scope.ServiceProvider.GetRequiredService<IFileSystem>();
        var service = scope.ServiceProvider.GetRecentRepositoryService(path);

        await service.TouchAsync(new RecentRepository(path, time), CancellationToken.None);

        Assert.True(fileSystem.FileExists(service.Path));
    }

    [Fact]
    public async Task TouchAsync_Correctly_Orders_Recent_Items()
    {
        var path1 = "one";
        var path2 = "two";
        var path3 = "three";
        var time1 = DateTime.UtcNow;
        var time2 = time1.AddMinutes(1);
        var time3 = time2.AddMinutes(1);

        using var scope = this.serviceProvider.CreateScope();
        var fileSystem = scope.ServiceProvider.GetRequiredService<IFileSystem>();
        var service = scope.ServiceProvider.GetRecentRepositoryService("path");

        await service.TouchAsync(new RecentRepository(path1, time1), CancellationToken.None);
        await service.TouchAsync(new RecentRepository(path2, time2), CancellationToken.None);
        await service.TouchAsync(new RecentRepository(path3, time3), CancellationToken.None);

        var recentRepositories = await service.ReadAsync(CancellationToken.None);
        Assert.NotEmpty(recentRepositories);
        Assert.Equal(3, recentRepositories.Count());
        Assert.True(recentRepositories.First().Path == path3);
        Assert.True(recentRepositories.Last().Path == path1);
    }
}
