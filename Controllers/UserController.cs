﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Online_Learning_Platform.Data;
using Online_Learning_Platform.Models;
using System;

namespace Online_Learning_Platform.Controllers
{
    public class UserController : Controller
    {
        private AppDbContext db;
        public UserController(AppDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        // sign up

        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new User());
        }



        //check if the email exist or not
        //if it exist return true, else false
        [NonAction]
        public bool UserEmailCheck(User u)
        {
            var testEmail = db.Users.Where(a => a.UserEmail == u.UserEmail).FirstOrDefault();
            if (testEmail == null)
            {
                return false;
            }
            return true;
        }

        [NonAction]
        public bool UserPassCheck(User u, string conf, out int t)
        {
            t = 3;
            if (u.UserPassword.Length < 8 || u.UserPassword.Length > 12)
            {
                t = 0;
                return false;
            }
            if (u.UserPassword != conf)
            {
                t = 2;
                return false;
            }
            return true;
        }
        [NonAction]
        public bool ConfUser(User u, IFormCollection f, out int wentWrong)
        { 


            // test email doesn't exist before in the database
            if (UserEmailCheck(u))
            {
                wentWrong = 1;
                return false;

            }

            /* check the password and making sure the password is:
             *  between 8 to 12 char
             *  have small and capital and numbers
             *  the conf is equal to it
             */
            int t;
            if (!UserPassCheck(u, f["confpass"], out t))
            {
                if (t == 0)
                {
                    // the length of the password is wrong
                    wentWrong = 2;
                }
                else if (t == 1)
                {
                    // the password doesn't come in the right format
                    wentWrong = 3;
                }
                else
                {
                    // the password confirm doesn't match the password
                    wentWrong = 4;
                }

                return false;
            }


            // to test that the user is old enough
            // working on

            // set the image
            // working on

            // Add The Data
            wentWrong = 5;
            return true;
        }

        // a function to save an img for a user
        [NonAction]
        private string SaveImage(IFormFile img)
        {
            // Generate a unique file name to prevent overwriting
            var fileName = Path.GetFileName(img.FileName);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

            // Set the correct path to save the image
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image", "User");

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
            return "/Image/User/" + uniqueFileName;
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(User u, IFormCollection f, IFormFile img)
        {
            if (HttpContext.Session.GetString("Id") == null && HttpContext.Session.GetString("Type") == null)
            {
                int wentWrong;
                if (ConfUser(u, f, out wentWrong))
                {
                    User newUser = new User();

                    newUser.UserName = u.UserName;

                    //working on
                    //check that the email is verified for google //gonna be fun to try :)
                    newUser.UserEmail = u.UserEmail;
                    newUser.UserPassword = u.UserPassword;
                    newUser.UserBirthday = u.UserBirthday;
                    newUser.UserImage = "";
                    if (f["Type"] == "user")
                    {
                        newUser.UserDescription = "Hello! My Name is " + newUser.UserName + " and I am Ready to Learn!!";
                        newUser.UserType = 1;
                    }
                    else
                    {
                        newUser.UserDescription = "Hello! My Name is " + newUser.UserName + " and I Provide Content!";
                        newUser.UserType = 2;
                    }

                    if (img != null)
                    {
                        newUser.UserImage = SaveImage(img);
                    }
                    else 
                    {
                        newUser.UserImage = "/img/user.png";
                    }

                    Console.WriteLine("data ok");


                    db.Users.Add(newUser);
                    await db.SaveChangesAsync();
                    Console.WriteLine("data added");
                }
                else
                {
                    switch (wentWrong)
                    {
                        case 0:
                            ModelState.AddModelError("UserId", "This Id Exist Already, Please Write another one or sign in");
                            break;
                        case 1:
                            ModelState.AddModelError("UserEmail", "This Email is used, try another one or sign in");
                            break;
                        case 2:
                            ModelState.AddModelError("UserPassword", "The Password Must be Between 8-12 char");
                            break;
                        case 4:
                            ModelState.AddModelError("UserPassword", "The Passwords Don't Match");
                            break;
                    }
                    return View(u);
                }


            }
            return RedirectToAction("Index", "Home");
        }

        // sign in

        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult SignIn(User u, IFormCollection f)
        {
            if (HttpContext.Session.GetString("Id") == null && HttpContext.Session.GetString("Type") == null)
            {
                User user = db.Users.Where(a => a.UserEmail == u.UserEmail).FirstOrDefault();
                if (user != null)
                {
                    if (user.UserPassword == u.UserPassword)
                    {
                        if ((user.UserType == 1 && f["Type"] == "user") || (user.UserType == 2 && f["Type"] == "courseProvider"))
                        {
                            HttpContext.Session.SetString("Id", user.UserId.ToString());
                            if (f["Type"] == "user")
                            {
                                HttpContext.Session.SetString("Type", "user");
                            }
                            else
                            {
                                HttpContext.Session.SetString("Type", "courseProvider");
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("UserType", "Wrong Type Selection");
                            return View(u);
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError("UserPassword", "The Password is Wrong, Try Again");
                        return View(u);
                    }
                }
                else
                {
                    ModelState.AddModelError("UserEmail", "The Email Doesn't Exist, Sign Up");
                    return View(u);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(db.Users.Where(a => a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault());

        }

        [HttpGet]
        public IActionResult SignOut()
        {
            //remove session data

            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("Type");

            //redirect the user to the home page
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditDescription(IFormCollection form)
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = HttpContext.Session.GetString("Id");

            User user = await db.Users.Where(u => u.UserId.ToString() == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                user.UserDescription = form["description"];

                // Save the changes to the database
                db.Users.Update(user);
                await db.SaveChangesAsync();

                return RedirectToAction("Profile");
            }

            return View("Error");
        }

        [HttpGet]
        public IActionResult EditData()
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new User());
        }

        [HttpPost]
        public async Task<IActionResult> EditData(User u, IFormCollection form, IFormFile img)
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }

                User? current = db.Users.Where(a => a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault();

                if (current != null)
                {
                    // update name
                    if (u.UserName != null && u.UserName != current.UserName)
                    {
                        current.UserName = u.UserName;
                    }

                    // update email
                    if (u.UserEmail != null && u.UserEmail != current.UserEmail)
                    {
                        User newu = db.Users.Where(a => a.UserEmail == u.UserEmail).FirstOrDefault();
                        if (newu == null)
                        {
                            current.UserEmail = u.UserEmail;
                        }
                        else
                        {
                            ModelState.AddModelError("UserEmail", "This Email Already Exist");
                            return View(u);
                        }
                    }

                    // update password
                    if (u.UserPassword != null && u.UserPassword != current.UserPassword)
                    {
                        int t;
                        if (!UserPassCheck(u, form["confpass"], out t))
                        {
                            if (t == 0)
                            {
                                // the length of the password is wrong
                                ModelState.AddModelError("UserPassword", "The Password Must be Between 8-12 char");
                            }
                            else if (t == 1)
                            {
                                // the password doesn't come in the right format
                            }
                            else
                            {
                                // the password confirm doesn't match the password
                                ModelState.AddModelError("UserPassword", "The Passwords Don't Match");

                            }
                            return View(u);

                        }
                        current.UserPassword = u.UserPassword;
                    }


                    //// Handle image upload

                    if (img != null)
                    {
                        // Check if the image file exists before deleting
                        if (System.IO.File.Exists(current.UserImage))
                        {
                            // Delete the old image file
                            System.IO.File.Delete(current.UserImage);
                        }
                        current.UserImage = SaveImage(img);
                    }


                    // update the user
                    current.UserCourses = new List<UserCourse>();
                    
                    db.Users.Update(current);
                    await db.SaveChangesAsync();
                


                return RedirectToAction("Profile");
            }
            return RedirectToAction("Index", "Home");

        }

        // go back to the user profile
        [HttpGet]
        public IActionResult Cancel()
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Profile");
        }

        // Delete the user
        [HttpGet]
        public IActionResult DeleteUser()
        {
            // find user
            User current = db.Users.Where(a => a.UserId.ToString() == HttpContext.Session.GetString("Id")).FirstOrDefault();

            // remove the user data

            // remove user
            if (current != null)
            {
                db.Users.Remove(current);
                db.SaveChangesAsync();
            }

            // back to sign out
            return RedirectToAction("SignOut");
        }
        
        // go to the correct dashboard
        [HttpGet]    
        public IActionResult DashBoard()
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (HttpContext.Session.GetString("Type") == "user")
            {

                var id = int.Parse(HttpContext.Session.GetString("Id"));
                var courses = db.UserCourses
                                        .Where(a => a.UserId == id)
                                        .Include(c => c.Course)
                                        .ToList();
                foreach (var course in courses)
                {
                    var students = db.UserCourses
                             .Where(a => a.CourseId == course.CourseId && a.UserId != id)
                             .ToList();
                }
                ViewData["numCourses"] = courses.Count();
                return View();
            }
            else
            {
                return RedirectToAction("DashBoard", "CourseProvider");
            }
        }



        /* a function to show the  courses booked by user
         */
        [HttpGet]
        public IActionResult ViewMyCourses()
        {
            if (
                HttpContext.Session.GetString("Id") == null
                || HttpContext.Session.GetString("Type") == null
                || HttpContext.Session.GetString("Type") != "user"
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


    }
}
