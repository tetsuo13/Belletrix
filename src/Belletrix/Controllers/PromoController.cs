using Belletrix.Core;
using Belletrix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    [Authorize]
    public class PromoController : Controller
    {
        public static string ActivePageName = "promos";

        public PromoController()
        {
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

        public ActionResult List()
        {
            Analytics.TrackPageView(Request, "Promo List", (Session["User"] as UserModel).Login);
            return View(PromoModel.GetPromos(true));
        }

        public ActionResult Add()
        {
            Analytics.TrackPageView(Request, "Add Promo", (Session["User"] as UserModel).Login);
            return View();
        }

        [HttpPost]
        public ActionResult CheckUniqueName(string name)
        {
            bool result = PromoModel.CheckNameForUniqueness(name.Trim());
            return Content(result ? "win" : "fail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PromoModel model)
        {
            if (ModelState.IsValid)
            {
                model.Save((Session["User"] as UserModel).Id);
                return RedirectToAction("Info", new { id = model.Id });
            }

            Analytics.TrackPageView(Request, "Add Promo", (Session["User"] as UserModel).Login);
            return View();
        }

        public ActionResult Info(int id)
        {
            PromoModel promo = PromoModel.GetPromo(id);

            if (promo == null)
            {
                return HttpNotFound();
            }

            Analytics.TrackPageView(Request, "Promo Info", (Session["User"] as UserModel).Login);
            return View(promo);
        }

        public ActionResult Students(int id)
        {
            PromoModel promo = PromoModel.GetPromo(id);

            if (promo == null)
            {
                return HttpNotFound();
            }

            ViewBag.Promo = promo;

            string title = String.Format("Students of {0} promo ({1})", promo.Description, promo.Code);
            Analytics.TrackPageView(Request, title, (Session["User"] as UserModel).Login);
            return View(StudentModel.FromPromo(id));
        }

        [AllowAnonymous]
        public ActionResult Entry()
        {
            TrackPageView("Promo Entry");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Entry(string code)
        {
            if (code == null || String.IsNullOrWhiteSpace(code))
            {
                ViewBag.ErrorMessage = "Invalid code";
                return View();
            }

            PromoModel promo = PromoModel.GetPromo(code);

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
        public ActionResult Form()
        {
            HttpCookie cookie = HttpContext.Request.Cookies["promo"] ?? null;

            if (cookie == null)
            {
                return RedirectToAction("Entry");
            }

            TrackPageView("Promo Form for " + cookie.Value);
            PrepareDropDowns();

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(StudentPromoModel model)
        {
            HttpCookie cookie = HttpContext.Request.Cookies["promo"];

            if (ModelState.IsValid)
            {
                int? userId = null;
                
                if (Session["User"] != null)
                {
                    userId = (Session["User"] as UserModel).Id;
                }

                model.Save(userId, cookie.Value);
                return RedirectToAction("Success");
            }

            Analytics.TrackPageView(Request, "Student Add", (Session["User"] as UserModel).Login);
            TrackPageView("Promo Form for " + cookie.Value);
            PrepareDropDowns();
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

        private void PrepareDropDowns()
        {
            IEnumerable<object> years = Enumerable.Range(1990, (DateTime.Now.Year - 1990 + 7))
                .Reverse()
                .Select(i => new { Id = i, Name = i.ToString() });

            ViewBag.Countries = CountryModel.GetCountries();
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
        }
    }
}
