using Bennett.AbroadAdvisor.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    public class NoteController : Controller
    {
        public ActionResult List(int studentId)
        {
            List<StudentModel> students = StudentModel.GetStudents(studentId);

            if (students.Count == 0)
            {
                return HttpNotFound();
            }

            ViewBag.Student = students[0];

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
