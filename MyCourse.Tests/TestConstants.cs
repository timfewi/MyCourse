using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests
{
    internal static class TestConstants
    {
        // Course Testdata
        // Valid-Data
        public const string ValidTitle = "Title";
        public const string ValidDescription = "Description";
        public static readonly TimeSpan ValidCourseDuration = TimeSpan.FromHours(2);
        public const int ValidMaxParticipants = 10;
        public const decimal ValidPrice = 99.99m;
        public const string ValidLocation = "Location";
        public const bool ValidIsActive = true;

        // Invalid-Data
        public const string InvalidTitle = null;
        public const string InvalidDescription = null;
        public static readonly TimeSpan InvalidCourseDuration = TimeSpan.Zero;
        public const int InvalidMaxParticipants = 0;
        public const decimal InvalidPrice = -1;
        public const string InvalidLocation = null;
        public const bool InvalidIsActive = false;

        // Application Testdata
        // Valid-Data
        public const string ValidFirstName = "FirstName";
        public const string ValidLastName = "LastName";
        public const string ValidEmail = "test@example.com";
        public const string ValidPhoneNumber = "+436701234567";
        public static readonly DateTime ValidApplicationDate = DateTime.Now;
        public const ApplicationStatusType ValidStatus = ApplicationStatusType.Approved;
        // Invalid-Data
        public const string InvalidFirstName = null;
        public const string InvalidLastName = null;
        public const string InvalidEmail = null;
        public const string InvalidPhoneNumber = null;
        public static readonly DateTime InvalidApplicationDate = DateTime.MinValue;
        public const ApplicationStatusType InvalidStatus = ApplicationStatusType.Approved;

        // Media Testdata
        // Valid-Data
        public const string ValidMediaUrl = "/uploads/images/tests/test-cat.jpg";
        public const string ValidFileName = "test.jpg";
        public const string ValidContentType = "image/jpeg";
        public const MediaType ValidMediaType = MediaType.Image;
        public const long ValidFileSize = 1024;
        // Invalid-Data
        public const string InvalidMediaUrl = null;
        public const string InvalidFileName = null;
        public const string InvalidContentType = null;
        public const MediaType InvalidMediaType = MediaType.Image;
        public const long InvalidFileSize = 0;

        // ID Testdata
        // Valid-Data
        public const int ValidApplicationId = 1;
        public const int ValidCourseId = 1;
        public const int ValidMediaId = 1;
        // Invalid-Data
        public const int InvalidCourseId = 0;
        public const int InvalidApplicationId = 0;
        public const int InvalidMediaId = 0;

    }
}
