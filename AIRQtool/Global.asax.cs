using QTool.DAL;
using QTool.Main.Cache;
using QTool.Main;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AIRQtool
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Statics.Setup(ConfigurationManager.AppSettings.Get("QToolConfig"), false);

            BlobClient.Init(Statics.isDEV, Statics.isQA, Statics.isTEST, Statics.isPROD);
            QToolCache.ReInitCache();
        }
    }
}
