using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Repositories;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Extensions;
using MyCourse.Domain.Services.ApplicationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.DependencyInjection
{
    public class DependencyInjectionTests : TestBase
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddInjectables();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        }

        [Fact]
        public void AddInjectables_RegisterAllInjectableServices()
        {
            // Arrange
            var services = _services;

            // Act
            // AddInjectables wurde bereits in ConfigureServices durch TestBase aufgerufen

            // Erstelle eine Liste aller Typen mit dem [Injectable]-Attribut
            var domainAssembly = Assembly.GetAssembly(typeof(InjectableAttribute));
            Assert.NotNull(domainAssembly);

            var typesWithAttributes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<InjectableAttribute>().Any())
                .ToList();

            // Assert
            foreach (var type in typesWithAttributes)
            {
                var attribute = type.GetCustomAttribute<InjectableAttribute>()!;
                var interfaces = type.GetInterfaces();

                if (type.IsGenericTypeDefinition)
                {
                    foreach (var @interface in interfaces)
                    {
                        if (@interface.IsGenericType)
                        {
                            var genericInterface = @interface.GetGenericTypeDefinition();
                            var serviceDescriptor = services.FirstOrDefault(sd =>
                                sd.ServiceType == genericInterface &&
                                sd.ImplementationType == type &&
                                sd.Lifetime == attribute.Lifetime);

                            Assert.NotNull(serviceDescriptor);
                        }
                    }
                }
                else
                {
                    if (interfaces.Any())
                    {
                        foreach (var @interface in interfaces)
                        {
                            var serviceDescriptor = services.FirstOrDefault(sd =>
                                sd.ServiceType == @interface &&
                                sd.ImplementationType == type &&
                                sd.Lifetime == attribute.Lifetime);

                            Assert.NotNull(serviceDescriptor);
                        }
                    }
                    else
                    {
                        var serviceDescriptor = services.FirstOrDefault(sd =>
                            sd.ServiceType == type &&
                            sd.ImplementationType == type &&
                            sd.Lifetime == attribute.Lifetime);

                        Assert.NotNull(serviceDescriptor);
                    }
                }
            }
        }

        [Fact]
        public void Services_Should_Be_Resolvable()
        {
            // Arrange
            var serviceProvider = _serviceProvider;

            // Erstelle eine Liste aller Typen mit dem [Injectable]-Attribut
            var domainAssembly = Assembly.GetAssembly(typeof(InjectableAttribute));
            Assert.NotNull(domainAssembly);

            var typesWithAttributes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<InjectableAttribute>().Any())
                .ToList();

            // Act & Assert
            foreach (var type in typesWithAttributes)
            {
                var attribute = type.GetCustomAttribute<InjectableAttribute>()!;
                var interfaces = type.GetInterfaces();

                if (type.IsGenericTypeDefinition)
                {
                    // Generische Typen erfordern spezielle Auflösung
                    // Wird seperat getestet
                    continue;
                }

                if (interfaces.Any())
                {
                    foreach (var @interface in interfaces)
                    {
                        var service = serviceProvider.GetService(@interface);
                        Assert.NotNull(service);
                        Assert.IsType(type, service);
                    }
                }
                else
                {
                    var service = serviceProvider.GetService(type);
                    Assert.NotNull(service);
                    Assert.IsType(type, service);
                }
            }
        }

        [Fact]
        public void ScopedServices_Should_Be_Same_Within_Scope_And_Different_Across_Scopes()
        {
            // Arrange
            var serviceProvider = _serviceProvider;

            var domainAssembly = Assembly.GetAssembly(typeof(InjectableAttribute));
            Assert.NotNull(domainAssembly);

            var scopedTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                            t.GetCustomAttribute<InjectableAttribute>()?.Lifetime == ServiceLifetime.Scoped)
                .ToList();

            foreach (var type in scopedTypes)
            {
                var interfaces = type.GetInterfaces();

                if (type.IsGenericTypeDefinition)
                {
                    // Generische Typen erfordern spezifische Tests
                    continue;
                }

                if (interfaces.Any())
                {
                    foreach (var @interface in interfaces)
                    {
                        using (var scope1 = serviceProvider.CreateScope())
                        using (var scope2 = serviceProvider.CreateScope())
                        {
                            var service1 = scope1.ServiceProvider.GetService(@interface);
                            var service2 = scope1.ServiceProvider.GetService(@interface);
                            var service3 = scope2.ServiceProvider.GetService(@interface);

                            Assert.NotNull(service1);
                            Assert.NotNull(service2);
                            Assert.NotNull(service3);

                            // Innerhalb desselben Scopes sollten service1 und service2 gleich sein
                            Assert.Same(service1, service2);

                            // Über verschiedene Scopes hinweg sollten service1 und service3 unterschiedlich sein
                            Assert.NotSame(service1, service3);
                        }
                    }
                }
                else
                {
                    using (var scope1 = serviceProvider.CreateScope())
                    using (var scope2 = serviceProvider.CreateScope())
                    {
                        var service1 = scope1.ServiceProvider.GetService(type);
                        var service2 = scope1.ServiceProvider.GetService(type);
                        var service3 = scope2.ServiceProvider.GetService(type);

                        Assert.NotNull(service1);
                        Assert.NotNull(service2);
                        Assert.NotNull(service3);

                        // Innerhalb desselben Scopes sollten service1 und service2 gleich sein
                        Assert.Same(service1, service2);

                        // Über verschiedene Scopes hinweg sollten service1 und service3 unterschiedlich sein
                        Assert.NotSame(service1, service3);
                    }
                }
            }
        }

        [Fact]
        public void TransientServices_Should_Be_Different_Each_Time_Resolved()
        {
            // Arrange
            var serviceProvider = _serviceProvider;

            var domainAssembly = Assembly.GetAssembly(typeof(InjectableAttribute));
            Assert.NotNull(domainAssembly);

            var transientTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                            t.GetCustomAttribute<InjectableAttribute>()?.Lifetime == ServiceLifetime.Transient)
                .ToList();

            foreach (var type in transientTypes)
            {
                var interfaces = type.GetInterfaces();

                if (type.IsGenericTypeDefinition)
                {
                    // Generische Typen erfordern spezifische Auflösung
                    continue;
                }

                if (interfaces.Any())
                {
                    foreach (var @interface in interfaces)
                    {
                        var service1 = serviceProvider.GetService(@interface);
                        var service2 = serviceProvider.GetService(@interface);
                        Assert.NotNull(service1);
                        Assert.NotNull(service2);
                        Assert.NotSame(service1, service2);
                    }
                }
                else
                {
                    var service1 = serviceProvider.GetService(type);
                    var service2 = serviceProvider.GetService(type);
                    Assert.NotNull(service1);
                    Assert.NotNull(service2);
                    Assert.NotSame(service1, service2);
                }
            }
        }

        [Fact]
        public void SingletonServices_Should_Be_Same_Every_Time_Resolved()
        {
            // Arrange
            var serviceProvider = _serviceProvider;

            var domainAssembly = Assembly.GetAssembly(typeof(InjectableAttribute));
            Assert.NotNull(domainAssembly);

            var singletonTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                            t.GetCustomAttribute<InjectableAttribute>()?.Lifetime == ServiceLifetime.Singleton)
                .ToList();

            foreach (var type in singletonTypes)
            {
                var interfaces = type.GetInterfaces();

                if (type.IsGenericTypeDefinition)
                {
                    // Generische Typen erfordern spezifische Auflösung
                    continue;
                }

                if (interfaces.Any())
                {
                    foreach (var @interface in interfaces)
                    {
                        var service1 = serviceProvider.GetService(@interface);
                        var service2 = serviceProvider.GetService(@interface);
                        Assert.NotNull(service1);
                        Assert.NotNull(service2);
                        Assert.Same(service1, service2);
                    }
                }
                else
                {
                    var service1 = serviceProvider.GetService(type);
                    var service2 = serviceProvider.GetService(type);
                    Assert.NotNull(service1);
                    Assert.NotNull(service2);
                    Assert.Same(service1, service2);
                }
            }
        }

    }
}
