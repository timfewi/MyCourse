using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ApplicationEx
{
    public class ApplicationException : Exception
    {
        public ApplicationErrorCode ErrorCode { get; }
        public int? ApplicationId { get; }
        public object? AdditionalData { get; }

        public ApplicationException(ApplicationErrorCode errorCode, string message, int? applicationId = null, object? additionalData = null)
            : base(message)
        {
            ErrorCode = errorCode;
            ApplicationId = applicationId;
            AdditionalData = additionalData;
        }
    }
}
