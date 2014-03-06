using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Analytics.TrackPageView(Request, "Dashboard", (Session["User"] as UserModel).Login);
            ViewBag.ActivePage = "dashboard";
            ViewBag.RecentActivity = EventLogModel.GetEvents();
            return View();
        }

        /// <summary>
        /// Function that does nothing.
        /// </summary>
        /// <returns>Some text.</returns>
        [HttpGet]
        public string Ping()
        {
            return PingModel.Ping();
        }
    }
}
