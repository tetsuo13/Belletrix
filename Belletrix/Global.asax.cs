using Belletrix.App_Start;
using StackExchange.Exceptional;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Belletrix
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Manually log an exception.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        public static void LogException(Exception e)
        {
            ErrorStore.LogException(e, HttpContext.Current);
        }

        protected void Application_Start()
        {
            // Disable the X-AspNetMvc-Version header.
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
        }
    }
}
