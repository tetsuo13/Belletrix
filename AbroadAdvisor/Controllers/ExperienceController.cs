using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class ExperienceController : Controller
    {
        public ExperienceController()
        {
            ViewBag.ActivePage = "experiences";
        }

        public ActionResult List()
        {
            Analytics.TrackPageView(Request, "Active Experiences", (Session["User"] as UserModel).Login);
            
            ViewBag.Countries = CountryModel.GetCountries();
            ViewBag.Semesters = StudentStudyAbroadWishlistModel.GetPeriods();
            ViewBag.Programs = ProgramModel.GetPrograms();
            ViewBag.ProgramTypes = ProgramTypeModel.GetProgramTypes();

            return View(StudyAbroadModel.GetAll());
        }

        public ActionResult Add(int studentId)
        {
            Analytics.TrackPageView(Request, "Add Experience", (Session["User"] as UserModel).Login);

            try
            {
                PrepareStudent(studentId);
            }
            catch (Exception)
            {
                return HttpNotFound("Invalid student");
            }

            PrepareDropDowns();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StudyAbroadModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save((Session["User"] as UserModel).Id);
                return RedirectToAction("View", "Student", new { Id = model.StudentId });
            }

            try
            {
                PrepareStudent(model.StudentId);
            }
            catch (Exception)
            {
                return HttpNotFound("Invalid student");
            }

            PrepareDropDowns();
            Analytics.TrackPageView(Request, "Add Experience", (Session["User"] as UserModel).Login);
            return View(model);
        }

        private void PrepareStudent(int studentId)
        {
            IList<StudentBaseModel> student = StudentModel.GetStudents(studentId).ToList();

            if (student.Count == 0)
            {
                throw new Exception();
            }

            ViewBag.Student = student[0];
        }

        private void PrepareDropDowns()
        {
            ViewBag.Years = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7)).Reverse());
            ViewBag.Semesters = StudentStudyAbroadWishlistModel.GetPeriods();
            ViewBag.Countries = new SelectList(CountryModel.GetCountries(), "Id", "Name");
            ViewBag.Programs = new SelectList(ProgramModel.GetPrograms(), "Id", "Name");
            ViewBag.ProgramTypes = ProgramTypeModel.GetProgramTypes();
        }
    }
}
