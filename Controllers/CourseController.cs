using Microsoft.AspNetCore.Mvc;

namespace Online_Learning_Platform.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
