using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();
        IQueryable<BlogPost> GetAllBlogPostsQuery();
        IQueryable<BlogPost> GetAllPublishedPostsQuery();
        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync();
        Task<BlogPost?> GetBlogPostByIdAsync(int blogPostId);
        Task<BlogPost?> GetBlogPostByTitleAsync(string title);
        Task AddBlogPostAsync(BlogPost blogPost);
        void UpdateBlogPost(BlogPost blogPost);
        Task UpdateTagsAsync(BlogPost blogPost, List<string> newTags);
        void DeleteBlogPost(BlogPost blogPost);
        void RemoveBlogPostMedia(BlogPostMedia blogPostMedia);
        Task LoadBlogPostMediasAsync(BlogPost blogPost);
    }
}
