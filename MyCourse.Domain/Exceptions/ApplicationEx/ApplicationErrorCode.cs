using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.ApplicationEx
{
    public enum ApplicationErrorCode
    {
        NotFound,                   
        InvalidOperation,            
        DuplicateApplication,         
        ApplicationAlreadyProcessed, 
        ApplicationClosed,          
        Unauthorized,            
        MaxApplicationsReached,      
        InvalidStatusTransition,      
        MissingRequiredFields,
        SaveFailed

    }
}
