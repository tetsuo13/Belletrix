using System.Web.Mvc;
using System.Web.Routing;

namespace Belletrix
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;

            // StackExchange.Exceptional error log page.
            routes.MapRoute(
                name: "Exceptions",
                url: "errors/{path}/{subPath}",
                defaults: new
                {
                    controller = "Home",
                    action = "Exceptions",
                    path = UrlParameter.Optional,
                    subPath = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Robots",
                url: "robots.txt",
                defaults: new
                {
                    controller = "Home",
                    action = "Robots"
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "User",
                    action = "Login",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}
