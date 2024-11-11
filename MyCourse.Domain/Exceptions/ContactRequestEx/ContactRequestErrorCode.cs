using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ContactRequestEx
{
    public enum ContactRequestErrorCode
    {
        NotFound,
        AlreadyAnswered,
        EmailSendingFailed,
        ValidationError,
        UnauthorizedAccess,

    }
}
