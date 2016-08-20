using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Belletrix.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IUserService UserService;

        public LoginController(IUserService userService)
        {
            UserService = userService;
        }

        public async Task<ActionResult> Index(string returnUrl)
        {
            Analytics.TrackPageView(Request, "Belletrix", null);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {
            string mainError = "Invalid login credentials";

            if (ModelState.IsValid)
            {
                try
                {
                    UserModel user = await UserService.GetUser(model.UserName);

                    if (user.IsActive)
                    {
                        // Column for hash is a CHAR so even if the user has
                        // migrated they'll have spaces.
                        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                        {
                            string correctHash = user.PasswordIterations + ":" + user.PasswordSalt + ":" + user.PasswordHash;

                            if (PasswordHash.ValidatePassword(model.Password, correctHash))
                            {
                                return RedirectToAction("MigratePassword", new { id = user.Id });
                            }
                        }
                        else if (new PasswordHasher().VerifyHashedPassword(user.Password, model.Password) != PasswordVerificationResult.Failed)
                        {
                            await UserService.UpdateLastLogin(model.UserName);
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

            Analytics.TrackPageView(Request, "Belletrix", null);
            ModelState.AddModelError("", mainError);
            return View(model);
        }

        public ActionResult MigratePassword(int id)
        {
            return View(new MigratePasswordViewModel { Id = id });
        }

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
                        await UserService.Update(user);
                        return RedirectToAction("Index");
                    }
                }
            }

            Analytics.TrackPageView(Request, "Belletrix", null);
            ModelState.AddModelError("", errorMessage);
            return View(model);
        }
    }
}
