using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RachelsRosesWebPages {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                //url: "home/recipes",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "RecipeBox", id = UrlParameter.Optional }
            );
        }
    }
}
/*
Recognizing Patterns:
that url is the "placeholder" for the url to be placed there, if I wanted it on a specific recipe, then my action would be "Recipe" and my id would consist of the recipe name, such as "Cake"
*/