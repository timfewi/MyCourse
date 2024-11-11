using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ContactRequestEx
{
    public class ContactRequestNotFoundException : ContactRequestException
    {
        public ContactRequestNotFoundException(int contactRequestId) : base(ContactRequestErrorCode.NotFound, $"ContactRequest with ID {contactRequestId} not found.", contactRequestId)
        {
        }
    }
    public class AlreadyAnsweredException : ContactRequestException
    {
        public AlreadyAnsweredException(int contactRequestId)
            : base(ContactRequestErrorCode.AlreadyAnswered,
                   $"Kontaktanfrage mit ID {contactRequestId} wurde bereits beantwortet.",
                   contactRequestId)
        {
        }
    }

    public class ContactRequestEmailSendingException : ContactRequestException
    {
        public ContactRequestEmailSendingException(int contactRequestId, string message)
            : base(ContactRequestErrorCode.EmailSendingFailed,
                   $"Fehler beim Senden der Antwort-E-Mail für Kontaktanfrage ID {contactRequestId}: {message}",
                   contactRequestId)
        {
        }
    }

    public class ContactRequestValidationException : ContactRequestException
    {
        public ContactRequestValidationException(int? contactRequestId, string message)
            : base(ContactRequestErrorCode.ValidationError,
                   $"Validierungsfehler für Kontaktanfrage ID {contactRequestId}: {message}",
                   contactRequestId)
        {
        }
    }

    public class ContactRequestUnauthorizedException : ContactRequestException
    {
        public ContactRequestUnauthorizedException(int contactRequestId)
            : base(ContactRequestErrorCode.UnauthorizedAccess,
                   $"Unbefugter Zugriff auf Kontaktanfrage ID {contactRequestId}.",
                   contactRequestId)
        {
        }
    }
}
