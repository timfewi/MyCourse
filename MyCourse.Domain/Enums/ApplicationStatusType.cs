using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Enums
{
    public enum ApplicationStatusType
    {
        Pending,       // Bewerbung eingegangen, aber noch nicht bearbeitet
        Applied,       // Bewerbung bearbeitet, aber noch keine Bestätigung
        Participant,   // Benutzer wurde als Teilnehmer akzeptiert
        Waiting,       // Benutzer steht auf der Warteliste
        //Confirmed,     // Benutzer wurde fest als Teilnehmer bestätigt
        //Rejected,      // Bewerbung abgelehnt
        //Cancelled,     // Teilnahme storniert
        //Completed,     // Kurs erfolgreich abgeschlossen
        //Withdrawn      // Benutzer hat den Kurs vorzeitig verlassen
    }

}
