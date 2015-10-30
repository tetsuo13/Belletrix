using System.Web.Mvc;

namespace Belletrix.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

#if !DEBUG
            filters.Add(new Attributes.RequireHttpsAttribute());
#endif
        }
    }
}
