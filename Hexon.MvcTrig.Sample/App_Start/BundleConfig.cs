﻿using System.Web;
using System.Web.Optimization;

namespace Hexon.MvcTrig.Sample
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.fancybox.js",
                        "~/Scripts/hexon-mvcTrig.js",
                        "~/Scripts/hexon-mvcTrig.message.js",
                        "~/Scripts/hexon-mvcTrig.jquery.js",
                        "~/Scripts/hexon-mvcTrig.fancybox.js",
                        "~/Scripts/hexon-mvcTrig.bootstrap.js",
                        "~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery.fancybox.css",
                      "~/Content/jquery.fancybox-buttons.css",
                      "~/Content/jquery.fancybox-thumbs.css"));
        }
    }
}
