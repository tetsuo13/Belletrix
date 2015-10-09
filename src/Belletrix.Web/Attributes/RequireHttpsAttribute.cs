using System;
using System.Web.Mvc;
using RequireHttpsAttributeBase = System.Web.Mvc.RequireHttpsAttribute;

namespace Belletrix.Web.Attributes
{
    /// <summary>
    /// RequireHttpsAttribute using X-Forwarded-Proto header that AppHarbor
    /// has. HTTPS is done at the load balancer level.
    /// </summary>
    /// <seealso href="http://support.appharbor.com/discussions/problems/401-requirehttps-attribute-doesnt-work-in-aspnet-mvc3-on-appharbor">
    /// RequireHttps Attribute doesn't work in ASP.NET MVC3 on AppHarbor
    /// </seealso>
    /// <seealso href="https://gist.github.com/runesoerensen/915869">
    /// RequireHttpsAttribute using X-Forwarded-Proto header
    /// </seealso>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
        AllowMultiple = false)]
    public class RequireHttpsAttribute : RequireHttpsAttributeBase
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                return;
            }

            if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"],
                "https", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }

            HandleNonHttpsRequest(filterContext);
        }
    }
}
