using Microsoft.AspNetCore.Mvc;
using Online_Learning_Platform.Data;

namespace Online_Learning_Platform.Controllers
{
    public class CourseProviderController : Controller
    {
        private AppDbContext db;
        public CourseProviderController(AppDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.GetString("Type") == "user")
            {
                //send the request to the user controller
                return RedirectToAction("Profile", "User");
            }
            else if (HttpContext.Session.GetString("Type") == "courseProvider")
            {
                //show the course provider profile with the data
                return View(db.CourseProviders.Where(a => a.CourseProviderId == HttpContext.Session.GetString("Id")));
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
