using StackExchange.Profiling.Mvc;
using System.Web.Mvc;

namespace Belletrix.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ProfilingActionFilter());

#if !DEBUG
            filters.Add(new Attributes.RequireHttpsAttribute());
#endif
        }
    }
}
