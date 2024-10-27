using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class InjectableAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; set; }

        public InjectableAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
        }
    }
}
