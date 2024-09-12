using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public bool UserIdCheck(User u)
        {
            var testID = db.Users.Where(a => a.UserId == u.UserId).FirstOrDefault();
            if (testID != null)
            {
                return false;
            }
            return true;
        }


        //check if the email exist or not
        //if it exist return true, else false
        [NonAction]
        public bool UserEmailCheck(User u, out User user)
        {
            var testEmail = db.Users.Where(a => a.UserEmail == u.UserEmail).FirstOrDefault();
            if (testEmail == null)
            {
                user = null;
                return false;
            }
            user = testEmail;
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
        public bool ConfUser(User u, IFormCollection f, IFormFile i, out int wentWrong)
        {

            // test id doesn't exist before in the database
            if (!UserIdCheck(u))
            {
                wentWrong = 0;
                return false;
            }


            // test email doesn't exist before in the database
            if (UserEmailCheck(u, out User user))
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
                    if (ConfUser(u, f, i, out wentWrong))
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
                        switch(wentWrong)
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
                else
                {
                    //working on
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
                    User user;
                    if (UserEmailCheck(u, out user))
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

        [NonAction]
        public IActionResult SignOut()
        {
            //remove session data

            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("Type");
            
            //redirect the user to the home page
            return View("Index", "Home");
        }
    }
}
