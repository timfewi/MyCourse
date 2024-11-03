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

namespace MyCourse.Domain.Data.Repositories.MediaRepositories
{
    [Injectable(ServiceLifetime.Scoped)]
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        public MediaRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task AddCourseMediaAsync(int courseId, int mediaId)
        {
            var exists = await _dbContext.CourseMedias
                .AnyAsync(cm => cm.CourseId == courseId && cm.MediaId == mediaId);

            if (!exists)
            {
                var courseMedia = new CourseMedia
                {
                    CourseId = courseId,
                    MediaId = mediaId
                };

                await _dbContext.CourseMedias.AddAsync(courseMedia);
            }
        }
    }
}
