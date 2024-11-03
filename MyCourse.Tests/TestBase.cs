using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Data;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Extensions;
using MyCourse.Domain.Validation.EntityValidations;
using System.Reflection;
using Moq;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Repositories.CourseRepositories;
using MyCourse.Domain.Data.Repositories.MediaRepositories;

public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext _context;
    protected IServiceProvider _serviceProvider;
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

        // Registriere ILogger<T> mit NullLogger<T>
        _services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

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
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(m => m.WebRootPath).Returns(Path.Combine(Path.GetTempPath(), "wwwroot_test"));

        mockEnv.Setup(m => m.EnvironmentName).Returns("Development");
        mockEnv.Setup(m => m.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        Directory.CreateDirectory(mockEnv.Object.WebRootPath);

        services.AddSingleton<IWebHostEnvironment>(mockEnv.Object);

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
