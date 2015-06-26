using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.ViewModel;
using Belletrix.Models;
using System;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    [Authorize]
    public class ActivityLogController : Controller
    {
        public static string ActivePageName = "activitylog";

        public ActivityLogController()
        {
            ViewBag.ActivePage = ActivePageName;
        }

        public ActionResult View(int id)
        {
            var service = new ActivityLogService();
            var activity = service.FindByid(id);

            if (activity == null)
            {
                Analytics.TrackPageView(Request, "Activity Log Error", (Session["User"] as UserModel).Login);
                return HttpNotFound();
            }

            Analytics.TrackPageView(Request, "Activity Log View", (Session["User"] as UserModel).Login);

            return View(activity);
        }

        public ActionResult List()
        {
            Analytics.TrackPageView(Request, "Activity Log List", (Session["User"] as UserModel).Login);
            var service = new ActivityLogService();
            return View(service.GetActivityLogs());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ActivityLogCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = new ActivityLogService();
                    service.Create(model, (Session["User"] as UserModel).Id);
                    return RedirectToAction("List");
                }
                catch (Exception e)
                {
                    MvcApplication.LogException(e);
                    ModelState.AddModelError("Title",
                        "There was an error saving. It has been logged for later review.");
                }
            }

            return View(model);
        }
    }
}
