using MyCourse.Domain.DTOs.BlogPostDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Services
{
    public interface IBlogPostService
    {
        Task<IEnumerable<BlogPostListDto>> GetAllBlogPostsAsync();
        Task<IEnumerable<BlogPostListDto>> GetPublishedBlogPostsAsync();
        Task<BlogPostDetailDto> GetBlogPostDetailAsync(int id);
        Task<BlogPostEditWithImagesDto> GetBlogPostEditDetailsWithImagesAsync(int blogPostId);
        Task<BlogPostDetailDto> CreateBlogPostAsync(BlogPostCreateDto createDto);
        Task UpdateBlogPostWithImagesAsync(BlogPostEditWithImagesDto blogPostDto);
        Task DeleteBlogPostAsync(int id);
        Task ToggleBlogPostStatusAsync(int courseId);
    }
}
