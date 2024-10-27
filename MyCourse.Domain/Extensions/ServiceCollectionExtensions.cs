using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyCourse.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInjectables(this IServiceCollection services)
        {
            var domainAssembly = Assembly.GetAssembly(typeof(InjectableAttribute));

            if (domainAssembly == null)
                throw new InvalidOperationException("Domain assembly not found.");

            var typesWithAttributes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<InjectableAttribute>().Any())
                .ToList();

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
                            services.Add(new ServiceDescriptor(genericInterface, type, attribute.Lifetime));
                        }
                    }
                }
                else
                {
                    if (interfaces.Any())
                    {
                        foreach (var @interface in interfaces)
                        {
                            switch (attribute.Lifetime)
                            {
                                case ServiceLifetime.Scoped:
                                    services.AddScoped(@interface, type);
                                    break;
                                case ServiceLifetime.Transient:
                                    services.AddTransient(@interface, type);
                                    break;
                                case ServiceLifetime.Singleton:
                                    services.AddSingleton(@interface, type);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                    else
                    {
                        switch (attribute.Lifetime)
                        {
                            case ServiceLifetime.Scoped:
                                services.AddScoped(type);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(type);
                                break;
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(type);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

    }
}
