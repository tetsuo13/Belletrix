﻿using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using System;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    public class NoteController : Controller
    {
        public ActionResult List(int studentId)
        {
            try
            {
                ViewBag.Student = StudentModel.GetStudent(studentId);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            Analytics.TrackPageView(Request, "Note List", (Session["User"] as UserModel).Login);
            return View(NoteModel.GetNotes(studentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Add(NoteModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save((Session["User"] as UserModel).Id);
            }
        }
    }
}
