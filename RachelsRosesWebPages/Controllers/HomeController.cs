using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RachelsRosesWebPages.Controllers {
    public class HomeController : Controller {
        public class Ingredient {
            public string name;
            public string measurement;
        }
        public static Dictionary<string, string> ingredients = new Dictionary<string, string>();
        public ActionResult Index() {
            return View();
        }
        public ActionResult Delete(string item) {
            ingredients.Remove( item);
            return Redirect("/home/about"); 
        }
        public ActionResult CreateIngredient(string ingredient, string quantity) {
            ingredients.Add(ingredient, quantity);
            return Redirect("/home/about"); 
        }
        public ActionResult About() {
            ViewBag.ingredients = ingredients;
            return View();
        }
        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}

//notes for About.cshtml: 
//there's something wrong with lines 13-20... the value isn't being displayed and I can't delete things from    
//the dictionary, which may mean that hte item isn't being passed correctly... 
//look into this more... i'm not having trouble with the one Steve helped me set up, so do a comparison analysis

