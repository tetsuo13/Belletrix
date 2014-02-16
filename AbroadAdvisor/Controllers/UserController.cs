using Bennett.AbroadAdvisor.Models;
using System.Collections.Generic;
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

        new public ActionResult Profile()
        {
            return View(Session["User"]);
        }

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

            ModelState.AddModelError("", "Errors");
            return View(model);
        }

        public ActionResult Add()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

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

            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("List");
            }

            return View("Add", model);
        }

        public ActionResult Edit(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<UserModel> user = UserModel.GetUsers(username);

            if (user.Count == 0)
            {
                return HttpNotFound();
            }

            return View("Profile", user[0]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel currentUser = Session["User"] as UserModel;
                model.SaveChanges(currentUser.IsAdmin);
                return RedirectToAction("Profile");
            }

            return View("Profile", model);
        }

        public ActionResult List()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(UserModel.GetUsers());
        }
    }
}
