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
            name = name.Trim();
            currentRecipe = recipes.First(x => x.name == name);
            ViewBag.ingredients = currentRecipe.ingredients;
            ViewBag.recipename = currentRecipe.name;
            return View();
        }
        public ActionResult DeleteIngredient(string ingredient) {
            currentRecipe.ingredients = currentRecipe.ingredients.Where(x => x.name != ingredient).ToList();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateIngredient(string ingredient, string measurement) {
            ingredient = ingredient.Trim();
            measurement = measurement.Trim();
            Ingredient newingredient = new Ingredient(ingredient, measurement);
            currentRecipe.ingredients.Add(newingredient);
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            if (string.IsNullOrEmpty(recipeTitle)) {
                ViewBag.ErrorMessage = "Please enter a recipe title.";
            }
            Recipe newrecipe = new Recipe(recipeTitle);
            recipes.Add(newrecipe);
            return Redirect("/home/recipes");
        }
        public ActionResult DeleteRecipe(string recipeTitle) {
            recipes = recipes.Where(x => x.name != recipeTitle).ToList();
            return Redirect("/home/recipes");
        }
        public ActionResult EditRecipeTitle(string oldRecipeTitle, string newRecipeTitle) {
            currentRecipe.name = oldRecipeTitle;
            currentRecipe.name = newRecipeTitle;
            ViewBag.newRecipeTitle = newRecipeTitle; 
            return Redirect("/home/recipe?name=" + newRecipeTitle);
            //note to self, this didn't work the first time I did it, I obviously did something wrong. As always, start simple, evaluate
                //any problems, then proceed from there... don't immediately try another solution before figuring out what the problem is/was. 
        }
    }
}

//view looks up the view for that action method, it's looking up View called Recipe
//View looks at everything in the ViewBag and renders that
//the client/browser requests CreateRecipe, and CreateRecipe replies with go to "/home/recipes"
//browser automatically goes to "/home/recipes" and displays the recipes


/*
DONE: 
cannot create an ingredient or recipe names that has space after or before (trim ingredient names)

NOT DONE YET: 
cannot create an ingredient with null name
cannot create an ingredient/recipe with duplicate name
when you create a recipe/ingredient with duplicate name, display an error message on recipe and recipes that say there's a dupcliate name
use ViewBag.errorMessage = "Duplicate name error" or something

 
*/
