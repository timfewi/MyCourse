using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.MediaEx
{
    public class MediaException : Exception
    {
        public MediaErrorCode ErrorCode { get; set; }
        public int? MediaId { get; set; }
        public object? AdditionalData { get; set; }

        public MediaException(MediaErrorCode errorCode, string message, int? mediaId, object? additionalData = null) : base(message) 
        {
            ErrorCode = errorCode;
            MediaId = mediaId;
            AdditionalData = additionalData;
        }
    }
}
