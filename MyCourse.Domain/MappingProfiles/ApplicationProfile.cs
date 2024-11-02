using AutoMapper;
using MyCourse.Domain.DTOs.ApplicationDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Enums;
using MyCourse.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.MappingProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile() 
        {

            CreateMap<Application, ApplicationDetailDto>()
                            .ForMember(dest => dest.StatusDisplayName, opt => opt.MapFrom(src => src.Status.GetDisplayName()));

            CreateMap<ApplicationUpdateDto, Application>();

            CreateMap<ApplicationRegistrationDto, Application>()
                    .ForMember(dest => dest.ApplicationDate, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ApplicationStatusType.Pending))
                    .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Course, opt => opt.Ignore())
                    .ForMember(dest => dest.ApplicationMedias, opt => opt.Ignore());
        }
    }
}
