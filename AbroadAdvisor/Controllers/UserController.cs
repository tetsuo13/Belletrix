using Bennett.AbroadAdvisor.Models;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && model.IsValid(model.UserName, model.Password))
            {
                UserModel.UpdateLastLogin(model.UserName);
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                Session["User"] = UserModel.GetUser(model.UserName);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        public ActionResult Profile()
        {
            return View(Session["User"]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel currentUser = Session["User"] as UserModel;
                model.Id = currentUser.Id;
                UserModel.Update(model, currentUser.IsAdmin);
                Session["User"] = model;
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Errors");
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult List()
        {
            ViewBag.ActivePage = "students";
            return View(UserModel.GetUsers());
        }
    }
}
