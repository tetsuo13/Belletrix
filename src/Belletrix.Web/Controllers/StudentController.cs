﻿using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            await PrepareViewBag();

            return View(await StudentService.GetStudents());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(StudentSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                await PrepareViewBag();

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
            catch (Exception e)
            {
                string message = string.Format("Student ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id), e));
                return RedirectToAction("NotFound", "Error");
            }

            if (student == null)
            {
                string message = string.Format("Student ID {0} returned null", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

            ViewBag.StudyAbroad = await StudyAbroadService.GetAllForStudent(id);
            ViewBag.Notes = await StudentNoteService.GetAllNotes(id);
            ViewBag.ShowActionButtons = true;
            await PrepareViewBag();
            await PrepareStudyAbroadDropDowns();

            return View(student);
        }

        public async Task<ActionResult> ViewInline(int id)
        {
            StudentModel student;

            try
            {
                student = await StudentService.GetStudent(id);
            }
            catch (Exception e)
            {
                string message = string.Format("Student ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id), e));
                return RedirectToAction("NotFound", "Error");
            }

            if (student == null)
            {
                string message = string.Format("Student ID {0} returned null", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

            ViewBag.StudyAbroad = await StudyAbroadService.GetAllForStudent(id);
            ViewBag.Notes = await StudentNoteService.GetAllNotes(id);
            ViewBag.ShowActionButtons = false;
            await PrepareViewBag();
            await PrepareStudyAbroadDropDowns();

            return PartialView("View.Partial", student);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MvcApplication.LogException(new ArgumentException("Missing student ID", nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

            StudentModel student;

            try
            {
                student = await StudentService.GetStudent(id.Value);
            }
            catch (Exception e)
            {
                string message = string.Format("Student ID {0} not found", id.Value);
                MvcApplication.LogException(new ArgumentException(message, nameof(id), e));
                return RedirectToAction("NotFound", "Error");
            }

            if (student == null)
            {
                string message = string.Format("Student ID {0} returned null", id.Value);
                MvcApplication.LogException(new ArgumentException(message, nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

            await PrepareViewBag();

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

            await PrepareViewBag();

            return View(model);
        }

        public async Task<ActionResult> Add()
        {
            await PrepareViewBag();

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

            await PrepareViewBag();

            return View(model);
        }

        public async Task<PartialViewResult> StudyAbroad(int id)
        {
            ViewBag.StudentId = id;
            await PrepareStudyAbroadDropDowns();

            return PartialView(await StudyAbroadService.GetAllForStudent(id));
        }

        private async Task PrepareViewBag()
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

            ViewBag.CanUserDeleteStudents = (Session["User"] as UserModel).IsAdmin;
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

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            StudentModel student = await StudentService.GetStudent(id);

            if (student == null)
            {
                return Json(new GenericResult()
                {
                    Result = false,
                    Message = "Invalid student id"
                });
            }

            UserModel currentUser = Session["User"] as UserModel;

            if (!currentUser.IsAdmin)
            {
                return Json(new GenericResult()
                {
                    Result = false,
                    Message = "Student not eligible for deletion"
                });
            }

            return Json(await StudentService.Delete(id));
        }
    }
}
