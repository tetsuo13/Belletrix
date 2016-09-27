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

        public async Task<ActionResult> Edit(int id)
        {
            PromoViewModel promo = await PromoService.GetPromo(id);

            if (promo == null)
            {
                string message = string.Format("Promo ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, "id"));
                return RedirectToAction("NotFound", "Error");
            }

            Analytics.TrackPageView(Request, "Edit Promo", (Session["User"] as UserModel).Login);

            PromoEditViewModel model = new PromoEditViewModel()
            {
                Id = promo.Id,
                Description = promo.Description,
                Active = promo.Active
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(PromoEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await PromoService.Update(model);
                return RedirectToAction("Info", new { id = model.Id });
            }

            return View(model);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            PromoViewModel promo = await PromoService.GetPromo(id);
            Analytics.TrackPageView(Request, "Delete Promo", (Session["User"] as UserModel).Login);

            if (promo == null)
            {
                return Json(new GenericResult()
                {
                    Result = false,
                    Message = "Invalid promo code"
                });
            }

            if (!promo.CanDelete)
            {
                return Json(new GenericResult()
                {
                    Result = false,
                    Message = "Promo not eligible for deletion"
                });
            }

            return Json(await PromoService.Delete(id));
        }

        public async Task<ActionResult> Info(int id)
        {
            PromoViewModel promo = await PromoService.GetPromo(id);

            if (promo == null)
            {
                string message = string.Format("Promo ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, "id"));
                return RedirectToAction("NotFound", "Error");
            }

            Analytics.TrackPageView(Request, "Promo Info", (Session["User"] as UserModel).Login);
            promo.Students = await StudentService.FromPromo(promo.Id);
            return View(promo);
        }

        #endregion

        #region User portal

        /// <summary>
        /// Requires a guid to work but accept anything that's a string and
        /// try parsing it as a guid. Done this way to ensure all students
        /// will hit this page regardless if something's happened to the
        /// token (sent via email which truncated a few characters off the
        /// end).
        /// </summary>
        [AllowAnonymous]
        public async Task<ActionResult> Entry(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View(model: "Missing code.");
            }

            Guid guidToken;

            if (!Guid.TryParse(token, out guidToken))
            {
                return View(model: "Invalid code.");
            }

            PromoViewModel promo = await PromoService.GetPromo(guidToken);

            if (promo == null)
            {
                return View(model: "The code you tried to use is invalid.");
            }
            else if (!promo.Active)
            {
                return View(model: "Promo has expired. Please contact Center for Global Studies for help.");
            }

            HttpCookie cookie = new HttpCookie("promo", token.ToString());
            cookie.Expires = DateTime.Now.AddHours(8);
            HttpContext.Response.SetCookie(cookie);

            TrackPageView("Promo Form for " + token);
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

                await StudentService.InsertStudent(model, userId, Guid.Parse(cookie.Value),
                    HttpContext.Request.UserHostAddress);
                return RedirectToAction("Success");
            }

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
