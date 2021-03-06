﻿using Belletrix.Domain;
using Belletrix.Entity.ViewModel;
using StackExchange.Exceptional;
using StackExchange.Profiling;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public static string ActivePageName = "dashboard";

        private readonly IPingService PingService;
        private readonly IEventLogService EventLogService;

        public HomeController(IPingService pingService, IEventLogService eventLogService)
        {
            PingService = pingService;
            EventLogService = eventLogService;
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.ActivePage = ActivePageName;

            DashboardViewModel model = new DashboardViewModel();
            MiniProfiler profiler = MiniProfiler.Current;

            using (profiler.Step("Get events"))
            {
                model.RecentActivity = await EventLogService.GetEvents(10);
            }

            return View(model);
        }

        /// <summary>
        /// Function that does nothing.
        /// </summary>
        /// <remarks>Promotions use this too. Keep anonymous.</remarks>
        /// <returns>Some text.</returns>
        [AllowAnonymous]
        public async Task<string> Ping()
        {
            MiniProfiler profiler = MiniProfiler.Current;

            using (profiler.Step("Ping"))
            {
                return await PingService.Ping();
            }
        }

        [AllowAnonymous]
        [OutputCache(Duration = 86400)]
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }

        [AllowAnonymous]
        [OutputCache(Duration = 86400)]
        public ActionResult Sitemap()
        {
            Response.ContentType = "text/xml";
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
