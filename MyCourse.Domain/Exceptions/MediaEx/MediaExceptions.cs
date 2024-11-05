using FluentValidation.Results;
using MyCourse.Domain.Exceptions.CourseEx;
using MyCourse.Domain.Exceptions.CourseExceptions.CourseEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.MediaEx
{
    public class MediaNotFoundException : MediaException
    {
        public MediaNotFoundException(int mediaId)
            : base(MediaErrorCode.FileNotExists, $"Media with ID {mediaId} does not exist.", mediaId)
        {
        }
    }

    public class InvalidMediaOperationException : MediaException
    {
        public InvalidMediaOperationException(string message, int? mediaId = null, object? additionalData = null)
            : base(MediaErrorCode.InvalidOperation, message, mediaId, additionalData)
        {
        }
    }

    public class MediaValidationException : MediaException
    {
        public IList<ValidationFailure> ValidationErrors { get; }

        public MediaValidationException(IList<ValidationFailure> errors)
            : base(MediaErrorCode.ValidationFailed, "Media validation failed.", null, errors)
        {
            ValidationErrors = errors;
        }
    }

    public class MediaSaveException : MediaException
    {
        public MediaSaveException(string message, int? mediaId = null, object? additionalData = null)
            : base(MediaErrorCode.SaveFailed, message, mediaId, additionalData)
        {
        }
    }


    public class UnsupportedFileTypeException : MediaException
    {
        public string FileName { get; }
        public string FileType { get; }

        public UnsupportedFileTypeException(string fileName, string fileType, int? mediaId = null, object? additionalData = null)
            : base(MediaErrorCode.UnsupportedFileType, $"File type '{fileType}' of file '{fileName}' is not supported.", mediaId, additionalData)
        {
            FileName = fileName;
            FileType = fileType;
        }
    }

    public class FileTooLargeException : MediaException
    {
        public string FileName { get; }
        public long FileSize { get; }
        public long MaxAllowedSize { get; }

        public FileTooLargeException(string fileName, long fileSize, long maxAllowedSize, int? mediaId = null, object? additionalData = null)
            : base(MediaErrorCode.FileTooLarge, $"File '{fileName}' is too large ({fileSize} bytes). Max allowed size is {maxAllowedSize} bytes.", mediaId, additionalData)
        {
            FileName = fileName;
            FileSize = fileSize;
            MaxAllowedSize = maxAllowedSize;
        }
    }

    public class MediaAlreadyExistsException : MediaException
    {
        public MediaAlreadyExistsException(string fileName, int? mediaId = null, object? additionalData = null)
            : base(MediaErrorCode.MediaAlreadyExists, $"Media with file name '{fileName}' already exists.", mediaId, additionalData)
        {
        }
    }

    public class UnauthorizedMediaAccessException : MediaException
    {
        public UnauthorizedMediaAccessException(int mediaId)
            : base(MediaErrorCode.UnauthorizedAccess, $"Unauthorized access to media with ID {mediaId}.", mediaId)
        {
        }
    }
}
