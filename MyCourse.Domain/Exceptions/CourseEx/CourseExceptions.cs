using MyCourse.Domain.Exceptions.CourseExceptions.CourseEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.CourseEx
{
    public static class CourseExceptions
    {
        public class CourseNotFoundException : CourseException
        {
            public CourseNotFoundException(int courseId)
                : base(CourseErrorCode.NotFound, $"Course with ID {courseId} not found.", courseId)
            {
            }
        }

        public class CourseFullException : CourseException
        {
            public CourseFullException(int courseId)
                : base(CourseErrorCode.CourseFull, $"Course with ID {courseId} is full.", courseId)
            {
            }
        }

        public class InvalidCourseOperationException : CourseException
        {
            public InvalidCourseOperationException(string message, int? courseId = null, object? additionalData = null)
                : base(CourseErrorCode.InvalidOperation, message, courseId, additionalData)
            {
            }
        }

        public class MaxParticipantsExceededException : CourseException
        {
            public MaxParticipantsExceededException(int courseId)
                : base(CourseErrorCode.MaxParticipantsExceeded, $"Maximum number of participants for course ID {courseId} has been reached.", courseId)
            {
            }
        }

        public class InvalidCourseDateException : CourseException
        {
            public InvalidCourseDateException(DateTime courseDate, int? courseId = null)
                : base(CourseErrorCode.InvalidCourseDate, $"Course date {courseDate} is invalid.", courseId, new { courseDate })
            {
            }
        }

        public class NotActiveCourseException : CourseException
        {
            public NotActiveCourseException(int courseId)
                : base(CourseErrorCode.NotActive, $"Course with ID {courseId} is not active.", courseId)
            {
            }
        }

        public class DuplicateCourseException : CourseException
        {
            public DuplicateCourseException(string title)
                : base(CourseErrorCode.DuplicateCourse, $"Course with title '{title}' already exists.")
            {
            }
        }

        public class UnauthorizedCourseAccessException : CourseException
        {
            public UnauthorizedCourseAccessException(int courseId)
                : base(CourseErrorCode.UnauthorizedAccess, $"Unauthorized access to course with ID {courseId}.", courseId)
            {
            }
        }

        public class CourseSaveException : CourseException
        {
            public CourseSaveException(string message, int? courseId = null, object? additionalData = null)
                : base(CourseErrorCode.SaveFailed, message, courseId, additionalData)
            {
            }
        }

        public class CourseUpdateException : CourseException
        {
            public CourseUpdateException(string message, int? courseId = null, object? additionalData = null)
                : base(CourseErrorCode.UpdateFailed, message, courseId, additionalData)
            {
            }
        }
    }
}
