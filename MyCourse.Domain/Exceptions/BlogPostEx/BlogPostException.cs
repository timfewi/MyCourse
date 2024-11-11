using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.BlogPostEx
{
    public class BlogPostException : Exception
    {
        public BlogPostErrorCode ErrorCode { get; set; }
        public int? BlogPostId { get; set; }
        public object? AdditionalData { get; set; }

        public BlogPostException(BlogPostErrorCode errorCode,string message, int? blogPostId = null, object? additionalData = null) : base(message)
        {
            ErrorCode = errorCode;
            BlogPostId = blogPostId;
            AdditionalData = additionalData;
        }
    }
}
