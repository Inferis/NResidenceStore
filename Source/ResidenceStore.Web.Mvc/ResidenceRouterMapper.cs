using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResidenceStore.Web.Mvc
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class ResidenceRouteMapper
    {
        public static void MapResidenceRoutes(this RouteCollection routes, string name, string url, object defaults)
        {
            MapResidenceRoutes(routes, name, url, CreateRouteValueDictionary(defaults));
        }

        public static void MapResidenceRoutes(this RouteCollection routes, string name, string url, RouteValueDictionary defaults)
        {
            if (!url.EndsWith("/")) url += "/";
            defaults = defaults ?? new RouteValueDictionary() { { "controller", "Residence" } };

            // POST to root = register
            routes.MapRoute(
                name + "_Register",
                url,
                defaults.WithAction("Register"),
                new { httpMethod = new HttpMethodConstraint(HttpVerbs.Post.ToString()) }
                );

            // GET to root with token = confirm verify 
            routes.MapRoute(
                name + "_Confirm",
                url + "{token}",
                defaults.WithAction("Confirm"),
                new { httpMethod = new HttpMethodConstraint(HttpVerbs.Get.ToString()) }
                );

            // GET to root without token = check verified
            routes.MapRoute(
                name + "_Verify",
                url,
                defaults.WithAction("Verify"),
                new { httpMethod = new HttpMethodConstraint(HttpVerbs.Get.ToString()) }
                );

            // DELETE to root with token = remove token
            routes.MapRoute(
                name + "_Delete",
                url + "{token}",
                defaults.WithAction("Delete"),
                new { httpMethod = new HttpMethodConstraint(HttpVerbs.Delete.ToString()) }
                );

            // OPTIONS to root without token = get options
            routes.MapRoute(
                name + "_Options",
                url,
                defaults.WithAction("Options"),
                new { httpMethod = new HttpMethodConstraint(HttpVerbs.Options.ToString()) }
                );
        }

        private static RouteValueDictionary CreateRouteValueDictionary(object values)
        {
            var dictionary = values as IDictionary<string, object>;
            return dictionary != null ? new RouteValueDictionary(dictionary) : new RouteValueDictionary(values);
        }

        private static RouteValueDictionary WithAction(this RouteValueDictionary values, string action)
        {
            var r = new RouteValueDictionary(values);
            r["action"] = action;
            return r;
        }
    }
}
