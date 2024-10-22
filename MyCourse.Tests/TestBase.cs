using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Data;
using MyCourse.Domain.Entities;

public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public TestBase()
    {
        var serviceCollection = new ServiceCollection();

        // Initialisiere den DbContext mit einer InMemory-Datenbank
        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        // Initialisiere Identity
        serviceCollection.AddIdentity<User, IdentityRole<int>>()
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<AppDbContext>();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Initialisiere Testdaten
    }

    public void Dispose()
    {
        // Bereinige Ressourcen
        _context.Database.EnsureDeleted();
        _context.Dispose();

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
