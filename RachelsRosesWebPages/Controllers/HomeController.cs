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
        }
    }
    public class HomeController : Controller {
        public static List<Recipe> recipes = new List<Recipe>();
        public static Recipe currentRecipe = null;
        public Dictionary<string, string> oldAndNewIngredientNames = null;
        public Dictionary<string, string> oldAndNewIngredientMeasurements = null;

        public ActionResult Index() {
            return View();
        }
        public ActionResult Recipes() {
            ViewBag.recipes = recipes;
            return View();
        }
        public ActionResult Recipe(string name) {
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");

            name = name.Trim();
            currentRecipe = recipes.First(x => x.name == name);
            ViewBag.ingredients = currentRecipe.ingredients;
            ViewBag.recipename = currentRecipe.name;
            foreach (var ingredient in currentRecipe.ingredients) {
                if (string.IsNullOrEmpty(ingredient.name)) {
                    ViewBag.ErrorMessage = "Please enter an ingredient name and measurement.";
                }
            }
            if (oldAndNewIngredientMeasurements != null && oldAndNewIngredientNames != null) {
                ViewBag.oldName = oldAndNewIngredientNames.Keys;
                ViewBag.newName = oldAndNewIngredientNames.Values;
                ViewBag.oldMeasurement = oldAndNewIngredientMeasurements.Keys;
                ViewBag.newMeasurement = oldAndNewIngredientMeasurements.Values;
            }
            return View();
        }
        public ActionResult DeleteIngredient(string ingredient) {
            currentRecipe.ingredients = currentRecipe.ingredients.Where(x => x.name != ingredient).ToList();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateIngredient(string ingredient, string measurement) {
            ingredient = ingredient.Trim();
            measurement = measurement.Trim();
            if (string.IsNullOrEmpty(ingredient) || string.IsNullOrEmpty(measurement)) {
                ViewBag.ErrorMessage = "Please enter both an ingredient and a measurement";
            } else { ViewBag.ErrorMessage = null; }

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
        }
        public ActionResult EditIngredientName(string oldName, string newName) {
            //oldName here is empty
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == oldName)
                    ing.name = newName;
            }
            var currentIngredient = new Ingredient(oldName, null);
            var updatedIngredient = new Ingredient(newName, null);
            oldAndNewIngredientNames.Add(oldName, newName);
            //still trying to decide which method would be the best way to do this, i like it with the Ingredient objects,
            //but I have to be able to access them outside of this method and I want to think through the design before I 
            //assign them to class as opposed to this method
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult EditIngredientMeasurement(string name, string oldMeasurement, string newMeasurement) {
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == name && ing.measurement == oldMeasurement) {
                    ing.measurement = newMeasurement;
                }
            }
            oldAndNewIngredientMeasurements.Add(oldMeasurement, newMeasurement);
            return Redirect("/home/recipe?name=" + currentRecipe.name);
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
edit a recipe name


NOT DONE YET: 
edit an ingredient name
cannot create an ingredient with null name
cannot create an ingredient/recipe with duplicate name
when you create a recipe/ingredient with duplicate name, display an error message on recipe and recipes that say there's a dupcliate name
use ViewBag.errorMessage = "Duplicate name error" or something

 
*/
