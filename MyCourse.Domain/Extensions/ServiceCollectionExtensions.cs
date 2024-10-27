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
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes<InjectableAttribute>().Any());

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
                            services.Add(new ServiceDescriptor(@interface.GetGenericTypeDefinition(), type, attribute.Lifetime));
                        }
                    }
                }
                else
                {
                    if (interfaces.Any())
                    {
                        foreach (var @interface in interfaces)
                        {
                            services.Add(new ServiceDescriptor(@interface, type, attribute.Lifetime));
                        }
                    }
                    else
                    {
                        services.Add(new ServiceDescriptor(type, type, attribute.Lifetime));
                    }
                }
            }
        }


    }
}
