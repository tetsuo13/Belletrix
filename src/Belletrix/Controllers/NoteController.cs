using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    public class NoteController : Controller
    {
        private readonly IStudentNoteService StudentNoteService;

        public NoteController(IStudentNoteService studentNoteService)
        {
            StudentNoteService = studentNoteService;
        }

        public async Task<ActionResult> List(int studentId)
        {
            try
            {
                ViewBag.Student = StudentModel.GetStudent(studentId);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            await Analytics.TrackPageView(Request, "Note List", (Session["User"] as UserModel).Login);
            return View(await StudentNoteService.GetAllNotes(studentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Add(NoteModel model)
        {
            if (ModelState.IsValid)
            {
                StudentNoteService.InsertNote((Session["User"] as UserModel).Id, model);
                StudentNoteService.SaveChanges();
            }
        }
    }
}
