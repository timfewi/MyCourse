using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ApplicationEx
{
    public enum ApplicationErrorCode
    {
        NotFound,                     // Die Anmeldung wurde nicht gefunden
        InvalidOperation,             // Ungültiger Betriebszustand oder -operation
        DuplicateApplication,         // Der Benutzer hat sich bereits für diesen Kurs angemeldet
        ApplicationAlreadyProcessed,  // Die Anmeldung wurde bereits genehmigt oder abgelehnt
        ApplicationClosed,            // Der Anmeldezeitraum für den Kurs ist abgeschlossen
        Unauthorized,                 // Der Benutzer ist nicht berechtigt, diese Operation durchzuführen
        MaxApplicationsReached,       // Maximale Anzahl von Anmeldungen für den Kurs erreicht
        InvalidStatusTransition,      // Ungültiger Statusübergang der Anmeldung
        MissingRequiredFields         // Erforderliche Felder fehlen (optional, da FluentValidation dies abdeckt)
    }
}
