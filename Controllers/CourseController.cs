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

            ViewData["Lesson"] = course.Lessons;
            ViewData["Message"] = msg;
            return View(course);
        }

        
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
            //if (courseImage != null && courseImage.Length > 0)
            //{
            //    // Process the uploaded image (store it in a directory or database)
            //    var filePath = Path.Combine("wwwroot/images/courses", courseImage.FileName);
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        courseImage.CopyTo(stream);
            //    }

            //    // Save the file name or path to the course record
            //    course.CourseImage = courseImage.FileName;
            //}

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

    }
}
