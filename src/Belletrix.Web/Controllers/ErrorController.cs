using Belletrix.Core;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult AntiForgery()
        {
            Analytics.TrackPageView(Request, "Error - AntiForgery");
            return View();
        }
    }
}
