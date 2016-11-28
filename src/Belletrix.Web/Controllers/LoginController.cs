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

        public ActionResult Index(string returnUrl)
        {
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

                    if (user != null &&
                        user.IsActive &&
                        new PasswordHasher().VerifyHashedPassword(user.Password, model.Password) != PasswordVerificationResult.Failed)
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
                catch (Exception e)
                {
                    mainError = e.Message;
                    MvcApplication.LogException(e);
                }
            }

            ModelState.AddModelError("", mainError);
            return View(model);
        }
    }
}
