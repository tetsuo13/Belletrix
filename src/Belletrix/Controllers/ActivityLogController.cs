using Belletrix.Core;
using Belletrix.Domain;
using Belletrix.Entity.ViewModel;
using Belletrix.Models;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    [Authorize]
    public class ActivityLogController : Controller
    {
        public ActivityLogController()
        {
            ViewBag.ActivePage = "activitylog";
        }

        public ActionResult List()
        {
            Analytics.TrackPageView(Request, "Activity Log List", (Session["User"] as UserModel).Login);
            ActivityLogService service = new ActivityLogService();
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
            return View(model);
        }
    }
}
