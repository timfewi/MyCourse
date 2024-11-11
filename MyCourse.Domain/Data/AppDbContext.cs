using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Courses = Set<Course>();
            Applications = Set<Application>();
            Medias = Set<Media>();
            CourseMedias = Set<CourseMedia>();
            ApplicationMedias = Set<ApplicationMedia>();
            ContactRequests = Set<ContactRequest>();
            BlogPosts = Set<BlogPost>();
            BlogPostMedias = Set<BlogPostMedia>();
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<CourseMedia> CourseMedias { get; set; }
        public DbSet<ApplicationMedia> ApplicationMedias { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogPostMedia> BlogPostMedias { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Course>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);
        }
    }
}
