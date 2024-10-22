using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Tests.UnitTests.Domain.Entities
{
    public class CourseEntityTests : TestBase
    {
        [Fact]
        public void Can_Create_Course_With_Valid_Data()
        {
            // Arrange
            var course = new Course
            {
                Title = TestConstants.ValidTitle,
                Description = TestConstants.ValidDescription,
                CourseDuration = TestConstants.ValidCourseDuration,
                CourseDate = DateTime.Now.AddDays(1),
                MaxParticipants = TestConstants.ValidMaxParticipants,
                Price = TestConstants.ValidPrice,
                Location = TestConstants.ValidLocation,
                IsActive = TestConstants.ValidIsActive
            };

            // Act
            _context.Courses.Add(course);
            _context.SaveChanges();

            // Assert
            var createdCourse = _context.Courses.Find(course.Id);
            Assert.NotNull(createdCourse);
            Assert.Equal(TestConstants.ValidTitle, createdCourse.Title);
            Assert.Equal(TestConstants.ValidDescription, createdCourse.Description);
            Assert.Equal(TestConstants.ValidCourseDuration, createdCourse.CourseDuration);
            Assert.Equal(DateTime.Now.AddDays(1).Date, createdCourse.CourseDate.Date);
            Assert.Equal(TestConstants.ValidMaxParticipants, createdCourse.MaxParticipants);
            Assert.Equal(TestConstants.ValidPrice, createdCourse.Price);
            Assert.Equal(TestConstants.ValidLocation, createdCourse.Location);
            Assert.Equal(TestConstants.ValidIsActive, createdCourse.IsActive);
        }




    }
}

