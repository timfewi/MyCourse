using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ApplicationEx
{
    public static class ApplicationExceptions
    {
        public static ApplicationException NotFound(int applicationId)
        {
            return new ApplicationException(
                ApplicationErrorCode.NotFound,
                $"Application with ID {applicationId} not found.",
                applicationId: applicationId
            );
        }

        public static ApplicationException InvalidOperation(string message, int? applicationId = null, object? additionalData = null)
        {
            return new ApplicationException(
                ApplicationErrorCode.InvalidOperation,
                message,
                applicationId: applicationId,
                additionalData: additionalData
            );
        }

        public static ApplicationException DuplicateApplication(string email, int courseId)
        {
            return new ApplicationException(
                ApplicationErrorCode.DuplicateApplication,
                $"User with email {email} has already applied for course ID {courseId}.",
                additionalData: new { email, courseId }
            );
        }

        public static ApplicationException ApplicationAlreadyProcessed(int applicationId)
        {
            return new ApplicationException(
                ApplicationErrorCode.ApplicationAlreadyProcessed,
                $"Application with ID {applicationId} has already been processed.",
                applicationId: applicationId
            );
        }

        public static ApplicationException ApplicationClosed(int courseId)
        {
            return new ApplicationException(
                ApplicationErrorCode.ApplicationClosed,
                $"Registration period for course ID {courseId} is closed.",
                additionalData: new { courseId }
            );
        }

        public static ApplicationException Unauthorized(string action, int? applicationId = null)
        {
            return new ApplicationException(
                ApplicationErrorCode.Unauthorized,
                $"Unauthorized to perform action '{action}'.",
                applicationId: applicationId
            );
        }

        public static ApplicationException MaxApplicationsReached(int courseId)
        {
            return new ApplicationException(
                ApplicationErrorCode.MaxApplicationsReached,
                $"Maximum number of applications for course ID {courseId} has been reached.",
                additionalData: new { courseId }
            );
        }

        public static ApplicationException InvalidStatusTransition(int applicationId, string fromStatus, string toStatus)
        {
            return new ApplicationException(
                ApplicationErrorCode.InvalidStatusTransition,
                $"Cannot transition application ID {applicationId} from {fromStatus} to {toStatus}.",
                applicationId: applicationId,
                additionalData: new { fromStatus, toStatus }
            );
        }

        // Weitere Methoden für spezifische Fehlerfälle können hier hinzugefügt werden
    }
}
