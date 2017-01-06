using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
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
            MiniProfiler profiler = MiniProfiler.Current;
            ActivityLogViewViewModel activity = null;

            using (profiler.Step("Get activity"))
            {
                activity = await ActivityService.FindAllInfoById(id);
            }

            if (activity == null)
            {
                string message = string.Format("Activity Log ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

            using (profiler.Step("Get all label types"))
            {
                ViewBag.TypeLabels = ActivityService.GetActivityTypeLabels();
            }

            using (profiler.Step("Get all documents"))
            {
                activity.Documents = await ActivityService.FindDocuments(id);
            }

            return View(activity);
        }

        public async Task<ActionResult> List()
        {
            MiniProfiler profiler = MiniProfiler.Current;
            IEnumerable<ActivityLogModel> logs;

            using (profiler.Step("Get activity list"))
            {
                using (profiler.Step("Get all activities"))
                {
                    logs = await ActivityService.GetActivityLogs();
                }
                using (profiler.Step("Get all label types"))
                {
                    ViewBag.TypeLabels = ActivityService.GetActivityTypeLabels();
                }
            }

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
                    await ActivityService.InsertActivityBlock(Session, model);
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
            var activity = await ActivityService.FindById(id);

            if (activity == null)
            {
                string message = string.Format("Activity Log ID {0} not found", id);
                MvcApplication.LogException(new ArgumentException(message, nameof(id)));
                return RedirectToAction("NotFound", "Error");
            }

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
                    await ActivityService.UpdateActivityBlock(Session, model);
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

        public async Task<PartialViewResult> TitleCheck(string title)
        {
            return PartialView("TitleCheck.Partial", await ActivityService.FindByTitle(title));
        }

        [HttpPost]
        public async Task<JsonResult> AddDocument(AddNewDocumentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new GenericResult()
                {
                    Result = false,
                    Message = "Missing selected file"
                });
            }

            return Json(await ActivityService.AddDocument(Session, model));
        }
    }
}
