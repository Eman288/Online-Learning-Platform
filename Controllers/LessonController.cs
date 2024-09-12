using Microsoft.AspNetCore.Mvc;

namespace Online_Learning_Platform.Controllers
{
    public class LessonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
