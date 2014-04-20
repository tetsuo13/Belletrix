using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    public class NoteController : Controller
    {
        public ActionResult List(int studentId)
        {
            IList<StudentBaseModel> students = StudentModel.GetStudents(studentId).ToList();

            if (students.Count == 0)
            {
                return HttpNotFound();
            }

            Analytics.TrackPageView(Request, "Note List", (Session["User"] as UserModel).Login);
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
