using System.Web;
using System.Web.Optimization;

namespace Mao.Flazor
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/lib/css").Include(
                "~/lib/fontawesome-free-5.15.3-web/css/all.min.css",                                                // fontawesome
                "~/lib/bootstrap-4.6.0-dist/css/bootstrap.min.css",                                                 // bootstrap
                "~/lib/site.css"));

            bundles.Add(new ScriptBundle("~/lib/js").Include(
                "~/lib/jquery-3.6.0.min.js",                                                                        // jquery
                "~/lib/popper.js-1.16.1/umd/popper.min.js",                                                         // * bootstrap 下拉選單功能需要的參考
                "~/lib/fontawesome-free-5.15.3-web/js/all.min.js",                                                  // fontawesome
                "~/lib/bootstrap-4.6.0-dist/js/bootstrap.min.js",                                                   // bootstrap
                "~/lib/site.js"));
        }
    }
}
