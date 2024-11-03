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
    }
}
