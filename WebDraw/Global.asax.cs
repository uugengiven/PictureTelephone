using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebDraw
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configure DB updates to occur if the web.config says it is ok
            if (bool.Parse(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"]))
            {
                var configuration = new WebDraw.Migrations.WebDrawDbContext.Configuration();
                var migrator = new DbMigrator(configuration);
                migrator.Update();

                var configuration1 = new Migrations.ApplicationDbContext.Configuration();
                var migrator1 = new DbMigrator(configuration1);
                migrator1.Update();
            }
        }
    }
}
