using Piwik.Tracker;
using System;
using System.Diagnostics;
using System.Web;

namespace Bennett.AbroadAdvisor.Core
{
    public class Analytics
    {
        [Conditional("!DEBUG")]
        public static void TrackPageView(HttpRequestBase request, string pageTitle, string username)
        {
            PiwikTracker.URL = "http://analytics.andreinicholson.com";
            PiwikTracker tracker = new PiwikTracker(17);

            tracker.setIp(request.UserHostAddress);
            tracker.setBrowserLanguage(request.UserLanguages);
            tracker.setUrl(request.Url.ToString());
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
