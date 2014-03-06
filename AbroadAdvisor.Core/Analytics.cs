using Piwik.Tracker;
using System;
using System.Web;

namespace Bennett.AbroadAdvisor.Core
{
    /// <summary>
    /// Record analytics server-side. Only functions when not in DEBUG.
    /// </summary>
    /// <remarks>
    /// <para>
    /// We use nginx to distribute requests between multiple servers.
    /// Because of this setup, the <c>Request.UserHostAddress</c> will be the
    /// IP address of our load-balancer instead of the IP address of the
    /// remote user. You can use
    /// <c>Request.ServerVariables["HTTP_X_FORWARDED_FOR"]</c> to access the
    /// user's IP address.
    /// </para>
    /// </remarks>
    /// <seealso href="http://support.appharbor.com/kb/getting-started/information-about-our-load-balancer">
    /// Information about our load-balancer
    /// </seealso>
    public class Analytics
    {
        public static void TrackPageView(HttpRequestBase request, string pageTitle, string username)
        {
#if DEBUG
            return;
#endif

            PiwikTracker.URL = "http://analytics.andreinicholson.com";
            PiwikTracker tracker = new PiwikTracker(17);

            try
            {
                tracker.setIp(request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            }
            catch (Exception)
            {
                tracker.setIp(request.UserHostAddress);
            }

            tracker.setBrowserLanguage(request.UserLanguages);
            tracker.setUrl(request.Url.Scheme + "://" + request.Url.Host + request.Url.PathAndQuery);
            tracker.setBrowserHasCookies(request.Cookies.Count > 0);
            tracker.setUserAgent(request.UserAgent);

            if (request.UrlReferrer != null)
            {
                tracker.setUrlReferrer(request.UrlReferrer.ToString());
            }

            if (!String.IsNullOrEmpty(username))
            {
                tracker.setCustomVariable(1, "username", username);
            }

            tracker.setTokenAuth("fdb0ca65560ea3264a5bcc6922ac08ce");
            tracker.doTrackPageView(pageTitle);
        }
    }
}
