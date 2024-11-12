using MyCourse.Domain.DTOs.BlogPostDtos;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard.BlogPost
{
    public class BlogPostManageViewModel
    {
        public IEnumerable<BlogPostListDto> BlogPosts { get; set; } = new List<BlogPostListDto>();
    }
}
