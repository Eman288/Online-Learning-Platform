﻿using Microsoft.AspNetCore.Mvc;
using Online_Learning_Platform.Models;
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
                return View(db.Users.Where(a => a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault());
            }
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public IActionResult SignOut()
        {
            //remove session data

            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("Type");

            //redirect the Course Provider to the home page
            return View("Index", "Home");
        }

        // a function to display the course provider dashboard
        [HttpGet]
        public IActionResult DashBoard()
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (HttpContext.Session.GetString("Type") == "user")
            {
                return RedirectToAction("DashBoard", "User"); ;
            }
            else
            {
                return View(new Course());
            }
        }
    
        
        
    }
}
