using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.Entities
{
    public class MediaEntityTests : TestBase
    {
        [Fact]
        public void Can_Create_Media_With_Valid_Data()
        {
            // Arrange
            var media = new Media
            {
                Url = TestConstants.ValidMediaUrl,
                FileName = TestConstants.ValidFileName,
                ContentType = TestConstants.ValidContentType,
                MediaType = TestConstants.ValidMediaType,
                Description = TestConstants.ValidDescription,
                FileSize = TestConstants.ValidFileSize,
                CourseMedias = new List<CourseMedia>
                {
                    new CourseMedia
                    {
                        CourseId = TestConstants.ValidCourseId
                    }
                },
                ApplicationMedias = new List<ApplicationMedia>
                {
                    new ApplicationMedia
                    {
                        ApplicationId = TestConstants.ValidApplicationId
                    }
                }
            };
            // Act
            _context.Medias.Add(media);
            _context.SaveChanges();
            // Assert
            var createdMedia = _context.Medias.Find(media.Id);
            Assert.NotNull(createdMedia);
            Assert.Equal(TestConstants.ValidMediaUrl, createdMedia.Url);
            Assert.Equal(TestConstants.ValidMediaType, createdMedia.MediaType);
            Assert.Single(createdMedia.CourseMedias);
            Assert.Single(createdMedia.ApplicationMedias);
        }
    }
}
