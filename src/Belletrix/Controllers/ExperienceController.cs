using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    [Authorize]
    public class ExperienceController : Controller
    {
        public static string ActivePageName = "experiences";

        private readonly IStudentService StudentService;
        private readonly IStudyAbroadService StudyAbroadService;

        public ExperienceController(IStudentService studentService, IStudyAbroadService studyAbroadService)
        {
            StudentService = studentService;
            StudyAbroadService = studyAbroadService;

            ViewBag.ActivePage = ActivePageName;
        }

        public async Task<ActionResult> List()
        {
            await Analytics.TrackPageView(Request, "Active Experiences", (Session["User"] as UserModel).Login);

            ViewBag.Countries = await StudentService.GetCountries();
            ViewBag.Semesters = StudentService.GetStudyAbroadWishlistPeriods();
            ViewBag.Programs = await StudentService.GetPrograms();
            ViewBag.ProgramTypes = await StudentService.GetProgramTypes();

            return View(await StudyAbroadService.GetAll());
        }

        public async Task<ActionResult> Add(int studentId)
        {
            await Analytics.TrackPageView(Request, "Add Experience", (Session["User"] as UserModel).Login);

            try
            {
                await PrepareStudent(studentId);
            }
            catch (Exception)
            {
                return HttpNotFound("Invalid student");
            }

            await PrepareDropDowns();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(StudyAbroadModel model)
        {
            if (ModelState.IsValid)
            {
                await StudyAbroadService.Save(model, (Session["User"] as UserModel).Id);
                return RedirectToAction("View", "Student", new { Id = model.StudentId });
            }

            try
            {
                await PrepareStudent(model.StudentId);
            }
            catch (Exception)
            {
                return HttpNotFound("Invalid student");
            }

            await PrepareDropDowns();
            await Analytics.TrackPageView(Request, "Add Experience", (Session["User"] as UserModel).Login);

            return View(model);
        }

        private async Task PrepareStudent(int studentId)
        {
            try
            {
                ViewBag.Student = await StudentService.GetStudent(studentId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task PrepareDropDowns()
        {
            ViewBag.Years = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7)).Reverse());
            ViewBag.Semesters = StudentService.GetStudyAbroadWishlistPeriods();
            ViewBag.Countries = new SelectList(await StudentService.GetCountries(), "Id", "Name");
            ViewBag.Programs = new SelectList(await StudentService.GetPrograms(), "Id", "Name");
            ViewBag.ProgramTypes = await StudentService.GetProgramTypes();
        }
    }
}
