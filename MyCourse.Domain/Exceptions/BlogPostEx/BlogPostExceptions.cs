using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Exceptions.BlogPostEx
{
    public static class BlogPostExceptions
    {
        // Not Found
        public class BlogPostNotFoundException : BlogPostException
        {
            public BlogPostNotFoundException(int blogPostId) : base(BlogPostErrorCode.NotFound, $"BlogPost with ID {blogPostId} not found.", blogPostId) { }
        }

        // Invalid Operation 
        public class BlogPostInvalidOperationException : BlogPostException
        {
            public BlogPostInvalidOperationException(string message, int? blogPostId = null, object? additionalData = null) : base(BlogPostErrorCode.InvalidOperation, message, blogPostId, additionalData) { }
        }

        // Validation Failed
        public class BlogPostValidationException : BlogPostException
        {
            public BlogPostValidationException(string message, int? blogPostId = null, object? additionalData = null)
                : base(BlogPostErrorCode.ValidationFailed, message, blogPostId, additionalData) { }
        }

        // Duplicate Title
        public class BlogPostDuplicateTitleException : BlogPostException
        {
            public BlogPostDuplicateTitleException(string title, int? blogPostId = null)
                : base(BlogPostErrorCode.DuplicateTitle, $"A BlogPost with the title '{title}' already exists.", blogPostId) { }
        }

        // Media Not Found
        public class BlogPostMediaNotFoundException : BlogPostException
        {
            public BlogPostMediaNotFoundException(int mediaId, int? blogPostId = null)
                : base(BlogPostErrorCode.MediaNotFound, $"Media with ID {mediaId} not found for BlogPost {blogPostId}.", blogPostId, mediaId) { }
        }

        // Media Limit Exceeded
        public class BlogPostMediaLimitExceededException : BlogPostException
        {
            public BlogPostMediaLimitExceededException(int maxMediaAllowed, int? blogPostId = null)
                : base(BlogPostErrorCode.MediaLimitExceeded, $"The maximum number of media items ({maxMediaAllowed}) has been exceeded for BlogPost {blogPostId}.", blogPostId) { }
        }

        // Unauthorized
        public class BlogPostUnauthorizedException : BlogPostException
        {
            public BlogPostUnauthorizedException(string action, int? blogPostId = null)
                : base(BlogPostErrorCode.Unauthorized, $"You are not authorized to {action} BlogPost {blogPostId}.", blogPostId) { }
        }

        // Concurrency Conflict
        public class BlogPostConcurrencyConflictException : BlogPostException
        {
            public BlogPostConcurrencyConflictException(int blogPostId, string message = "A concurrency conflict occurred while updating the BlogPost.")
                : base(BlogPostErrorCode.ConcurrencyConflict, message, blogPostId) { }
        }

        // Invalid Tag
        public class BlogPostInvalidTagException : BlogPostException
        {
            public BlogPostInvalidTagException(string tag, int? blogPostId = null)
                : base(BlogPostErrorCode.InvalidTag, $"The tag '{tag}' is invalid for BlogPost {blogPostId}.", blogPostId, tag) { }
        }

        // Database Error
        public class BlogPostDatabaseException : BlogPostException
        {
            public BlogPostDatabaseException(string message, int? blogPostId = null, object? additionalData = null)
                : base(BlogPostErrorCode.DatabaseError, message, blogPostId, additionalData) { }
        }


        // Update Error
        public class BlogPostUpdateException : BlogPostException
        {
            public BlogPostUpdateException(string message, int? blogPostId, object? additionalData = null) : base(BlogPostErrorCode.InvalidOperation, message, blogPostId, additionalData) { }
        }
    }
}
