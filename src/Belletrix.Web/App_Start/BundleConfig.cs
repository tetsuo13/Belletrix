using System.Web.Optimization;

namespace Belletrix.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/bootstrap-multiselect.js",
                "~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/jquery.dataTables.bootstrap.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/bootbox.min.js",
                "~/Scripts/Belletrix.js",
                "~/Scripts/Belletrix.ActivityLog.js",
                "~/Scripts/Belletrix.Promo.js",
                "~/Scripts/Belletrix.Student.js",
                "~/Scripts/Belletrix.Student.Note.js"));

            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/css/bootstrap-multiselect.css")
                .Include("~/css/font-awesome.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/datepicker.css")
                .Include("~/Content/css/jquery.dataTables.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/jquery.dataTables.bootstrap.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/Belletrix.css"));
        }
    }
}
