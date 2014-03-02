﻿using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using System;
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
            if (ModelState.IsValid)
            {
                try
                {
                    UserModel user = UserModel.GetUser(model.UserName);
                    string correctHash = user.PasswordIterations + ":" + user.PasswordSalt + ":" + user.Password;

                    if (PasswordHash.ValidatePassword(model.Password, correctHash))
                    {
                        UserModel.UpdateLastLogin(model.UserName);
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        Session["User"] = UserModel.GetUser(model.UserName);
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception)
                {
                }
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
                model.Id = currentUser.Id;
                model.SaveChanges(currentUser.IsAdmin);
                Session["User"] = model;
                return RedirectToAction("Index", "Home");
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
