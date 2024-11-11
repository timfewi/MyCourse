using FluentValidation;
using MyCourse.Domain.Exceptions.ApplicationEx;
using MyCourse.Domain.Exceptions.ContactRequestEx;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Domain.Exceptions.CourseExceptions.CourseEx;
using MyCourse.Domain.Exceptions.MediaEx;
using MyCourse.Web.Models.ErrorModels;
using System.Net;
using System.Text.Json;
using ApplicationException = MyCourse.Domain.Exceptions.ApplicationEx.ApplicationException;

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
            catch (MediaException ex)
            {
                _logger.LogWarning(ex, "A media-related error occurred.");

                context.Response.StatusCode = ex.ErrorCode switch
                {
                    MediaErrorCode.FileNotExists => (int)HttpStatusCode.NotFound,
                    MediaErrorCode.FileEmpty => (int)HttpStatusCode.BadRequest,
                    MediaErrorCode.ValidationFailed => (int)HttpStatusCode.BadRequest,
                    MediaErrorCode.UnsupportedFileType => (int)HttpStatusCode.UnsupportedMediaType,
                    MediaErrorCode.FileTooLarge => (int)HttpStatusCode.RequestEntityTooLarge,
                    MediaErrorCode.UnauthorizedAccess => (int)HttpStatusCode.Unauthorized,
                    MediaErrorCode.InvalidOperation => (int)HttpStatusCode.BadRequest,
                    MediaErrorCode.MediaAlreadyExists => (int)HttpStatusCode.Conflict,
                    MediaErrorCode.MediaNotLinkedToCourse => (int)HttpStatusCode.BadRequest,
                    MediaErrorCode.SaveFailed => (int)HttpStatusCode.Conflict,
                    _ => (int)HttpStatusCode.InternalServerError
                };
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    Error = ex.Message,
                    Code = ex.ErrorCode.ToString(),
                    Details = new
                    {
                        mediaId = ex.MediaId,
                        additionalData = ex.AdditionalData
                    }
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            catch (CourseException ex)
            {
                _logger.LogWarning(ex, "A course-related error occurred.");

                context.Response.StatusCode = ex.ErrorCode switch
                {
                    CourseErrorCode.NotFound => (int)HttpStatusCode.NotFound,
                    CourseErrorCode.CourseFull => (int)HttpStatusCode.Conflict,
                    CourseErrorCode.InvalidOperation => (int)HttpStatusCode.BadRequest,
                    CourseErrorCode.NotActive => (int)HttpStatusCode.BadRequest,
                    CourseErrorCode.MaxParticipantsExceeded => (int)HttpStatusCode.BadRequest,
                    CourseErrorCode.InvalidCourseDate => (int)HttpStatusCode.BadRequest,
                    CourseErrorCode.DuplicateCourse => (int)HttpStatusCode.Conflict,
                    CourseErrorCode.UnauthorizedAccess => (int)HttpStatusCode.Unauthorized,
                    CourseErrorCode.SaveFailed => (int)HttpStatusCode.InternalServerError,
                    CourseErrorCode.UpdateFailed => (int)HttpStatusCode.InternalServerError,
                    _ => (int)HttpStatusCode.InternalServerError
                };

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
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "An application-related error occurred.");

                context.Response.StatusCode = ex.ErrorCode switch
                {
                    ApplicationErrorCode.NotFound => (int)HttpStatusCode.NotFound,
                    ApplicationErrorCode.InvalidOperation => (int)HttpStatusCode.BadRequest,
                    ApplicationErrorCode.DuplicateApplication => (int)HttpStatusCode.Conflict,
                    ApplicationErrorCode.ApplicationAlreadyProcessed => (int)HttpStatusCode.BadRequest,
                    ApplicationErrorCode.ApplicationClosed => (int)HttpStatusCode.BadRequest,
                    ApplicationErrorCode.Unauthorized => (int)HttpStatusCode.Unauthorized,
                    ApplicationErrorCode.MaxApplicationsReached => (int)HttpStatusCode.BadRequest,
                    ApplicationErrorCode.InvalidStatusTransition => (int)HttpStatusCode.BadRequest,
                    ApplicationErrorCode.MissingRequiredFields => (int)HttpStatusCode.BadRequest,
                    ApplicationErrorCode.SaveFailed => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
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
            catch(ContactRequestException ex)
            {
                _logger.LogWarning(ex, "A contactrequest-related error occurred.");

                context.Response.StatusCode = ex.ErrorCode switch
                {
                    ContactRequestErrorCode.NotFound => (int)HttpStatusCode.NotFound,
                    ContactRequestErrorCode.EmailSendingFailed => (int)HttpStatusCode.InternalServerError,
                    ContactRequestErrorCode.UnauthorizedAccess => (int)HttpStatusCode.Unauthorized,
                    ContactRequestErrorCode.ValidationError => (int)HttpStatusCode.BadRequest,
                    ContactRequestErrorCode.AlreadyAnswered => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {

                    Error = ex.Message,
                    Code = ex.ErrorCode.ToString(),
                    Details = new
                    {
                        contactRequestId = ex.ContactRequestId,
                        additionalData = ex.AdditionalData,
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
