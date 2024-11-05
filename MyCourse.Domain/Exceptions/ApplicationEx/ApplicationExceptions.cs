using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ApplicationEx
{
    public static class ApplicationExceptions
    {
        public class ApplicationNotFoundException : ApplicationException
        {
            public ApplicationNotFoundException(int applicationId)
                : base(ApplicationErrorCode.NotFound, $"Application with ID {applicationId} not found.", applicationId)
            {
            }
        }

        public class DuplicateApplicationException : ApplicationException
        {
            public DuplicateApplicationException(string email, int courseId)
                : base(ApplicationErrorCode.DuplicateApplication, $"An application with email '{email}' already exists for course ID {courseId}.", null, new { email, courseId })
            {
            }
        }

        public class ApplicationAlreadyProcessedException : ApplicationException
        {
            public ApplicationAlreadyProcessedException(int applicationId)
                : base(ApplicationErrorCode.ApplicationAlreadyProcessed, $"Application with ID {applicationId} has already been processed.", applicationId)
            {
            }
        }

        public class UnauthorizedApplicationAccessException : ApplicationException
        {
            public UnauthorizedApplicationAccessException(int applicationId)
                : base(ApplicationErrorCode.Unauthorized, $"Unauthorized access to application with ID {applicationId}.", applicationId)
            {
            }
        }

        public class MaxApplicationsReachedException : ApplicationException
        {
            public MaxApplicationsReachedException(int courseId)
                : base(ApplicationErrorCode.MaxApplicationsReached, $"Maximum number of applications for course ID {courseId} has been reached.", null, new { courseId })
            {
            }
        }

        public class InvalidStatusTransitionException : ApplicationException
        {
            public InvalidStatusTransitionException(int applicationId, string fromStatus, string toStatus)
                : base(ApplicationErrorCode.InvalidStatusTransition, $"Cannot transition application ID {applicationId} from {fromStatus} to {toStatus}.", applicationId, new { fromStatus, toStatus })
            {
            }
        }

        public class ApplicationSaveException : ApplicationException
        {
            public ApplicationSaveException(string message, int? applicationId = null, object? additionalData = null)
                : base(ApplicationErrorCode.SaveFailed, message, applicationId, additionalData)
            {
            }
        }
    }
}
