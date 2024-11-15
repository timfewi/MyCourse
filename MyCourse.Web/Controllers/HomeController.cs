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
               new Feature
            {
                IconClass = "fas fa-users",
                Title = "Gemeinschaft und Vernetzung",
                Description = "Verbinde dich mit Gleichgesinnten und teile kreative Momente in einer unterstützenden Umgebung."
            },
            new Feature
            {
                IconClass = "fas fa-chalkboard-teacher",
                Title = "Professionelle Anleitung",
                Description = "Lerne von erfahrenen Künstlern, die dich Schritt für Schritt durch den Malprozess führen."
            },
            new Feature
            {
                IconClass = "fas fa-palette",
                Title = "Kreative Freiheit",
                Description = "Entfalte deine Kreativität ohne Einschränkungen und entdecke deinen eigenen Malstil."
            },
            new Feature
            {
                IconClass = "fas fa-spa",
                Title = "Entspannende Atmosphäre",
                Description = "Genieße eine stressfreie Umgebung, in der du deine Gedanken schweifen lassen und dich künstlerisch ausdrücken kannst."
            },
            new Feature
            {
                IconClass = "fas fa-paint-brush",
                Title = "Hochwertige Materialien",
                Description = "Verwende erstklassige Malutensilien, die deine künstlerische Erfahrung bereichern."
            },
            new Feature
            {
                IconClass = "fas fa-calendar-alt",
                Title = "Flexible Kurszeiten",
                Description = "Wähle aus verschiedenen Kurszeiten, die sich deinem persönlichen Zeitplan anpassen."
            }
            };

            var allPublishedBlogs = await _blogPostService.GetPublishedBlogPostsAsync();

            // IMPL.
            //var top4Blogs = allPublishedBlogs.Take(4).Select(b => new BlogPostHomeViewModel
            //{
            //    Id = b.Id,
            //    Title = b.Title,
            //    ShortDescription = b.ShortDescription,
            //    ThumbnailUrl = b.ThumbnailUrl,
            //    DateCreated = b.DateCreated,
            //    Tags = b.Tags,
            //}).ToList();
            
            var blogViewModel = allPublishedBlogs.Select(b => new BlogPostHomeViewModel
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
                //Blogs = top4Blogs,
                Blogs = blogViewModel,
                TotalPublishedBlogPosts = allPublishedBlogs.Count(),
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CookiePolicy()
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
