using Bennett.AbroadAdvisor.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        public StudentController()
        {
            ViewBag.ActivePage = "students";
        }

        public ActionResult List()
        {
            return View(StudentModel.GetStudents());
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<StudentModel> student = StudentModel.GetStudents(id);

            if (student.Count == 0)
            {
                return HttpNotFound();
            }

            PrepareDropDowns();
            return View("Add", student[0]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                model.SaveChanges((Session["User"] as UserModel).Id);
                return RedirectToAction("List");
            }

            PrepareDropDowns();
            return View(model);
        }

        public ActionResult Add()
        {
            PrepareDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save((Session["User"] as UserModel).Id);
                return RedirectToAction("List");
            }

            ModelState.AddModelError("", "There were errors adding the student.");
            PrepareDropDowns();
            return View(model);
        }

        private void PrepareDropDowns()
        {
            ViewBag.Countries = new SelectList(CountryModel.GetCountries(), "Id", "Name");
            ViewBag.DormHalls = new SelectList(DormModel.GetDorms(), "Id", "HallName");
            ViewBag.Majors = new SelectList(MajorsModel.GetMajors(), "Id", "Name");
        }
    }
}
