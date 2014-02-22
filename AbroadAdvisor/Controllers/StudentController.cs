using Bennett.AbroadAdvisor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return View(StudentModel.GetStudents(null));
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<StudentModel> student = StudentModel.GetStudents(id.Value);

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
            return View("Add", model);
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

            PrepareDropDowns();
            return View(model);
        }

        private void PrepareDropDowns()
        {
            ViewBag.Countries = new SelectList(CountryModel.GetCountries(), "Id", "Name");

            ViewBag.EnteringYears = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990)).Reverse());
            ViewBag.GraduatingYears = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 6)).Reverse());
            ViewBag.Classifications = new SelectList(StudentClassificationModel.GetClassifications(), "Id", "name");
            
            List<MajorsModel> majors = MajorsModel.GetMajors();
            ViewBag.AvailableMajors = majors;
            ViewBag.AvailableMinors = majors;
        }
    }
}
