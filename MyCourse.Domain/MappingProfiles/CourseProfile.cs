using AutoMapper;
using MyCourse.Domain.DTOs.CourseDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
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
            // List DTO
            CreateMap<Course, CourseListDto>()
                   .ForMember(dest => dest.DefaultImageUrl, opt => opt.MapFrom(src =>
                       src.CourseMedias
                           .Select(cm => cm.Media.Url)
                           .FirstOrDefault() ?? "/images/placeholder.png"))
                   .ForMember(dest => dest.HoverImageUrl, opt => opt.MapFrom(src =>
                       src.CourseMedias
                           .Select(cm => cm.Media.Url)
                           .Skip(1)
                           .FirstOrDefault()
                       ?? src.CourseMedias
                           .Select(cm => cm.Media.Url)
                           .FirstOrDefault()
                       ?? "/images/placeholder.png"));

            // Detail DTO
            CreateMap<Course, CourseDetailDto>()
                .ForMember(dest => dest.ApplicationCount, opt => opt.MapFrom(src => src.Applications.Count))
                 .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.CourseMedias.Select(cm => cm.Media.Url).ToList()));


            // Create DTO
            CreateMap<CourseCreateDto, Course>();
            // Update DTO
            CreateMap<CourseUpdateDto, Course>();
            CreateMap<Course, CourseUpdateDto>();


        }
    }
}
