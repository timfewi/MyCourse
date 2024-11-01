using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
