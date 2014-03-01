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

        public ActionResult View(int id)
        {
            List<StudentModel> student = StudentModel.GetStudents(id);

            if (student.Count == 0)
            {
                return HttpNotFound();
            }

            PrepareDropDowns();
            return View(student[0]);
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
            if (model.StudyAbroadCountry != null &&
                model.StudyAbroadCountry.Cast<int>().Count() == 1 &&
                model.StudyAbroadCountry.ElementAt(0) == 0)
            {
                model.StudyAbroadCountry = null;
            }

            if (model.StudyAbroadYear != null &&
                model.StudyAbroadYear.Cast<int>().Count() == 1 &&
                model.StudyAbroadYear.ElementAt(0) == 0)
            {
                model.StudyAbroadYear = null;
            }

            if (model.StudyAbroadCountry == null)
            {
                model.StudyAbroadPeriod = null;
            }

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
            ViewBag.Languages = LanguageModel.GetLanguages();

            ViewBag.EnteringYears = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 2)).Reverse());
            ViewBag.GraduatingYears = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7)).Reverse());
            ViewBag.Classifications = new SelectList(StudentClassificationModel.GetClassifications(), "Id", "Name");
            ViewBag.Semesters = StudentStudyAbroadWishlistModel.GetPeriods();
            
            IEnumerable<MajorsModel> majors = MajorsModel.GetMajors();
            ViewBag.AvailableMajors = majors;
            ViewBag.AvailableMinors = majors;
        }
    }
}
