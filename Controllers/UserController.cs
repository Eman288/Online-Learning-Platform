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

        [NonAction]
        public bool UserIdCheck(User u, string type)
        {
            if (type == "user")
            {
                var testID = db.Users.Where(a => a.UserId == u.UserId).FirstOrDefault();
                if (testID != null)
                {
                    return false;
                }
                return true;
            }
            else
            {
                var testID = db.CourseProviders.Where(a => a.CourseProviderId == u.UserId).FirstOrDefault();
                if (testID != null)
                {
                    return false;
                }
                return true;
            }

        }


        //check if the email exist or not
        //if it exist return true, else false
        [NonAction]
        public bool UserEmailCheck(User u, string type)
        {
            if (type == "user")
            {
                var testEmail = db.Users.Where(a => a.UserEmail == u.UserEmail).FirstOrDefault();
                if (testEmail == null)
                {
                    return false;
                }
                return true;
            }
            else
            {
                var testEmail = db.CourseProviders.Where(a => a.CourseProviderEmail == u.UserEmail).FirstOrDefault();
                if (testEmail == null)
                {
                    return false;
                }
                return true;
            }

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
        public bool ConfUser(User u, IFormCollection f, IFormFile i, string type, out int wentWrong)
        {

            // test id doesn't exist before in the database
            if (!UserIdCheck(u, type))
            {
                wentWrong = 0;
                return false;
            }


            // test email doesn't exist before in the database
            if (UserEmailCheck(u, type))
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


        [HttpPost]
        public async Task<IActionResult> SignUp(User u, IFormCollection f, IFormFile i)
        {
            if (HttpContext.Session.GetString("Id") == null && HttpContext.Session.GetString("Type") == null)
            {
                if (f["Type"] == "user")
                {
                    int wentWrong;
                    if (ConfUser(u, f, i, "user", out wentWrong))
                    {
                        User newUser = new User();

                        newUser.UserId = u.UserId;
                        newUser.UserName = u.UserName;

                        //working on
                        //check that the email is verified for google //gonna be fun to try :)
                        newUser.UserEmail = u.UserEmail;
                        newUser.UserPassword = u.UserPassword;
                        newUser.UserBirthday = u.UserBirthday;
                        newUser.UserImage = "";
                        newUser.UserDescription = "Hello! My Name is " + newUser.UserName + " and I am Ready to Learn!!";

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
                else if (f["Type"] == "courseProvider")
                {
                    //working on

                    int wentWrong;
                    if (ConfUser(u, f, i, "courseProvider", out wentWrong))
                    {
                        CourseProvider newprovider = new CourseProvider();

                        newprovider.CourseProviderId = u.UserId;
                        newprovider.CourseProviderName = u.UserName;

                        //working on
                        //check that the email is verified for google //gonna be fun to try :)
                        newprovider.CourseProviderEmail = u.UserEmail;
                        newprovider.CourseProviderPassword = u.UserPassword;
                        newprovider.CourseProviderBirthday = u.UserBirthday;
                        newprovider.CourseProviderImage = "";
                        newprovider.CourseProviderDescription = "Hello! My Name is " + newprovider.CourseProviderName + " and I Provide Content!";

                        Console.WriteLine("data ok");

                        db.CourseProviders.Add(newprovider);
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
                if (f["Type"] == "user")
                {
                    User user = db.Users.Where(a => a.UserEmail == u.UserEmail).FirstOrDefault();
                    if (user != null)
                    {

                        if (user.UserPassword == u.UserPassword)
                        {
                            HttpContext.Session.SetString("Id", user.UserId);
                            HttpContext.Session.SetString("Type", "user");
                            return RedirectToAction("Index", "Home");
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
                else
                {
                    //working on
                    CourseProvider? provider = db.CourseProviders.Where(a => a.CourseProviderEmail == u.UserEmail).FirstOrDefault();
                    if (provider != null)
                    {

                        if (provider.CourseProviderPassword == u.UserPassword)
                        {
                            HttpContext.Session.SetString("Id", provider.CourseProviderId);
                            HttpContext.Session.SetString("Type", "user");
                            return RedirectToAction("Index", "Home");
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

            if (HttpContext.Session.GetString("Type") == "user")
            {
                //show user profile with user data
                return View(db.Users.Where(a => a.UserId == HttpContext.Session.GetString("Id")).FirstOrDefault());
            }
            else if (HttpContext.Session.GetString("Type") == "courseProvider")
            {
                //send the request to the course provider controller
                return RedirectToAction("Profile", "CourseProvider");
            }
            return RedirectToAction("Index", "Home");
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

            User user = await db.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();

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
        public async Task<IActionResult> EditData(User u, IFormCollection form)
        {
            if (HttpContext.Session.GetString("Id") == null || HttpContext.Session.GetString("Type") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("Type") == "user")
            {
                User? current = db.Users.Where(a => a.UserId == HttpContext.Session.GetString("Id")).FirstOrDefault();

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


                    // update the user
                    db.Users.Update(current);
                    await db.SaveChangesAsync();
                }


                return RedirectToAction("Profile");
            }
            else
            {
                CourseProvider? current = db.CourseProviders.Where(a => a.CourseProviderId == HttpContext.Session.GetString("Id")).FirstOrDefault();

                if (current != null)
                {
                    // update name
                    if (u.UserName != null && u.UserName != current.CourseProviderName)
                    {
                        current.CourseProviderName = u.UserName;
                    }

                    // update email
                    if (u.UserEmail != null && u.UserEmail != current.CourseProviderEmail)
                    {
                        CourseProvider? newu = db.CourseProviders.Where(a => a.CourseProviderEmail == u.UserEmail).FirstOrDefault();
                        if (newu == null)
                        {
                            current.CourseProviderEmail = u.UserEmail;
                        }
                        else
                        {
                            ModelState.AddModelError("UserEmail", "This Email Already Exist");
                            return View(u);
                        }
                    }

                    // update password
                    if (u.UserPassword != null && u.UserPassword != current.CourseProviderPassword)
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
                        current.CourseProviderPassword = u.UserPassword;
                    }


                    // update the user
                    db.CourseProviders.Update(current);
                    await db.SaveChangesAsync();
                }


                return RedirectToAction("Profile", "CourseProvider");
            }
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
            User current = db.Users.Where(a => a.UserId == HttpContext.Session.GetString("Id")).FirstOrDefault();

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
    }
}
