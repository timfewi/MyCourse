using System.Text.Json;

namespace MyCourse.Web.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                return next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during request processing.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Task.CompletedTask;
            }
        }
    }
}
