using Bennett.AbroadAdvisor.Models;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ActivePage = "dashboard";
            ViewBag.RecentActivity = EventLogModel.GetEvents();
            return View();
        }
    }
}
