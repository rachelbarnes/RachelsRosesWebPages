using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RachelsRosesWebPages.Controllers {
    public class Ingredient {
        public string name;
        public string measurement;

        public Ingredient(string _name, string _measurement) {
            name = _name;
            measurement = _measurement;
        }
    }
    public class Recipe {
        public string name;
        public List<Ingredient> ingredients;

        public Recipe(string _name) {
            name = _name;
            ingredients = new List<Ingredient>();
            //since we always start with an empty list, creating an empty list inthe constructor is the easiest way to go for now
        }
    }
    public class HomeController : Controller {
        public static List<Recipe> recipes = new List<Recipe>();
        public static Recipe currentRecipe = null;

        public ActionResult Index() {
            return View();
        }
        public ActionResult Recipes() {
            ViewBag.recipes = recipes;
            return View();
        }
        public ActionResult Recipe(string name) {
            currentRecipe = recipes.First(x => x.name == name);
            ViewBag.ingredients = currentRecipe.ingredients;
                //view looks up the view for that action method, it's looking up View called Recipe
                //View looks at everything in the ViewBag and renders that
            return View();
        }
        public ActionResult DeleteIngredient(string ingredient) {
            currentRecipe.ingredients = currentRecipe.ingredients.Where(x => x.name != ingredient).ToList();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateIngredient(string ingredient, string measurement) {
            Ingredient newingredient = new Ingredient(ingredient, measurement);
            currentRecipe.ingredients.Add(newingredient);
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateRecipe(string recipeTitle) {
            Recipe newrecipe = new Recipe(recipeTitle);
            recipes.Add(newrecipe);
            return Redirect("/home/recipes");
                //the client/browser requests CreateRecipe, and CreateRecipe replies with go to "/home/recipes"
                    //browser automatically goes to "/home/recipes" and displays the recipes
        }
        public ActionResult DeleteRecipe(string recipeTitle) {
            recipes = recipes.Where(x => x.name != recipeTitle).ToList();
            return Redirect("/home/recipes");
        }
    }
}

//cannot create an ingredient or recipe names that has space after or before (trim ingredient names)
//cannot create an ingredient with null name
//cannot create an ingredient/recipe with duplicate name
//when you create a recipe/ingredient with duplicate name, display an error message on recipe and recipes that say there's a dupcliate name
    //use ViewBag.errorMessage = "Duplicate name error" or something