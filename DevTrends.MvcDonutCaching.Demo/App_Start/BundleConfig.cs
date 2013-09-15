using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DevTrends.MvcDonutCaching.Demo
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection t)
        {
            t.Add(new ScriptBundle("~/Content/style").Include(
                "~/Content/normalize.css",
                "~/Content/main.css"
            ));
        }
    }
}