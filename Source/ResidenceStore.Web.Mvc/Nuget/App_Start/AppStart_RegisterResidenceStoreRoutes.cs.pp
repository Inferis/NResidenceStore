using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Infrastructure;
 
[assembly: WebActivator.PostApplicationStartMethod(typeof($rootnamespace$.AppStart_RegisterResidenceStoreRoutes), "Start")]
 
namespace $rootnamespace$ {
    public static class AppStart_RegisterResidenceStoreRoutes {
        public static void Start() {
            // Set everything up with you having to do any work.
            // I'm doing this because it means that
            // your app will just run. You might want to get rid of this 
            // and integrate with your own Global.asax. 
            // It's up to you. 
            RegisterRoutes(RouteTable.Routes);
        }
 
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Residence", // Route name
                "{controller}/{token}", // URL with parameters
                new { controller = "Residence", action = "Index", token = UrlParameter.Optional } // Parameter defaults
            );
        }
    }
}