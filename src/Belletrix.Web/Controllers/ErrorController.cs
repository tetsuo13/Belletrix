using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult AntiForgery()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}
