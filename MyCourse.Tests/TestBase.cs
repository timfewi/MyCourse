using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Data;
using MyCourse.Domain.Entities;

public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    protected readonly IServiceCollection _services;

    public TestBase()
    {
        _services = new ServiceCollection();

        // Initialisiere den DbContext mit einer InMemory-Datenbank
        _services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        // Initialisiere Identity
        _services.AddIdentity<User, IdentityRole<int>>()
              .AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

        // Ermögliche abgeleiteten Klassen, zusätzliche Dienste zu konfigurieren
        ConfigureServices(_services);

        _serviceProvider = _services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<AppDbContext>();

        SeedDatabase();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Kann von abgeleiteten Klassen überschrieben werden.
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
