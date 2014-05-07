using Bennett.AbroadAdvisor.Core;
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
            Analytics.TrackPageView(Request, "Student List", (Session["User"] as UserModel).Login);
            PrepareDropDowns();
            return View(StudentModel.GetStudents());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(StudentSearchModel model)
        {
            if (ModelState.IsValid)
            {
                PrepareDropDowns();
                Analytics.TrackPageView(Request, "Student List", (Session["User"] as UserModel).Login);
                return View("List", StudentModel.Search(model));
            }

            return List();
        }

        public ActionResult View(int id)
        {
            StudentModel student;

            try
            {
                student = StudentModel.GetStudent(id);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            ViewBag.StudyAbroad = StudyAbroadModel.GetAll(id);
            ViewBag.Notes = NoteModel.GetNotes(id);
            PrepareDropDowns();
            PrepareStudyAbroadDropDowns();
            Analytics.TrackPageView(Request, "Student", (Session["User"] as UserModel).Login);
            return View(student);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudentModel student;

            try
            {
                student = StudentModel.GetStudent(id.Value);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            PrepareDropDowns();
            Analytics.TrackPageView(Request, "Student Edit", (Session["User"] as UserModel).Login);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentModel model)
        {
            CheckStudyAbroadDestinations(ref model);

            if (ModelState.IsValid)
            {
                model.SaveChanges(Session["User"] as UserModel);
                return RedirectToAction("List");
            }

            Analytics.TrackPageView(Request, "Student Edit", (Session["User"] as UserModel).Login);
            PrepareDropDowns();
            return View(model);
        }

        public ActionResult Add()
        {
            PrepareDropDowns();
            Analytics.TrackPageView(Request, "Student Add", (Session["User"] as UserModel).Login);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StudentModel model)
        {
            CheckStudyAbroadDestinations(ref model);

            if (ModelState.IsValid)
            {
                model.Save(Session["User"] as UserModel);
                return RedirectToAction("List");
            }

            Analytics.TrackPageView(Request, "Student Add", (Session["User"] as UserModel).Login);
            PrepareDropDowns();
            return View(model);
        }

        public PartialViewResult StudyAbroad(int id)
        {
            ViewBag.StudentId = id;
            ViewData.Model = StudyAbroadModel.GetAll(id);
            PrepareStudyAbroadDropDowns();
            return PartialView();
        }

        private void CheckStudyAbroadDestinations(ref StudentModel model)
        {
            if (model.StudyAbroadCountry != null &&
                model.StudyAbroadCountry.Cast<int>().Count() == 1 &&
                model.StudyAbroadCountry.ElementAt(0) == 0)
            {
                model.StudyAbroadCountry = Enumerable.Empty<int>();
            }

            if (model.StudyAbroadYear != null &&
                model.StudyAbroadYear.Cast<int>().Count() == 1 &&
                model.StudyAbroadYear.ElementAt(0) == 0)
            {
                model.StudyAbroadYear = Enumerable.Empty<int>();
            }

            if (model.StudyAbroadCountry == null)
            {
                model.StudyAbroadPeriod = Enumerable.Empty<int>();
            }
        }

        private void PrepareDropDowns()
        {
            IEnumerable<object> years = Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7))
                .Reverse()
                .Select(i => new { Id = i, Name = i.ToString() });

            IEnumerable<CountryModel> countries = CountryModel.GetCountries();

            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            ViewBag.Languages = LanguageModel.GetLanguages();

            ViewBag.GraduatingYears = new SelectList(years, "Id", "Name");
            ViewBag.GraduatingYearsAsEnumerable = years;
            ViewBag.Classifications = new SelectList(StudentClassificationModel.GetClassifications(), "Id", "Name");
            
            ViewBag.AvailableMajors = MajorsModel.GetMajors();
            ViewBag.AvailableMinors = MinorsModel.GetMinors();

            IList<object> studyYears = new List<object>(years);
            studyYears.Insert(0, new { Id = 1, Name = "Any Year" });

            ViewBag.StudyAbroadYears = new SelectList(studyYears, "Id", "Name");
            ViewBag.StudyAbroadSemesters = StudentStudyAbroadWishlistModel.GetPeriodsWithCatchAll();

            List<GroupedSelectListItem> places = CountryModel.GetRegions().Select(r => new GroupedSelectListItem()
                {
                    GroupKey = "1",
                    GroupName = "Regions",
                    Text = r.Name,
                    Value = r.Id.ToString()
                }).ToList();

            places.AddRange(countries.Select(c => new GroupedSelectListItem()
                {
                    GroupKey = "2",
                    GroupName = "Countries",
                    Text = c.Name,
                    Value = c.Id.ToString()
                }));

            ViewBag.StudyAbroadPlaces = places;
        }

        private void PrepareStudyAbroadDropDowns()
        {
            ViewBag.Countries = CountryModel.GetCountries();
            ViewBag.Semesters = StudentStudyAbroadWishlistModel.GetPeriods();
            ViewBag.Programs = ProgramModel.GetPrograms();
            ViewBag.ProgramTypes = ProgramTypeModel.GetProgramTypes();
        }
    }
}
