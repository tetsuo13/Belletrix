using System;
using System.Web;
using System.Web.Http;

namespace Belletrix.Web.Controllers
{
    [AllowAnonymous]
    public class CacheController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            DateTime startDate = DateTime.Now;

            // Cache stuff here.

            TimeSpan span = DateTime.Now - startDate;

            return String.Format("Cached {0} objects in {1} seconds",
                HttpRuntime.Cache.Count, span.TotalSeconds);
        }
    }
}
