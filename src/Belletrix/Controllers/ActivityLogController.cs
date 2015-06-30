using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.ViewModel;
using Belletrix.Models;
using System;
using System.Threading.Tasks;
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

        public async Task<ActionResult> View(int id)
        {
            var service = new ActivityLogService();
            var activity = await service.FindByid(id);

            if (activity == null)
            {
                await Analytics.TrackPageView(Request, "Activity Log Error", (Session["User"] as UserModel).Login);
                return HttpNotFound();
            }

            await Analytics.TrackPageView(Request, "Activity Log View", (Session["User"] as UserModel).Login);

            return View(activity);
        }

        public async Task<ActionResult> List()
        {
            await Analytics.TrackPageView(Request, "Activity Log List", (Session["User"] as UserModel).Login);
            var service = new ActivityLogService();
            return View(await service.GetActivityLogs());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(ActivityLogCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = new ActivityLogService();
                    await service.Create(model, (Session["User"] as UserModel).Id);
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
