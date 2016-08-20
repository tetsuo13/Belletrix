using Belletrix.Web.App_Start;
using System.Linq;
using StackExchange.Exceptional;
using StackExchange.Profiling;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StackExchange.Profiling.Mvc;
using System.Collections.Generic;

namespace Belletrix.Web
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

        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                LogException(ex);
                Server.ClearError();
                Response.Redirect("/error/antiforgery", true);

                // Don't let this fall into the "uncaught exception" code.
                // Response.Redirect() doesn't exit the function.
                return;
            }

            HttpException httpException = ex as HttpException;
            bool notFoundException = httpException != null && httpException.GetHttpCode() == 404;

            // Don't log every "/wp-admin" and other common exploit path under
            // the sun by every random attacker.
            if (!notFoundException)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Configures the anti-forgery tokens. See
        /// http://www.asp.net/mvc/overview/security/xsrfcsrf-prevention-in-aspnet-mvc-and-web-pages
        /// </summary>
        private static void ConfigureAntiForgeryTokens()
        {
            // Rename the Anti-Forgery cookie from "__RequestVerificationToken" to "f". This adds a little security
            // through obscurity and also saves sending a few characters over the wire. Sadly there is no way to change
            // the form input name which is hard coded in the @Html.AntiForgeryToken helper and the
            // ValidationAntiforgeryTokenAttribute to  __RequestVerificationToken.
            // <input name="__RequestVerificationToken" type="hidden" value="..." />
            AntiForgeryConfig.CookieName = "f";

            // If you have enabled SSL. Uncomment this line to ensure that the Anti-Forgery
            // cookie requires SSL to be sent across the wire.
#if !DEBUG
            AntiForgeryConfig.RequireSsl = true;
#endif
        }

        /// <summary>
        /// Configures the view engines. By default, Asp.Net MVC includes the Web Forms (WebFormsViewEngine) and 
        /// Razor (RazorViewEngine) view engines. You can remove view engines you are not using here for better
        /// performance.
        /// </summary>
        private static void ConfigureViewEngines()
        {
            // Only use the RazorViewEngine.
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-AspNet-Version");
        }

        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
        }

        protected void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {
            if (Context.User.Identity.Name != "anicholson")
            {
                MiniProfiler.Stop(true);
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        protected void Application_Start()
        {
            // Disable the X-AspNetMvc-Version header.
            MvcHandler.DisableMvcResponseHeader = true;

            ConfigureAntiForgeryTokens();
            ConfigureViewEngines();

            AreaRegistration.RegisterAllAreas();

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Filters.Add(new System.Web.Http.AuthorizeAttribute());

            IEnumerable<IViewEngine> copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (IViewEngine item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }
    }
}
