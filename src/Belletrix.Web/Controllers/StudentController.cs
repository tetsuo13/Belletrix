using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        public static string ActivePageName = "students";

        private readonly IStudentService StudentService;
        private readonly IStudentNoteService StudentNoteService;
        private readonly IPromoService PromoService;
        private readonly IStudyAbroadService StudyAbroadService;

        public StudentController(IStudentService studentService, IStudentNoteService studentNoteService,
            IPromoService promoService, IStudyAbroadService studyAbroadService)
        {
            StudentService = studentService;
            StudentNoteService = studentNoteService;
            PromoService = promoService;
            StudyAbroadService = studyAbroadService;

            ViewBag.ActivePage = ActivePageName;
        }

        public async Task<ActionResult> List()
        {
            Analytics.TrackPageView(Request, "Student List", (Session["User"] as UserModel).Login);
            await PrepareDropDowns();

            return View(await StudentService.GetStudents());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(StudentSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await PrepareDropDowns();
                Analytics.TrackPageView(Request, "Student List", (Session["User"] as UserModel).Login);

                return View("List", await StudentService.Search(model));
            }

            return await List();
        }

        public async Task<ActionResult> View(int id)
        {
            StudentModel student = null;

            try
            {
                student = await StudentService.GetStudent(id);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            if (student == null)
            {
                return HttpNotFound();
            }

            ViewBag.StudyAbroad = await StudyAbroadService.GetAll(id);
            ViewBag.Notes = await StudentNoteService.GetAllNotes(id);
            ViewBag.ShowActionButtons = true;
            await PrepareDropDowns();
            await PrepareStudyAbroadDropDowns();
            Analytics.TrackPageView(Request, "Student", (Session["User"] as UserModel).Login);

            return View(student);
        }

        public async Task<ActionResult> ViewInline(int id)
        {
            StudentModel student;

            try
            {
                student = await StudentService.GetStudent(id);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            if (student == null)
            {
                return HttpNotFound();
            }

            ViewBag.StudyAbroad = await StudyAbroadService.GetAll(id);
            ViewBag.Notes = await StudentNoteService.GetAllNotes(id);
            ViewBag.ShowActionButtons = false;
            await PrepareDropDowns();
            await PrepareStudyAbroadDropDowns();

            return PartialView("View.NameCheck", student);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudentModel student;

            try
            {
                student = await StudentService.GetStudent(id.Value);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }

            if (student == null)
            {
                return HttpNotFound();
            }

            await PrepareDropDowns();
            Analytics.TrackPageView(Request, "Student Edit", (Session["User"] as UserModel).Login);

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                await StudentService.UpdateStudent(model, Session["User"] as UserModel,
                    HttpContext.Request.UserHostAddress);
                return RedirectToAction("List");
            }

            Analytics.TrackPageView(Request, "Student Edit", (Session["User"] as UserModel).Login);
            await PrepareDropDowns();

            return View(model);
        }

        public async Task<ActionResult> Add()
        {
            await PrepareDropDowns();
            Analytics.TrackPageView(Request, "Student Add", (Session["User"] as UserModel).Login);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                await StudentService.InsertStudent(model, Session["User"] as UserModel,
                    HttpContext.Request.UserHostAddress);
                return RedirectToAction("List");
            }

            Analytics.TrackPageView(Request, "Student Add", (Session["User"] as UserModel).Login);
            await PrepareDropDowns();

            return View(model);
        }

        public async Task<PartialViewResult> StudyAbroad(int id)
        {
            ViewBag.StudentId = id;
            await PrepareStudyAbroadDropDowns();

            return PartialView(await StudyAbroadService.GetAll(id));
        }

        private async Task PrepareDropDowns()
        {
            IEnumerable<object> years = Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7))
                .Reverse()
                .Select(i => new { Id = i, Name = i.ToString() });

            IEnumerable<CountryModel> countries = await StudentService.GetCountries();

            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            ViewBag.Languages = await StudentService.GetLanguages();

            ViewBag.EnteringYears = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 2)).Reverse());
            ViewBag.GraduatingYears = new SelectList(years, "Id", "Name");
            ViewBag.GraduatingYearsAsEnumerable = years;
            ViewBag.Classifications = new SelectList(StudentService.GetClassifications(), "Id", "Name");

            ViewBag.AvailableMajors = await StudentService.GetMajors();
            ViewBag.AvailableMinors = await StudentService.GetMinors();

            IList<object> studyYears = new List<object>(years);
            studyYears.Insert(0, new { Id = StudentService.GetStudyAbroadWishlistCatchAllYearValue(), Name = "Any Year" });

            ViewBag.StudyAbroadYears = new SelectList(studyYears, "Id", "Name");
            ViewBag.StudyAbroadSemesters = StudentService.GetStudyAbroadWishlistPeriodsWithCatchAll();

            List<GroupedSelectListItem> places = (await StudentService.GetRegions()).Select(r => new GroupedSelectListItem()
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
            ViewBag.Promos = await PromoService.AsSources();
        }

        private async Task PrepareStudyAbroadDropDowns()
        {
            ViewBag.Countries = await StudentService.GetCountries();
            ViewBag.Semesters = StudentService.GetStudyAbroadWishlistPeriods();
            ViewBag.Programs = await StudentService.GetPrograms();
            ViewBag.ProgramTypes = await StudentService.GetProgramTypes();
        }

        public async Task<PartialViewResult> NameCheck(string firstName, string lastName)
        {
            return PartialView(await StudentService.SearchByFullName(firstName, lastName));
        }
    }
}
