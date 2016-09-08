using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    [Authorize]
    public class PromoController : Controller
    {
        public static string ActivePageName = "promos";

        private readonly IStudentService StudentService;
        private readonly IPromoService PromoService;

        public PromoController(IStudentService studentService, IPromoService promoService)
        {
            StudentService = studentService;
            PromoService = promoService;

            ViewBag.ActivePage = ActivePageName;
        }

        private void TrackPageView(string pageTitle)
        {
            if (Session["User"] != null)
            {
                Analytics.TrackPageView(Request, pageTitle, (Session["User"] as UserModel).Login);
            }
            else
            {
                Analytics.TrackPageView(Request, pageTitle);
            }
        }

        #region Promo management

        public async Task<ActionResult> List()
        {
            Analytics.TrackPageView(Request, "Promo List", (Session["User"] as UserModel).Login);
            return View(await PromoService.GetPromos());
        }

        public ActionResult Add()
        {
            Analytics.TrackPageView(Request, "Add Promo", (Session["User"] as UserModel).Login);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CheckUniqueName(string name)
        {
            bool result = await PromoService.CheckNameForUniqueness(name.Trim());
            return Content(result ? "win" : "fail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(PromoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int promoId = await PromoService.Save(model, (Session["User"] as UserModel).Id);
                return RedirectToAction("Info", new { id = promoId });
            }

            Analytics.TrackPageView(Request, "Add Promo", (Session["User"] as UserModel).Login);
            return View(model);
        }

        public async Task<ActionResult> Info(int id)
        {
            PromoViewModel promo = await PromoService.GetPromo(id);

            if (promo == null)
            {
                return HttpNotFound();
            }

            Analytics.TrackPageView(Request, "Promo Info", (Session["User"] as UserModel).Login);
            return View(promo);
        }

        public async Task<ActionResult> Students(int id)
        {
            PromoViewModel promo = await PromoService.GetPromo(id);

            if (promo == null)
            {
                return HttpNotFound();
            }

            ViewBag.Promo = promo;

            string title = string.Format("Students of {0} promo ({1})", promo.Description, promo.Code);
            Analytics.TrackPageView(Request, title, (Session["User"] as UserModel).Login);
            return View(await StudentService.FromPromo(id));
        }

        #endregion

        #region User portal

        [AllowAnonymous]
        public ActionResult Entry()
        {
            TrackPageView("Promo Entry");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Entry(string code)
        {
            if (code == null || string.IsNullOrWhiteSpace(code))
            {
                ViewBag.ErrorMessage = "Invalid code";
                return View();
            }

            PromoViewModel promo = await PromoService.GetPromo(code);

            if (promo == null)
            {
                TrackPageView("Promo Invalid Code");
                ViewBag.ErrorMessage = "Invalid code";
                return View();
            }

            HttpCookie cookie = new HttpCookie("promo", code);
            cookie.Expires = DateTime.Now.AddHours(8);
            HttpContext.Response.SetCookie(cookie);

            TrackPageView("Promo Form for " + code);
            return RedirectToAction("Form");
        }

        [AllowAnonymous]
        public async Task<ActionResult> Form()
        {
            HttpCookie cookie = HttpContext.Request.Cookies["promo"] ?? null;

            if (cookie == null)
            {
                return RedirectToAction("Entry");
            }

            TrackPageView("Promo Form for " + cookie.Value);
            await PrepareDropDowns();

            return View();
        }

        [AllowAnonymous]
        public async Task<PartialViewResult> NameCheck(string firstName, string lastName)
        {
            return PartialView(await StudentService.SearchByFullName(firstName, lastName));
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Form(StudentPromoModel model)
        {
            HttpCookie cookie = HttpContext.Request.Cookies["promo"];

            if (ModelState.IsValid)
            {
                int? userId = null;
                
                if (Session["User"] != null)
                {
                    userId = (Session["User"] as UserModel).Id;
                }

                await StudentService.InsertStudent(model, userId, cookie.Value,
                    HttpContext.Request.UserHostAddress);
                return RedirectToAction("Success");
            }

            Analytics.TrackPageView(Request, "Student Add", (Session["User"] as UserModel).Login);
            TrackPageView("Promo Form for " + cookie.Value);
            await PrepareDropDowns();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Success()
        {
            HttpCookie cookie = HttpContext.Request.Cookies["promo"] ?? null;

            if (cookie == null)
            {
                return RedirectToAction("Entry");
            }

            TrackPageView("Promo Success for " + cookie.Value);
            return View();
        }

        private async Task PrepareDropDowns()
        {
            IEnumerable<object> years = Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7))
                .Reverse()
                .Select(i => new { Id = i, Name = i.ToString() });

            ViewBag.Countries = await StudentService.GetCountries();
            ViewBag.Languages = await StudentService.GetLanguages();

            ViewBag.GraduatingYears = new SelectList(years, "Id", "Name");
            ViewBag.GraduatingYearsAsEnumerable = years;
            ViewBag.Classifications = new SelectList(StudentService.GetClassifications(), "Id", "Name");

            ViewBag.AvailableMajors = await StudentService.GetMajors();
            ViewBag.AvailableMinors = await StudentService.GetMinors();

            IList<object> studyYears = new List<object>(years);
            studyYears.Insert(0, new { Id = 1, Name = "Any Year" });

            ViewBag.StudyAbroadYears = new SelectList(studyYears, "Id", "Name");
            ViewBag.StudyAbroadSemesters = StudentService.GetStudyAbroadWishlistPeriodsWithCatchAll();
        }

        #endregion
    }
}
