using MyCourse.Domain.DTOs.MediaDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Interfaces.Services
{
    public interface IMediaService
    {
        Task<int> CreateMediaAsync(MediaCreateDto mediaDto);
        Task AddMediaToCourseAsync(int courseId, MediaCreateDto mediaDto);
    }
}
