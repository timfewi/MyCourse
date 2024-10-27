using MyCourse.Domain.Exceptions.CourseEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.CourseExceptions.CourseEx
{
    public class CourseException : Exception
    {
        public CourseErrorCode ErrorCode { get; }
        public int? CourseId { get; }
        public object? AdditionalData { get; }

        public CourseException(CourseErrorCode errorCode, string message, int? courseId = null, object? additionalData = null)
            : base(message)
        {
            ErrorCode = errorCode;
            CourseId = courseId;
            AdditionalData = additionalData;
        }
    }

}
