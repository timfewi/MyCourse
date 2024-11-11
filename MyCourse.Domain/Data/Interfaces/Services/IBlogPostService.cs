using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
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
        Task<BlogPostDetailDto> CreateBlogPostAsync(BlogPostCreateDto createDto);
        Task<BlogPostDetailDto> UpdateBlogPostAsync(int id, BlogPostCreateDto updateDto);
        Task DeleteBlogPostAsync(int id);
    }
}
