using AutoMapper;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseListDto>();
            CreateMap<Course, CourseDetailDto>()
                .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count));

            CreateMap<CourseCreateDto, Course>();
            CreateMap<CourseUpdateDto, Course>();

            CreateMap<Course, CourseUpdateDto>();
        }
    }
}
