﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Online_Learning_Platform.Data;
using Online_Learning_Platform.Models;
using System;

namespace Online_Learning_Platform.Controllers
{
    public class LessonController : Controller
    {

        private AppDbContext db;
        public LessonController(AppDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        [NonAction]
        // save an image in the lesson folder and return the path
        private string SaveImage(IFormFile img)
        {
            // Generate a unique file name to prevent overwriting
            var fileName = Path.GetFileName(img.FileName);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

            // Set the correct path to save the image
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", "Lesson");

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
            return "/Image/Lesson/" + uniqueFileName;
        }

        //save a video in the lesson folder and return the path
        [NonAction]
        private string SaveVideo(IFormFile video)
        {
            // Generate a unique file name to prevent overwriting
            var fileName = Path.GetFileName(video.FileName);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

            // Set the correct path to save the video in wwwroot/Video/Lesson folder
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Video", "Lesson");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                video.CopyTo(stream);
            }

            // Return the relative path to the saved video
            return "/Video/Lesson/" + uniqueFileName;
        }


        // a function to create a lesson in a course
        [HttpPost]
        public IActionResult CreateLesson(IFormCollection f, IFormFile img, IFormFile video)
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
                )
            {
                return RedirectToAction("Index", "Home");
            }
            int courseId = int.Parse(f["courseId"]);

            var course = db.UserCourses.Where(a => a.UserId.ToString() == HttpContext.Session.GetString("Id") && a.CourseId == courseId).FirstOrDefault();
            if (course != null)
            {
                var c = db.Courses.Where(a => a.CourseId == courseId).FirstOrDefault();
                var lesson = new Lesson();
                lesson.LessonName = f["LessonName"];
                lesson.LessonDescription = f["LessonDes"];

                // Check if the image file is provided
                if (img != null && img.Length > 0)
                {
                    // Store the relative path in the Course model
                    lesson.LessonImage = SaveImage(img);
                }
                else
                {
                    lesson.LessonImage = "/img/noCourseImg.png";
                }

                if (video != null && video.Length > 0)
                {
                    lesson.LessonVideo = SaveVideo(video);
                }
                else
                {
                    lesson.LessonVideo = "";
                }
                db.Lessons.Add(lesson);
                if (c.Lessons == null)
                {
                    c.Lessons = new List<Lesson>();
                }
                lesson.Course = c;
                lesson.CourseId = c.CourseId;
                c.Lessons.Add(lesson);
                db.Courses.Update(c);
                db.SaveChanges();
                return RedirectToAction("ViewCourse", "Course", new {id = c.CourseId});

            }
            return RedirectToAction("Index", "Home");
        }

        // a function to delete a lesson
        [HttpPost]
        public IActionResult DeleteLesson(int id)
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") == "user"
                )
            {
                return RedirectToAction("Index", "Home");
            }
            var lesson = db.Lessons.Where(l => l.LessonId == id).FirstOrDefault();
            if (lesson == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userCourse = db.UserCourses.Where(c => c.CourseId == lesson.CourseId).FirstOrDefault();

            if (userCourse == null || userCourse.UserId.ToString() != HttpContext.Session.GetString("Id"))
            {
                return RedirectToAction("Index", "Home");
            }

            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("ViewCourse", "Course", new { id = userCourse.CourseId });

        }

        // edit lesson data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditData(IFormCollection f, IFormFile img, IFormFile video)
        {
            if (
                  HttpContext.Session.GetString("Id") == null
                  || HttpContext.Session.GetString("Type") == null
                  || HttpContext.Session.GetString("Type") == "user"
                  )
            {
                return RedirectToAction("Index", "Home");
            }
            
            var lesson = db.Lessons.Where(l => l.LessonId.ToString() == f["id"]).FirstOrDefault();
            if (lesson == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userCourse = db.UserCourses.Where(c => c.CourseId == lesson.CourseId).FirstOrDefault();

            if (userCourse == null || userCourse.UserId.ToString() != HttpContext.Session.GetString("Id"))
            {
                return RedirectToAction("Index", "Home");
            }


            if (f["lessonName"] != "")
            {
                lesson.LessonName = f["lessonName"];
            }

            if (f["lessonDes"] != "")
            {
                lesson.LessonDescription = f["lessonDes"];
            }

            lesson.LessonUpdatedAt = DateTime.Now;

            if (img != null)
            {
                // Check if the image file exists before deleting
                if (System.IO.File.Exists(lesson.LessonImage))
                {
                    // Delete the old image file
                    System.IO.File.Delete(lesson.LessonImage);
                }

                // edit the img
                lesson.LessonImage = SaveImage(img);
            }

            if (video != null)
            {
                // Check if the video file exists before deleting
                if (System.IO.File.Exists(lesson.LessonVideo))
                {
                    // Delete the old image file
                    System.IO.File.Delete(lesson.LessonVideo);
                }

                // edit the video
                lesson.LessonVideo = SaveVideo(video);
            }

            db.Lessons.Update(lesson);
            db.SaveChanges();
            return RedirectToAction("ViewCourse", "Course", new { id = userCourse.CourseId });
        }

    }
}
