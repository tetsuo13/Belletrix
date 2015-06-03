using Piwik.Tracker;
using System;
using System.Configuration;
using System.Web;

namespace Belletrix.Core
{
    /// <summary>
    /// Record analytics server-side. Only functions when not running a Debug
    /// build. Intended to be a "fire and forget" API.
    /// </summary>
    /// <remarks>
    /// We use nginx to distribute requests between multiple servers.
    /// Because of this setup, the <b>Request.UserHostAddress</b> will be the
    /// IP address of our load-balancer instead of the IP address of the
    /// remote user. You can use
    /// <b>Request.ServerVariables["HTTP_X_FORWARDED_FOR"]</b> to access the
    /// user's IP address.
    /// </remarks>
    /// <seealso href="http://support.appharbor.com/kb/getting-started/information-about-our-load-balancer">
    /// Information about our load-balancer
    /// </seealso>
    public class Analytics
    {
        public static void TrackPageView(HttpRequestBase request, string pageTitle, string username = null)
        {
            if (new DebuggingService().RunningInDebugMode())
            {
                return;
            }

            try
            {
                PiwikTracker.URL = "http://analytics.andreinicholson.com";
                PiwikTracker tracker = new PiwikTracker(17);

                try
                {
                    string token = ConfigurationManager.AppSettings["AnalyticsAdminToken"];

                    if (!String.IsNullOrEmpty(token))
                    {
                        tracker.setTokenAuth(token);

                        try
                        {
                            tracker.setIp(request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                        }
                        catch (Exception)
                        {
                            tracker.setIp(request.UserHostAddress);
                        }
                    }
                }
                catch (Exception)
                {
                }

                if (request.UserLanguages != null)
                {
                    tracker.setBrowserLanguage(request.UserLanguages[0]);
                }

                tracker.setUrl(request.Url.Scheme + "://" + request.Url.Host + request.Url.PathAndQuery);
                tracker.setBrowserHasCookies(request.Cookies.Count > 0);
                tracker.setUserAgent(request.UserAgent);
                tracker.setRequestTimeout(600000);

                if (request.UrlReferrer != null)
                {
                    tracker.setUrlReferrer(request.UrlReferrer.ToString());
                }

                if (!String.IsNullOrEmpty(username))
                {
                    tracker.setCustomVariable(1, "username", username);
                }

                tracker.doTrackPageView(pageTitle);
            }
            catch (Exception)
            {
                // TODO: Log it?
            }
        }
    }
}
