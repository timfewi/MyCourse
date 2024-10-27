using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Services
{
    public abstract class BaseService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<BaseService> _logger;
        protected readonly IMapper _mapper;
        protected readonly IConfiguration _configuration;

        protected BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = _serviceProvider.GetService<ILogger<BaseService>>() ?? throw new ArgumentNullException(nameof(_logger));
            _mapper = _serviceProvider.GetService<IMapper>() ?? throw new ArgumentNullException(nameof(_mapper));
            _configuration = _serviceProvider.GetService<IConfiguration>() ?? throw new ArgumentNullException(nameof(_configuration));
        }

    }
}
