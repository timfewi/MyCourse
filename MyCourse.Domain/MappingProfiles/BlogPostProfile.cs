using AutoMapper;
using MyCourse.Domain.DTOs.BlogPostDtos.BlogPostMediaDtos;
using MyCourse.Domain.DTOs.BlogPostDtos;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCourse.Domain.DTOs.MediaDtos;

namespace MyCourse.Domain.MappingProfiles
{
    public class BlogPostProfile : Profile
    {
        public BlogPostProfile()
        {
            // Mapping for BlogPost to BlogPostListDto
            CreateMap<BlogPost, BlogPostListDto>()
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src =>
                    src.Description.Length > 100 ? src.Description.Substring(0, 100) + "..." : src.Description))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    src.BlogPostMedias.Select(bpm => bpm.Media.Url).FirstOrDefault()));

            // Mapping for BlogPost to BlogPostDetailDto
            CreateMap<BlogPost, BlogPostDetailDto>()
                .ForMember(dest => dest.Medias, opt => opt.MapFrom(src => src.BlogPostMedias));


            CreateMap<BlogPost, BlogPostEditWithImagesDto>()
     .ForMember(dest => dest.ExistingImages, opt => opt.Ignore())
     .ForMember(dest => dest.NewImages, opt => opt.Ignore());


            // Mapping for BlogPostMedia to BlogPostMediaDetailDto
            CreateMap<BlogPostMedia, BlogPostMediaDetailDto>()
                .ForMember(dest => dest.MediaId, opt => opt.MapFrom(src => src.Media.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Media.Url))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Media.FileName))
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.Media.MediaType))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.Media.ContentType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Media.Description))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.Media.FileSize))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order));

            // Mapping for BlogPostCreateDto to BlogPost
            CreateMap<BlogPostCreateDto, BlogPost>()
                .ForMember(dest => dest.BlogPostMedias, opt => opt.Ignore());

            // Mapping for BlogPostMediaCreateDto to BlogPostMedia
            CreateMap<BlogPostMediaCreateDto, BlogPostMedia>()
                .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.BlogPostId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Mapping for BlogPostMediaCreateDto to MediaCreateDto
            CreateMap<BlogPostMediaCreateDto, MediaCreateDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.MediaType))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize));

            // Mapping for BlogPostMediaCreateDto to Media
            CreateMap<BlogPostMediaCreateDto, Media>();
        }
    }
}
