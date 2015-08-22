using Belletrix.Core;
using Belletrix.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace Belletrix.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public static string ActivePageName = "user";

        public UserController()
        {
            ViewBag.ActivePage = ActivePageName;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Analytics.TrackPageView(Request, "Belletrix", null);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            string mainError = "Invalid login credentials";

            if (ModelState.IsValid)
            {
                try
                {
                    UserModel user = UserModel.GetUser(model.UserName);
                    string correctHash = user.PasswordIterations + ":" + user.PasswordSalt + ":" + user.Password;

                    if (user.IsActive && PasswordHash.ValidatePassword(model.Password, correctHash))
                    {
                        UserModel.UpdateLastLogin(model.UserName);
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        Session["User"] = UserModel.GetUser(model.UserName);

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
                catch (Exception e)
                {
                    mainError = e.Message;
                    MvcApplication.LogException(e);
                }
            }

            Analytics.TrackPageView(Request, "Belletrix", null);
            ModelState.AddModelError("", mainError);
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        new public ActionResult Profile()
        {
            Analytics.TrackPageView(Request, "Profile", (Session["User"] as UserModel).Login);
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
        new public ActionResult Profile(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel currentUser = Session["User"] as UserModel;
                model.Id = currentUser.Id;
                model.SaveChanges(currentUser.IsAdmin);
                Session["User"] = model;
                return RedirectToAction("Index", "Home");
            }

            Analytics.TrackPageView(Request, "Profile", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Profile";
            return View("Profile", model);
        }

        public ActionResult Add()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Analytics.TrackPageView(Request, "Add User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Add";
            return View("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UserModel model)
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
                model.Save();
                return RedirectToAction("List");
            }

            Analytics.TrackPageView(Request, "Add User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Add";
            return View("Profile", model);
        }

        public ActionResult Edit(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserModel user = UserModel.GetUsers(username).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }

            Analytics.TrackPageView(Request, "Edit User", (Session["User"] as UserModel).Login);
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
        public ActionResult Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel currentUser = Session["User"] as UserModel;
                model.SaveChanges(currentUser.IsAdmin);
                return RedirectToAction("Index", "Home");
            }

            Analytics.TrackPageView(Request, "Edit User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Edit";
            return View("Profile", model);
        }

        public ActionResult List()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Analytics.TrackPageView(Request, "User List", (Session["User"] as UserModel).Login);
            return View(UserModel.GetUsers());
        }
    }
}
