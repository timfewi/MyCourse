using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.Entities
{
    public class ApplicationEntityTests : TestBase
    {
       [Fact]
        public void Can_Create_Application_With_Valid_Data()
        {
            // Arrange
            var application = new Application
            {
                CourseId = TestConstants.ValidCourseId,
                FirstName = TestConstants.ValidFirstName,
                LastName = TestConstants.ValidLastName,
                Email = TestConstants.ValidEmail,
                ApplicationDate = DateTime.Now.Date,
                Status = TestConstants.ValidStatus,
            };

            // Act
            _context.Applications.Add(application);
            _context.SaveChanges();

            // Assert
            var createdApplication = _context.Applications.Find(application.Id);
            Assert.NotNull(createdApplication);
            Assert.Equal(TestConstants.ValidCourseId, createdApplication.CourseId);
            Assert.Equal(TestConstants.ValidFirstName, createdApplication.FirstName);

            Assert.Equal(TestConstants.ValidEmail, createdApplication.Email);
            Assert.Equal(DateTime.Now.Date, createdApplication.ApplicationDate.Date);
            Assert.Equal(TestConstants.ValidStatus, createdApplication.Status);
        }
    }
}
