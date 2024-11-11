using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.BlogPostEx
{
    public enum BlogPostErrorCode
    {
        NotFound,
        InvalidOperation,
        ValidationFailed,
        DuplicateTitle,
        MediaNotFound,
        MediaLimitExceeded,
        Unauthorized,
        ConcurrencyConflict,
        InvalidTag,
        DatabaseError
    }
}
