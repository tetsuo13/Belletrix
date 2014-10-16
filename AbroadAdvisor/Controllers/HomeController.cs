using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using StackExchange.Exceptional;
using System;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // TODO: This should be removed in production.
        public ActionResult Throw()
        {
            var ex = new Exception("This is an exception throw from the Samples project! - Check out the log to see this exception.");
            // here's how your catch/throw might can add more info, for example SQL is special cased and shown in the UI:
            ex.Data["SQL"] = "Select * From FUBAR -- This is a SQL command!";
            //ErrorStore.LogException(ex, System.Web.HttpContext.Current);
            throw ex;
        }

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
