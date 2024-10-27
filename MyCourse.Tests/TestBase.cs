using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Data;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Extensions;
using MyCourse.Domain.Validation.EntityValidations;
using System.Reflection;

public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext _context;
    protected readonly IServiceProvider _serviceProvider;
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

        // AutoMapper
        _services.AddAutoMapper(Assembly.GetAssembly(typeof(TestBase)));

        // Füge FluentValidation hinzu
        _services.AddFluentValidationAutoValidation();
        _services.AddFluentValidationClientsideAdapters();

        // Registriere Validatoren aus dem Assembly, das CourseValidator enthält
        _services.AddValidatorsFromAssemblyContaining<CourseValidator>();


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
