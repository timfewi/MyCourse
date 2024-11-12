using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Web.Models.BlogPostModels;

namespace MyCourse.Web.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostController> _logger;

        public BlogPostController(
            IBlogPostService blogPostService,
            ILogger<BlogPostController> logger
            )
        {
            _blogPostService = blogPostService;
            _logger = logger;
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var blogPostDto = await _blogPostService.GetBlogPostDetailAsync(id);
                if (blogPostDto == null || !blogPostDto.IsPublished)
                {
                    return NotFound();
                }

                var viewModel = new BlogPostDetailViewModel
                {
                    Id = blogPostDto.Id,
                    Title = blogPostDto.Title,
                    Description = blogPostDto.Description, // HTML-Inhalt
                    IsPublished = blogPostDto.IsPublished,
                    DateCreated = blogPostDto.DateCreated,
                    PublishedDate = blogPostDto.DateCreated,
                    Tags = blogPostDto.Tags,
                    ThumbnailUrl = blogPostDto.Medias.FirstOrDefault()?.Url ?? string.Empty,
                    Medias = blogPostDto.Medias.Select(m => new BlogPostMediaDetailViewModel
                    {
                        Url = m.Url,
                        Caption = string.Empty,
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des BlogPosts mit ID {id}.", id);
                TempData["ErrorMessage"] = "Fehler beim Laden des BlogPosts.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
