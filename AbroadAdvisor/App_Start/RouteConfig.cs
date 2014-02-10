using System.Security.Policy;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bennett.AbroadAdvisor
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;

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
