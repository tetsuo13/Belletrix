using Piwik.Tracker;
using StackExchange.Profiling;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.Core
{
    /// <summary>
    /// Record analytics server-side. Only functions when not running a Debug
    /// build. Intended to be a "fire and forget" API.
    /// </summary>
    public class Analytics
    {
        public static void TrackPageView(HttpRequestBase request, string pageTitle, string username = null)
        {
            if (!new DebuggingService().RunningInDebugMode())
            {
                MiniProfiler profiler = MiniProfiler.Current;

                using (profiler.Step("Log request in analytics"))
                {
                    Task.Factory.StartNew(() => SendRequest(request, pageTitle, username));
                }
            }
        }

        private static void SendRequest(HttpRequestBase request, string pageTitle, string username)
        {
            try
            {
                PiwikTracker.URL = "http://analytics.andreinicholson.com";
                PiwikTracker tracker = new PiwikTracker(17);

                try
                {
                    string token = ConfigurationManager.AppSettings["AnalyticsAdminToken"];

                    if (!string.IsNullOrEmpty(token))
                    {
                        tracker.setTokenAuth(token);
                        tracker.setIp(request.UserHostAddress);
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

                if (!string.IsNullOrEmpty(username))
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
