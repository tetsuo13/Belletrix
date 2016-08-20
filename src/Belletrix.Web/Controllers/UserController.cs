using Belletrix.Core;
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

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddUserViewModel model)
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                UserModel user = new UserModel()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PasswordHash = new PasswordHasher().HashPassword(model.Password),
                    Email = model.Email,
                    Login = model.Login,
                    IsAdmin = model.IsAdmin,
                    IsActive = model.IsActive
                };

                await UserService.InsertUser(user);
                return RedirectToAction("List");
            }

            Analytics.TrackPageView(Request, "Add User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Add";
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            UserModel user = await UserService.GetUser(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            UserModel currentUser = Session["User"] as UserModel;
            ViewBag.CurrentUserIsAdmin = currentUser.IsAdmin;

            EditUserViewModel model = new EditUserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Login = user.Login,
                Email = user.Email,
                Created = user.Created,
                LastLogin = user.LastLogin,
                IsAdmin = user.IsAdmin,
                IsActive = user.IsActive
            };

            Analytics.TrackPageView(Request, "Edit User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Edit";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (ModelState.IsValid)
            {
                UserModel user = await UserService.GetUser(model.Id);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid user");
                }
                else
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    if (currentUser.IsAdmin)
                    {
                        user.IsAdmin = model.IsAdmin;
                        user.IsActive = model.IsActive;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        user.PasswordHash = new PasswordHasher().HashPassword(model.Password);
                    }

                    await UserService.Update(user);
                    return RedirectToAction("List");
                }
            }

            ViewBag.CurrentUserIsAdmin = currentUser.IsAdmin;

            Analytics.TrackPageView(Request, "Edit User", (Session["User"] as UserModel).Login);
            ViewBag.Action = "Edit";
            return View(model);
        }

        public async Task<ActionResult> List()
        {
            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Analytics.TrackPageView(Request, "User List", (Session["User"] as UserModel).Login);
            return View(await UserService.GetUsers());
        }
    }
}
