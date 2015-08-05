using Belletrix.App_Start;
using StackExchange.Exceptional;
using System;
using System.Web;
using System.Web.Helpers;
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

        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                LogException(ex);
                Server.ClearError();
                Response.Redirect("/error/antiforgery", true);
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

            UnityConfig.ConfigureIocUnityContainer();
        }
    }
}
