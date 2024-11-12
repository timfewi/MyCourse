using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Web.Models;
using MyCourse.Web.Models.ErrorModels;
using MyCourse.Web.Models.HomeModels;

namespace MyCourse.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICourseService _courseService;
        private readonly IBlogPostService _blogPostService;

        public HomeController(
            ILogger<HomeController> logger,
            ICourseService courseService,
            IBlogPostService blogPostService
            )
        {
            _logger = logger;
            _courseService = courseService;
            _blogPostService = blogPostService;
        }

        public async Task<IActionResult> Index()
        {
            var activeCourses = await _courseService.GetAllActiveCoursesAsync();

            // Map CourseListDto zu CourseHomeViewModel
            var activeCoursesHomeViewModel = activeCourses.Select(course => new CourseHomeViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description.Length > 100 ? course.Description.Substring(0, 100) + "..." : course.Description,
                CourseDate = course.CourseDate,
                CourseDuration = course.CourseDuration,
                Location = course.Location,
                Price = course.Price,
                IsActive = course.IsActive,
                DefaultImageUrl = course.DefaultImageUrl,
                HoverImageUrl = course.HoverImageUrl
            }).ToList();

            // Definiere die Features
            var features = new List<Feature>
            {
                new Feature { IconClass = "fas fa-graduation-cap", Title = "Erfahrene Dozenten", Description = "Unsere Dozenten sind Experten auf ihrem Gebiet und teilen ihr Wissen praxisnah." },
                new Feature { IconClass = "fas fa-laptop-code", Title = "Flexible Lernzeiten", Description = "Lerne in deinem eigenen Tempo und gestalte deine Lernzeiten flexibel." },
                new Feature { IconClass = "fas fa-certificate", Title = "Zertifikate", Description = "Erhalte nach Abschluss deines Kurses ein anerkanntes Zertifikat." },
                new Feature { IconClass = "fas fa-users", Title = "Community", Description = "Werde Teil unserer Community und vernetze dich mit anderen Lernenden." }
            };

            var allPublishedBlogs = await _blogPostService.GetPublishedBlogPostsAsync();
            var top4Blogs = allPublishedBlogs.Take(4).Select(b => new BlogPostHomeViewModel
            {
                Id = b.Id,
                Title = b.Title,
                ShortDescription = b.ShortDescription,
                ThumbnailUrl = b.ThumbnailUrl,
                DateCreated = b.DateCreated,
                Tags = b.Tags,
            }).ToList();

            var viewModel = new HomeViewModel
            {
                ActiveCourses = activeCoursesHomeViewModel,
                Features = features,
                Blogs = top4Blogs,
                TotalPublishedBlogPosts = allPublishedBlogs.Count(),
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
