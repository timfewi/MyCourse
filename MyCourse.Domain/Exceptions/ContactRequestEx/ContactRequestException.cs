using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ContactRequestEx
{
    public class ContactRequestException : Exception
    {
        public ContactRequestErrorCode ErrorCode { get; set; }
        public int? ContactRequestId { get; set; }
        public object? AdditionalData { get; set; }

        public ContactRequestException(ContactRequestErrorCode errorCode, string message, int? contactRequestId, object? additionalData = null) : base(message)
        {
            ErrorCode = errorCode;
            ContactRequestId = contactRequestId;
            AdditionalData = additionalData;
        }
    }
}
