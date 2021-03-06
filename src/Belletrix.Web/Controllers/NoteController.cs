﻿using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly IStudentNoteService StudentNoteService;
        private readonly IStudentService StudentService;

        public NoteController(IStudentNoteService studentNoteService, IStudentService studentService)
        {
            StudentNoteService = studentNoteService;
            StudentService = studentService;
        }

        public async Task<ActionResult> List(int studentId)
        {
            try
            {
                ViewBag.Student = await StudentService.GetStudent(studentId);
            }
            catch (Exception e)
            {
                string message = string.Format("Student ID {0} not found", studentId);
                MvcApplication.LogException(new ArgumentException(message, nameof(studentId), e));
                return RedirectToAction("NotFound", "Error");
            }

            ViewBag.Notes = await StudentNoteService.GetAllNotes(studentId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Add(AddStudentNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                await StudentNoteService.InsertNote((Session["User"] as UserModel).Id, model,
                    HttpContext.Request.UserHostAddress);
            }
        }
    }
}
