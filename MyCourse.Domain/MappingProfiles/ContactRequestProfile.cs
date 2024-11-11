using AutoMapper;
using MyCourse.Domain.DTOs.ContactRequestDtos;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.MappingProfiles
{
    public class ContactRequestProfile : Profile
    {
        public ContactRequestProfile()
        {
            // Entity zu DTO
            CreateMap<ContactRequest, ContactRequestDto>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(dest => dest.AnswerDate, opt => opt.MapFrom(src => src.AnswerDate))
                .ForMember(dest => dest.AnswerMessage, opt => opt.MapFrom(src => src.AnswerMessage));

            // DTO zu Entity
            CreateMap<ContactRequestCreateDto, ContactRequest>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.Now));

            // DTO zu Entity für Antworten
            CreateMap<ContactRequestRespondDto, ContactRequest>()
                .ForMember(dest => dest.AnswerMessage, opt => opt.MapFrom(src => src.AnswerMessage))
                .ForMember(dest => dest.IsAnswered, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.AnswerDate, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
