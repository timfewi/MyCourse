using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.MediaEx
{
    public enum MediaErrorCode
    {
        FileEmpty,
        FileNotExists,
        InvalidOperation,
        ValidationFailed,
        UnsupportedFileType,
        FileTooLarge,
        MediaNotLinkedToCourse,
        MediaAlreadyExists,
        UnauthorizedAccess,
        SaveFailed
    }
}
