using Belletrix.Core;
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
            return View(ActivityLogModel.GetActivityLogs());
        }
    }
}
