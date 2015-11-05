using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
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
            catch (Exception)
            {
                return HttpNotFound();
            }

            await Analytics.TrackPageView(Request, "Note List", (Session["User"] as UserModel).Login);
            return View(await StudentNoteService.GetAllNotes(studentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Add(NoteModel model)
        {
            if (ModelState.IsValid)
            {
                await StudentNoteService.InsertNote((Session["User"] as UserModel).Id, model);
                StudentNoteService.SaveChanges();
            }
        }
    }
}
