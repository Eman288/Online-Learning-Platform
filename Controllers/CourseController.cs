using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Online_Learning_Platform.Data;
using Online_Learning_Platform.Models;
using System;

namespace Online_Learning_Platform.Controllers
{
    public class CourseController : Controller
    {

        private AppDbContext db;
        public CourseController(AppDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        /* a function to create a course 
         * if the current logged in isn't a course provider, the function will redirect to home page
         * other wise the function will  create the course
         */

        [HttpPost]
        public IActionResult CreateCourse(Course c, IFormCollection f, IFormFile courseImage)
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
                )
            {
                return RedirectToAction("Index", "Home");
            }

            // create the course

            Course NewCourse = new Course();


            // set the course's data
            NewCourse.CourseName = c.CourseName;
            NewCourse.CourseDescription = f["courseDes"];

            // Check if the image file is provided
            if (courseImage != null && courseImage.Length > 0)
            {
                // Generate a unique file name to prevent overwriting
                var fileName = Path.GetFileName(courseImage.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                // Set the correct path to save the image in wwwroot/Image/Course folder
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", "Course");

                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    courseImage.CopyTo(stream);
                }

                // Store the relative path in the Course model (use forward slashes for URLs)
                NewCourse.CourseImage = "/Image/Course/" + uniqueFileName;
            }
            else
            {
                NewCourse.CourseImage = "/img/noCourseImg.png";
            }

            // add the course into the database

            NewCourse.UserCourses = new List<UserCourse>();
            db.Courses.Add(NewCourse);
            db.SaveChanges();

            if (NewCourse.CourseId == 0)
            {
                // Handle case where the course was not saved correctly
                ModelState.AddModelError("", "Course could not be created.");
                return View(c);
            }


            // add the relationship data into the bridge

            var UserCourse = new UserCourse();

            var user = db.Users.Where(a => a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault();

            if (user == null)
            {
                // Handle case when the user is not found in the database
                return RedirectToAction("Index", "Home");
            }

            UserCourse.UserId = user.UserId;
            UserCourse.User = user;



            UserCourse.CourseId = NewCourse.CourseId;
            UserCourse.Course = NewCourse;
            

            db.UserCourses.Add(UserCourse);
            db.SaveChanges();

            if (UserCourse.Course == null)
            {
                return View("Index", "Home");
            }

            return RedirectToAction("ViewMyCourses");
        }



        /* a function to show the created courses by a course provider
         */
        [HttpGet]
        public IActionResult ViewMyCourses()
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
                )
            {
                return RedirectToAction("Index", "Home");
            }

            var id = int.Parse(HttpContext.Session.GetString("Id"));

            var myCourses = from Course in db.Courses
                            join UserCourse in db.UserCourses
                            on Course.CourseId equals UserCourse.CourseId
                            where UserCourse.UserId == id
                            select Course;

            ViewData["courses"] = myCourses.ToList();
            return View();
        }


        /*
         * a function to show the specific course
         */

        [HttpGet]
        public IActionResult ViewCourse(int id)
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
                )
            {
                return RedirectToAction("Index", "Home");
            }
            int msg;
            if (ViewData["Message"] != null)
            {
                msg = (int)ViewData["Message"];
            }
            else
            {
                msg = 1;
            }
            var course = db.Courses.Where(c => c.CourseId == id).FirstOrDefault();

            var trurProvider = db.UserCourses.Where(a => a.CourseId == id && a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault();

            if (course == null || trurProvider == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var lessons = db.Lessons.Where(a => a.CourseId == id).ToList();

            ViewData["Lessons"] = lessons;
            ViewData["Message"] = msg;
            return View(course);
        }


        // a function to save an img for a course
        [NonAction]
        private string SaveImage(IFormFile img)
        {
            // Generate a unique file name to prevent overwriting
            var fileName = Path.GetFileName(img.FileName);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

            // Set the correct path to save the image
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", "Course");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                img.CopyTo(stream);
            }

            // Return the relative path to the saved image
            return "/Image/Course/" + uniqueFileName;
        }

        // edit course data

        [HttpPost]
        public IActionResult EditData(IFormCollection f, IFormFile courseImage)
        {
            // Check if user is authorized to edit
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
                )
            {
                return RedirectToAction("Index", "Home");
            }


            if (string.IsNullOrEmpty(f["id"]) || !int.TryParse(f["id"], out int courseId))
            {
                // Handle invalid or missing ID
                ModelState.AddModelError("id", "Invalid course ID.");
                return View(); // Optionally return the same view with validation errors
            }


            // Check if user is authorized for this course
            var isCourseProvider = db.UserCourses
                .Where(a => a.CourseId == courseId && a.UserId.ToString() == HttpContext.Session.GetString("Id"))
                .FirstOrDefault();

            if (isCourseProvider == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var course = db.Courses.Where(a => a.CourseId == courseId).FirstOrDefault();

            if (course == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Update course details
            if (!string.IsNullOrEmpty(f["courseName"]))
            {
                course.CourseName = f["courseName"];
            }

            if (!string.IsNullOrEmpty(f["courseDes"]))
            {
                course.CourseDescription = f["courseDes"];
            }

            //// Handle image upload
            
            if (courseImage != null)
            {
                // Check if the image file exists before deleting
                if (System.IO.File.Exists(course.CourseImage))
                {
                    // Delete the old image file
                    System.IO.File.Delete(course.CourseImage);
                }
                course.CourseImage  = SaveImage(courseImage);
            }

            course.CourseUpdatedAt = DateTime.Now;

            db.Courses.Update(course);
            isCourseProvider.Course = course;
            db.UserCourses.Update(isCourseProvider);
            db.SaveChanges();

            return RedirectToAction("ViewCourse", new { id = courseId });
        }


        [HttpPost]
        public IActionResult SetCourseIdToDelete(int courseId)
        {
            HttpContext.Session.SetString("courseIdToDelete", courseId.ToString());
            return Ok();
        }

        //delete a course after getting the permission
        
        [HttpPost]
        public IActionResult DeleteCourse(int Id)
        {
            // Logging for debugging
            if (Id <= 0)
            {
                // Log or handle the case where the ID is invalid
                return BadRequest("Invalid course ID.");
            }

            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
            )
            {
                return RedirectToAction("Index", "Home");
            }

            var course = db.Courses.FirstOrDefault(a => a.CourseId == Id);

            if (course == null)
            {
                // Log that the course was not found
                return NotFound("Course not found.");
            }

            var userCourses = db.UserCourses.Where(a => a.CourseId == Id);

            if (userCourses.Count() > 0)
            {
                db.UserCourses.Remove(userCourses.FirstOrDefault());
            }

            db.Courses.Remove(course);
            db.SaveChanges();

            return RedirectToAction("ViewMyCourses");
        }


        // show all the courses in the main course page
        [HttpGet]
        public IActionResult ShowAllCourses(int page)
        {
            if (page <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // page => 1
            // courses from row 0 to 11
            // page => 2
            // courses from  row 12 to 23 and so on every 12 rows in each page
            var records = db.Courses
                       .OrderBy(c => c.CourseId)  // Assuming CourseId is the column to order by
                       .Skip((page - 1) * 12)  //Skip (a-1) rows (a is 1-based, LINQ is 0-based)
                       .Take(12)  //// Take 12 rows from 
                       .ToList();

            ViewData["courses"] = records;
            return View();
        }

        // show the course in the user view and the not signed user view
        [HttpGet]
        public IActionResult ViewUserCourse(int id)
        {
            var course = db.UserCourses.Where(a => a.CourseId == id).FirstOrDefault();
            if (course == null)
            {
                return RedirectToAction("ShowAllCourses", 1);
            }
            if (HttpContext.Session.GetString("Id") != "null" && HttpContext.Session.GetString("Id") == course.UserId.ToString())
            {
                // if it is the creator of the course, return the viewCourse view for the coure provider
                return RedirectToAction("ViewCourse", "Course", new {id = id});
            }
            var c = db.Courses.Where(c => c.CourseId == id).FirstOrDefault();
            var lessons = db.Lessons.Where(a => a.CourseId == id).ToList();
            int check = 1;
            ViewData["Lessons"] = lessons;
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") != "user") 
            {
                check = 0;
            }
            else
            {
                if (db.UserCourses.Where(a => a.CourseId == id && a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault() != null)
                {
                    check = 1;
                }
                else
                {
                    check = 0;
                }
            }
            ViewData["check"] = check;
            return View(c);
        }

        [HttpGet("Course/BookCourse/{courseId}")]
        // a function to book a course if it was a user
        public IActionResult BookCourse(int courseId)
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") != "user"
            )
            {
                return RedirectToAction("Index", "Home");
            }

            var user = db.Users.Where(u => u.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault();
            var course = db.Courses.Where(c => c.CourseId == courseId).FirstOrDefault();

            var usercourse = new UserCourse();

            usercourse.Course = course;
            usercourse.User = user;
            usercourse.UserId = user.UserId;
            usercourse.CourseId = course.CourseId;


            db.UserCourses.Add(usercourse);
            user.UserCourses.Add(usercourse);
            course.UserCourses.Add(usercourse);
            db.Users.Update(user);
            db.Courses.Update(course);

            db.SaveChanges();


            return RedirectToAction("ViewUserCourse", new { id = courseId });
        }

    }
}
