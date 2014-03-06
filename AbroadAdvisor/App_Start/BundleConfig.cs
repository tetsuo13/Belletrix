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
                "~/Scripts/tables.js",
                "~/Scripts/AbroadAdvisor.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-multiselect.css",
                "~/Content/css/sb-admin.css",
                "~/Content/css/font-awesome.min.css",
                "~/Content/css/datepicker.css",
                "~/Content/css/AbroadAdvisor.css"));
        }
    }
}
