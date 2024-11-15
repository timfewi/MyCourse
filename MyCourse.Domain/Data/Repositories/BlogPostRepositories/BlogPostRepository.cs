using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Repositories.BlogPostRepositories
{
    [Injectable(ServiceLifetime.Scoped)]
    public class BlogPostRepository(AppDbContext dbContext) : Repository<BlogPost>(dbContext), IBlogPostRepository
    {
        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            var blogPosts = await _dbSet
                .AsNoTracking()
                .Include(bp => bp.BlogPostMedias)
                    .ThenInclude(bpm => bpm.Media)
                .ToListAsync();

            return blogPosts;
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int blogPostId)
        {
            var blogPost = await _dbSet
                .Include(bp => bp.BlogPostMedias)
                    .ThenInclude(bpm => bpm.Media)
                .FirstOrDefaultAsync(bp => bp.Id == blogPostId);

            return blogPost;
        }

        public async Task<BlogPost?> GetBlogPostByTitleAsync(string title)
        {
            var blogPost = await _dbSet
                .AsNoTracking()
                .Where(bp => bp.Title == title)
                .Include(bp => bp.BlogPostMedias)
                    .ThenInclude(bpm => bpm.Media)
                .FirstOrDefaultAsync();

            return blogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync()
        {
            var publishedPosts = await _dbSet
                .AsNoTracking()
                .Where(bp => bp.IsPublished)
                .Include(bp => bp.BlogPostMedias)
                    .ThenInclude(bpm => bpm.Media)
                .OrderByDescending(bp => bp.DateCreated)
                .ToListAsync();
            return publishedPosts;
        }

        public IQueryable<BlogPost> GetAllBlogPostsQuery()
        {
            return _dbSet
                .Include(bp => bp.BlogPostMedias)
                    .ThenInclude(bpm => bpm.Media)
                .AsQueryable();
        }

        public IQueryable<BlogPost> GetAllPublishedPostsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(bp => bp.IsPublished)
                .Include(bp => bp.BlogPostMedias)
                    .ThenInclude(bpm => bpm.Media)
                .AsQueryable();
        }

        public async Task AddBlogPostAsync(BlogPost blogPost)
        {
            await _dbSet.AddAsync(blogPost);
        }

        public void DeleteBlogPost(BlogPost blogPost)
        {
            _dbSet.Remove(blogPost);
        }

        public void UpdateBlogPost(BlogPost blogPost)
        {
            _dbSet.Update(blogPost);  
        }

        public async Task UpdateTagsAsync(BlogPost blogPost, List<string> newTags)
        {
            if (_dbContext.Entry(blogPost).State == EntityState.Detached)
            {
                _dbContext.BlogPosts.Attach(blogPost);
            }

            blogPost.Tags.Clear();

            if (newTags != null && newTags.Any())
            {
                foreach (var tag in newTags)
                {
                    blogPost.Tags.Add(tag);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public void RemoveBlogPostMedia(BlogPostMedia blogPostMedia)
        {
            _dbContext.Remove(blogPostMedia);
        }

        public async Task LoadBlogPostMediasAsync(BlogPost blogPost)
        {
            await _dbContext.Entry(blogPost)
                .Collection(bp => bp.BlogPostMedias)
                .Query()
                .Include(bpm => bpm.Media)
                .LoadAsync();
        }

       
    }
}
