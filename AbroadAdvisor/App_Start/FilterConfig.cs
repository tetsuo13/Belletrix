﻿using System.Web.Mvc;

namespace Bennett.AbroadAdvisor
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
