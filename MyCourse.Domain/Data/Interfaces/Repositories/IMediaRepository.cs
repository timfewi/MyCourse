using Microsoft.AspNetCore.Http;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Repositories
{
    public interface IMediaRepository : IRepository<Media>
    {
        Task AddCourseMediaAsync(int courseId, int mediaId);
        Task AddBlogPostMediaAsync(int blogPostId, int mediaId);
        void RemoveCourseMedia(CourseMedia courseMedia);
        void RemoveBlogPostMedia(BlogPostMedia blogPostMedia);
        Task<Media> SaveImageAsync(IFormFile file, int courseId);
        void DeleteImage(Media media);
    }
}
