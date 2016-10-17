using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    [Authorize]
    public class StudyAbroadController : Controller
    {
        public static string ActivePageName = "experiences";

        private readonly IStudentService StudentService;
        private readonly IStudyAbroadService StudyAbroadService;

        public StudyAbroadController(IStudentService studentService, IStudyAbroadService studyAbroadService)
        {
            StudentService = studentService;
            StudyAbroadService = studyAbroadService;

            ViewBag.ActivePage = ActivePageName;
        }

        public async Task<ActionResult> List()
        {
            Analytics.TrackPageView(Request, "Active Experiences", (Session["User"] as UserModel).Login);

            ViewBag.Countries = await StudentService.GetCountries();
            ViewBag.Semesters = StudentService.GetStudyAbroadWishlistPeriods();
            ViewBag.Programs = await StudentService.GetPrograms();
            ViewBag.ProgramTypes = await StudentService.GetProgramTypes();

            return View(await StudyAbroadService.GetAll());
        }

        public async Task<ActionResult> Add(int studentId)
        {
            Analytics.TrackPageView(Request, "Add Experience", (Session["User"] as UserModel).Login);

            try
            {
                ViewBag.Student = await StudentService.GetStudent(studentId);
            }
            catch (Exception e)
            {
                string message = string.Format("Invalid student ID {0}", studentId);
                MvcApplication.LogException(new ArgumentException(message, nameof(studentId), e));
                return RedirectToAction("NotFound", "Error");
            }

            await PrepareDropDowns();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddStudyAbroadViewModel model)
        {
            if (ModelState.IsValid)
            {
                await StudyAbroadService.Save(model, (Session["User"] as UserModel).Id,
                    HttpContext.Request.UserHostAddress);
                return RedirectToAction("View", "Student", new { Id = model.StudentId });
            }

            try
            {
                ViewBag.Student = await StudentService.GetStudent(model.StudentId);
            }
            catch (Exception e)
            {
                string message = string.Format("Invalid student ID {0}", model.StudentId);
                MvcApplication.LogException(new ArgumentException(message, nameof(model), e));
                return RedirectToAction("NotFound", "Error");
            }

            await PrepareDropDowns();
            Analytics.TrackPageView(Request, "Add Experience", (Session["User"] as UserModel).Login);

            return View(model);
        }

        private async Task PrepareDropDowns()
        {
            ViewBag.Years = new SelectList(Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7)).Reverse());
            ViewBag.Semesters = StudentService.GetStudyAbroadWishlistPeriods();
            ViewBag.Countries = new SelectList(await StudentService.GetCountries(), "Id", "Name");
            ViewBag.Programs = new SelectList(await StudentService.GetPrograms(), "Id", "Name");
            ViewBag.ProgramTypes = await StudentService.GetProgramTypes();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            Analytics.TrackPageView(Request, "Delete Experience", (Session["User"] as UserModel).Login);
            return Json(await StudyAbroadService.Delete(id));
        }

        public async Task<ActionResult> Edit(int id)
        {
            StudyAbroadViewModel study = await StudyAbroadService.FindById(id);

            if (study == null)
            {
                string message = string.Format("Study abroad experience ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

            try
            {
                ViewBag.Student = await StudentService.GetStudent(study.StudentId);
            }
            catch (Exception e)
            {
                string message = string.Format("Invalid student ID {0}", study.StudentId);
                MvcApplication.LogException(new ArgumentException(message, nameof(id), e));
                return RedirectToAction("NotFound", "Error");
            }

            Analytics.TrackPageView(Request, "Experience Edit", (Session["User"] as UserModel).Login);

            EditStudyAbroadViewModel model = (EditStudyAbroadViewModel)study;
            await PrepareDropDowns();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditStudyAbroadViewModel model)
        {
            if (ModelState.IsValid)
            {
                await StudyAbroadService.Update(model, (Session["User"] as UserModel).Id,
                    HttpContext.Request.UserHostAddress);
                return RedirectToAction("View", "Student", new { Id = model.StudentId });
            }

            try
            {
                ViewBag.Student = await StudentService.GetStudent(model.StudentId);
            }
            catch (Exception e)
            {
                string message = string.Format("Invalid student ID {0}", model.StudentId);
                MvcApplication.LogException(new ArgumentException(message, nameof(model), e));
                return RedirectToAction("NotFound", "Error");
            }

            Analytics.TrackPageView(Request, "Experience Edit", (Session["User"] as UserModel).Login);
            await PrepareDropDowns();

            return View(model);
        }
    }
}
