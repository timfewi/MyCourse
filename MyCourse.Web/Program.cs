using Serilog;
using Microsoft.EntityFrameworkCore;
using MyCourse.Domain.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyCourse.Web.Middlewares;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Validation.EntityValidations;
using MyCourse.Domain.Data.Seeders;
using MyCourse.Domain.Extensions;
using MyCourse.Domain.MappingProfiles;
using System.Configuration;
using MyCourse.Domain.POCOs;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Appsettings.json config
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Connection string aus appsettings.json auslesen
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException
    ("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// Add Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie-Einstellungen konfigurieren (optional)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Account/Login"; // Pfad zur Admin-Login-Seite
    options.AccessDeniedPath = "/Admin/Error/AccessDenied"; // Pfad für Zugriff verweigert
    options.ExpireTimeSpan = TimeSpan.FromDays(14); // Ablaufzeit für persistente Cookies
    options.SlidingExpiration = true; // Automatische Verlängerung der Ablaufzeit bei Aktivität
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
});

// Binden der SMTP-Einstellungen
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<SmtpSettings>(sp => sp.GetRequiredService<IOptions<SmtpSettings>>().Value);

// FluentValidation registrieren
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Register validators from MyCourse.Domain
builder.Services.AddValidatorsFromAssemblyContaining<CourseValidator>();

// Automatic DI register
builder.Services.AddInjectables();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add AutoMapper (.Domain Assembly)
builder.Services.AddAutoMapper(typeof(CourseProfile));

// Add Middlewares
builder.Services.AddTransient<ExceptionMiddleware>();

// Memory Cache
builder.Services.AddMemoryCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    try
    {
        await AdminInitializer.InitializeAdminAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred during migration");
    }
}


app.Run();
