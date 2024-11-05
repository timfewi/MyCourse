using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.CourseEx
{
    public enum CourseErrorCode
    {
        NotFound,
        CourseFull,
        InvalidOperation,
        NotActive,
        MaxParticipantsExceeded,
        InvalidCourseDate,
        DuplicateCourse,
        UnauthorizedAccess,
        SaveFailed,
        UpdateFailed,
    }

}
