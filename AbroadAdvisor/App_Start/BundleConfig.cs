using System.Web.Optimization;

namespace Bennett.AbroadAdvisor.App_Start
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
                "~/Scripts/jquery.tablesorter.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/tables.js",
                "~/Scripts/AbroadAdvisor.js",
                "~/Scripts/AbroadAdvisor.Promo.js",
                "~/Scripts/AbroadAdvisor.Student.js"));

            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/css/bootstrap-multiselect.css")
                .Include("~/Content/css/sb-admin.css")
                .Include("~/Content/css/font-awesome.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/datepicker.css")
                .Include("~/Content/css/AbroadAdvisor.css"));
        }
    }
}
