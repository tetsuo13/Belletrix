using Belletrix.Core;
using System.Web.Mvc;

namespace Belletrix
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            if (!new DebuggingService().RunningInDebugMode())
            {
                filters.Add(new Attributes.RequireHttpsAttribute());
            }
        }
    }
}
