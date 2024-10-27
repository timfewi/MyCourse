using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }
        [Fact]
        public void AddInjectables_RegisterAllInjectableSevices()
        {
            // Arrange 
            var services = new ServiceCollection();

            // Build the service provider to 
        }
    }
}
