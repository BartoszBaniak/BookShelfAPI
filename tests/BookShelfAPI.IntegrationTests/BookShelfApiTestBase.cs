using BookShelfAPI.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelfAPI.IntegrationTests;

public abstract class BookShelfApiTestBase : IDisposable
{
    protected readonly BookShelfApiFactory Factory;
    protected readonly HttpClient Client;

    protected BookShelfApiTestBase()
    {
        Factory = new BookShelfApiFactory();
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
