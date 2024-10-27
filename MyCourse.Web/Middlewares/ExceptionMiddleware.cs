using FluentValidation;
using MyCourse.Domain.Exceptions.ApplicationEx;
using MyCourse.Domain.Exceptions.CourseExceptions.CourseEx;
using MyCourse.Web.Models.ErrorModels;
using System.Net;
using System.Text.Json;

namespace MyCourse.Web.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (CourseException ex)
            {
                _logger.LogWarning(ex, "A course-related error occurred.");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    Error = ex.Message,
                    Code = ex.ErrorCode.ToString(),
                    Details = new
                    {
                        courseId = ex.CourseId,
                        additionalData = ex.AdditionalData
                    }
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
            catch (Domain.Exceptions.ApplicationEx.ApplicationException ex)
            {
                // Behandlung von benutzerdefinierten ApplicationException
                _logger.LogWarning(ex, "An application-related error occurred.");

                context.Response.StatusCode = ex.ErrorCode switch
                {
                    ApplicationErrorCode.NotFound => (int)HttpStatusCode.NotFound,
                    ApplicationErrorCode.Unauthorized => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.BadRequest
                };
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    Error = ex.Message,
                    Code = ex.ErrorCode.ToString(),
                    Details = new
                    {
                        applicationId = ex.ApplicationId,
                        additionalData = ex.AdditionalData
                    }
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "A validation error occurred.");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage });

                var response = new ErrorResponse
                {
                    Error = "Validation failed",
                    Details = errors
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    Error = "An unexpected error occurred.",
                    Details = _env.IsDevelopment() ? ex.Message : null
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
