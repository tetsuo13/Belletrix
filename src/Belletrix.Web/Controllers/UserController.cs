﻿using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Belletrix.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public static string ActivePageName = "user";

        private readonly IUserService UserService;

        public UserController(IUserService userService)
        {
            UserService = userService;

            ViewBag.ActivePage = ActivePageName;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            await Analytics.TrackPageView(Request, "Belletrix", null);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            string mainError = "Invalid login credentials";

            if (ModelState.IsValid)
            {
                try
                {
                    UserModel user = await UserService.GetUser(model.UserName);

                    if (user.IsActive)
                    {
                        if (string.IsNullOrEmpty(user.PasswordHash))
                        {
                            string correctHash = user.PasswordIterations + ":" + user.PasswordSalt + ":" + user.Password;

                            if (PasswordHash.ValidatePassword(model.Password, correctHash))
                            {
                                return RedirectToAction("MigratePassword", new { id = user.Id });
                            }
                        }
                        else if (new PasswordHasher().VerifyHashedPassword(user.PasswordHash, model.Password) != PasswordVerificationResult.Failed)
                        {
                            UserService.UpdateLastLogin(model.UserName);
                            FormsAuthentication.SetAuthCookie(model.UserName, true);
                            Session["User"] = await UserService.GetUser(model.UserName);

                            if (Url.IsLocalUrl(returnUrl) &&
                                returnUrl.Length > 1 &&
                                returnUrl.StartsWith("/") &&
                                !returnUrl.StartsWith("//") &&
                                !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception e)
                {
                    mainError = e.Message;
                    MvcApplication.LogException(e);
                }
            }

            await Analytics.TrackPageView(Request, "Belletrix", null);
            ModelState.AddModelError("", mainError);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult MigratePassword(int id)
        {
            return View(new MigratePasswordViewModel { Id = id });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MigratePassword(MigratePasswordViewModel model)
        {
            string errorMessage = "There was a problem migrating";

            if (ModelState.IsValid)
            {
                UserModel user = await UserService.GetUser(model.Id);

                if (user.IsActive)
                {
                    string correctHash = user.PasswordIterations + ":" + user.PasswordSalt + ":" + user.Password;

                    if (!PasswordHash.ValidatePassword(model.CurrentPassword, correctHash))
                    {
                        errorMessage = "Current password is incorrect";
                    }
                    else
                    {
                        user.PasswordHash = new PasswordHasher().HashPassword(model.NewPassword);
                        UserService.UpdateUser(user, user.IsAdmin);
                        return RedirectToAction("Login", "User");
                    }
                }
            }

            await Analytics.TrackPageView(Request, "Belletrix", null);
            ModelState.AddModelError("", errorMessage);
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        new public async Task<ActionResult> Profile()
        {
            await Analytics.TrackPageView(Request, "Profile", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Profile";
            return View(Session["User"]);
        }

        /// <summary>
        /// Edit your own profile.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        new public async Task<ActionResult> Profile(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel currentUser = Session["User"] as UserModel;
                model.Id = currentUser.Id;

                if (!String.IsNullOrEmpty(model.PasswordHash))
                {
                    currentUser.PasswordHash = new PasswordHasher().HashPassword(model.Password);
                }

                UserService.UpdateUser(currentUser, currentUser.IsAdmin);

                Session["User"] = model;
                return RedirectToAction("Index", "Home");
            }

            await Analytics.TrackPageView(Request, "Profile", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Profile";
            return View("Profile", model);
        }

        public async Task<ActionResult> Add()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            await Analytics.TrackPageView(Request, "Add User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Add";
            return View("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(UserModel model)
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model != null &&
                (String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.ConfirmPassword)))
            {
                ModelState.AddModelError("Password", "Please supply and confirm a password");
            }
            else if (model != null && model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Passwords do not match");
            }

            if (ModelState.IsValid)
            {
                model.PasswordHash = new PasswordHasher().HashPassword(model.Password);

                UserService.InsertUser(model);
                return RedirectToAction("List");
            }

            await Analytics.TrackPageView(Request, "Add User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Add";
            return View("Profile", model);
        }

        public async Task<ActionResult> Edit(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserModel user = (await UserService.GetUsers(username)).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }

            await Analytics.TrackPageView(Request, "Edit User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Edit";
            return View("Profile", user);
        }

        /// <summary>
        /// Edit someone else's profile.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel currentUser = Session["User"] as UserModel;
                UserService.UpdateUser(currentUser, currentUser.IsAdmin);
                return RedirectToAction("Index", "Home");
            }

            await Analytics.TrackPageView(Request, "Edit User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Edit";
            return View("Profile", model);
        }

        public async Task<ActionResult> List()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            await Analytics.TrackPageView(Request, "User List", (Session["User"] as UserModel).Login);
            return View(await UserService.GetUsers());
        }
    }
}
