using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Enum;
using Belletrix.Entity.ViewModel;
using Belletrix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    [Authorize]
    public class ActivityLogController : Controller
    {
        public static string ActivePageName = "activitylog";

        private readonly IActivityService ActivityService;

        public ActivityLogController(IActivityService activityService)
        {
            ActivityService = activityService;

            ViewBag.ActivePage = ActivePageName;
        }

        public async Task<ActionResult> View(int id)
        {
            var activity = await ActivityService.FindByid(id);

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
            var logs = await ActivityService.GetActivityLogs();
            return View(logs);
        }

        private void PrepareViewBag(ActivityLogCreateViewModel model)
        {
            IEnumerable<SelectListItem> types = from ActivityLogTypes a
                                                in Enum.GetValues(typeof(ActivityLogTypes))
                                                select new SelectListItem
                                                {
                                                    Value = ((int)a).ToString(),
                                                    Text = a.GetDisplayName()
                                                };

            ViewBag.TypesSelect = new MultiSelectList(types, "Value", "Text", model.Types);
        }

        public ActionResult Add()
        {
            PrepareViewBag(new ActivityLogCreateViewModel());
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
                    int activityId = await ActivityService.InsertActivity(model, (Session["User"] as UserModel).Id);

                    await ActivityService.AssociatePeopleWithActivity(Session, activityId, model.SessionId);

                    ActivityService.ClearSession(Session, model.SessionId);

                    ActivityService.SaveChanges();

                    return RedirectToAction("List");
                }
                catch (Exception e)
                {
                    MvcApplication.LogException(e);
                    ModelState.AddModelError("Title",
                        "There was an error saving. It has been logged for later review.");
                }
            }

            PrepareViewBag(model);

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var activity = await ActivityService.FindByid(id);

            if (activity == null)
            {
                await Analytics.TrackPageView(Request, "Activity Log Error", (Session["User"] as UserModel).Login);
                return HttpNotFound();
            }

            await Analytics.TrackPageView(Request, "Activity Log Edit", (Session["User"] as UserModel).Login);

            ActivityLogEditViewModel model = (ActivityLogEditViewModel)activity;
            PrepareViewBag(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ActivityLogEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await ActivityService.UpdateActivity(model);

                    await ActivityService.AssociatePeopleWithActivity(Session, model.Id, model.SessionId);

                    ActivityService.ClearSession(Session, model.SessionId);

                    ActivityService.SaveChanges();

                    return RedirectToAction("List");
                }
                catch (Exception e)
                {
                    MvcApplication.LogException(e);
                    ModelState.AddModelError("Title",
                        "There was an error saving. It has been logged for later review.");
                }
            }

            PrepareViewBag(model);

            return View(model);
        }
    }
}
