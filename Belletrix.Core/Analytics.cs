using Piwik.Tracker;
using System;
using System.Configuration;
using System.Web;

namespace Belletrix.Core
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
        public static void TrackPageView(HttpRequestBase request, string pageTitle, string username = null)
        {
            if (!DeploymentEnvironment.IsProduction)
            {
                return;
            }

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
    }
}
