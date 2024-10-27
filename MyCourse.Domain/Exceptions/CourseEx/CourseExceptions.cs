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
        public static CourseException NotFound(int courseId)
        {
            return new CourseException(
                CourseErrorCode.NotFound,
                $"Course with id {courseId} not found.",
                courseId: courseId
            );
        }

        public static CourseException CourseFull(int courseId)
        {
            return new CourseException(
                CourseErrorCode.CourseFull,
                $"Course with id {courseId} is full.",
                courseId: courseId
            );
        }

        public static CourseException InvalidOperation(string message, int? courseId = null, object? additionalData = null)
        {
            return new CourseException(
                CourseErrorCode.InvalidOperation,
                message,
                courseId: courseId,
                additionalData: additionalData
            );
        }

        public static CourseException MaxParticipantsExceeded(int maxParticipants)
        {
            return new CourseException(
                CourseErrorCode.MaxParticipantsExceeded,
                $"Maximum participants {maxParticipants} exceeded.",
                additionalData: new { maxParticipants }
            );
        }

        public static CourseException InvalidCourseDate(DateTime courseDate)
        {
            return new CourseException(
                CourseErrorCode.InvalidCourseDate,
                $"Course date {courseDate} is invalid.",
                additionalData: new { courseDate }
            );
        }
    }
}
