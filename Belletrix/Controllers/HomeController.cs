using Belletrix.Core;
using Belletrix.Models;
using StackExchange.Exceptional;
using System.Web.Mvc;

namespace Belletrix.Controllers
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
        /// <remarks>Promotions use this too. Keep anonymous.</remarks>
        /// <returns>Some text.</returns>
        [AllowAnonymous]
        public string Ping()
        {
            return PingModel.Ping();
        }

        [AllowAnonymous]
        [OutputCache(Duration = 86400)]
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }

        /// <summary>
        /// This lets you access the error handler via a route in your application, secured by whatever
        /// mechanisms are already in place.
        /// </summary>
        /// <remarks>If mapping via RouteAttribute: [Route("errors/{path?}/{subPath?}")]</remarks>
        public ActionResult Exceptions()
        {
            var context = System.Web.HttpContext.Current;
            var page = new HandlerFactory().GetHandler(context, Request.RequestType, Request.Url.ToString(), Request.PathInfo);
            page.ProcessRequest(context);

            return null;
        }
    }
}
