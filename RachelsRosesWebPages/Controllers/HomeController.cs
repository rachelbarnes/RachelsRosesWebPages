using RachelsRosesWebPages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RachelsRosesWebPages.Controllers;
namespace RachelsRosesWebPages{

    public class Error {
        public string repeatedRecipeName = "This recipe is already in your recipe box.";
        public Error() { }
    }
    public class HomeController : Controller {
        public List<Recipe> getRecipes() {
            var db = new DatabaseAccess();
            return db.queryRecipe();
        }
        //public static List<Recipe> recipes = new List<Recipe>();
        public static Recipe currentRecipe = null;
        public static Ingredient currentIngredient = null;
        public ActionResult Recipes() {
            ViewBag.recipes = getRecipes();
            return View();
        }
        public ActionResult Recipe(string name) {
            var error = new Error();
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            name = name.Trim();
            currentRecipe = getRecipes().First(x => x.name == name);
            ViewBag.ingredients = currentRecipe.ingredients;
            ViewBag.recipename = currentRecipe.name;
            ViewBag.currentrecipe = currentRecipe;
            ViewBag.repeatedrecipetitle = error.repeatedRecipeName;
            return View();
        }
        public ActionResult Ingredient(string name, string measurement) {
            throw new NotImplementedException("");
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(measurement))
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            foreach (var ingredient in currentRecipe.ingredients) {
                if (ingredient.name == name && ingredient.measurement == measurement)
                    currentIngredient = ingredient;
            }
            ViewBag.currentrecipe = currentRecipe;
            ViewBag.currentingredient = currentIngredient;
            return View();
        }
        public ActionResult EditIng(string updatedName, string updatedMeasurement) {
            throw new NotImplementedException("");
            if ((string.IsNullOrEmpty(updatedName)) && (string.IsNullOrEmpty(updatedMeasurement))) {
                ViewBag.ErrorMessage = "Please enter an ingredient name and measurement";
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            }
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == currentIngredient.name) {
                    //putting this extra condition in here actually worked really well, better than expected, nice
                    if (ing.name != updatedName && !(string.IsNullOrEmpty(updatedName))) {
                        ing.name = updatedName;
                    } else { updatedName = ing.name; }
                    if (ing.measurement != updatedMeasurement && !(string.IsNullOrEmpty(updatedMeasurement))) {
                        ing.measurement = updatedMeasurement;
                    } else { updatedMeasurement = ing.measurement; }
                    currentIngredient = ing;
                }
            }
            return Redirect("/home/ingredient?name=" + currentIngredient.name + "&measurement=" + currentIngredient.measurement);
        }
        public ActionResult DeleteIngredient(string ingredient) {
            currentRecipe.ingredients = currentRecipe.ingredients.Where(x => x.name != ingredient).ToList();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateIngredient(string ingredient, string measurement) {
            throw new NotImplementedException("");
            measurement = measurement.Trim();
            if (!(string.IsNullOrEmpty(ingredient)) || !(string.IsNullOrEmpty(measurement))) {
                Ingredient newingredient = new Ingredient(ingredient, measurement);
                currentRecipe.ingredients.Add(newingredient);
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            Recipe newrecipe = new Recipe(recipeTitle);
            var db = new DatabaseAccess();
            db.InsertRecipe(newrecipe);
            return Redirect("/home/recipes");
        }
        public ActionResult DeleteRecipe(string recipeTitle) {
            //recipes = recipes.Where(x => x.name != recipeTitle).ToList();
            throw new NotImplementedException("create a new db function for delteing where the name matches and call that");
            return Redirect("/home/recipes");
        }
        public ActionResult EditRecipeTitle(string newRecipeTitle) {
            currentRecipe.name = newRecipeTitle;
            var db = new DatabaseAccess();
            db.UpdateRecipe(currentRecipe);
            return Redirect("/home/recipe?name=" + newRecipeTitle);
        }
        //every redirect clears out the viewbag... as a notice and warning

        //there's a bug here... i'm still trying to find the pattern... 
        public ActionResult AdjustYield(int updatedYield) {
            throw new NotImplementedException("Adjust reader to also look up and populate ingredients on a recipe, and update and insert to also save them");
            var convert = new Convert();
            if (currentRecipe.yield == 0) {
                currentRecipe.yield = updatedYield;
            } else {
                var oldYield = currentRecipe.yield;
                currentRecipe.yield = updatedYield;
                foreach (var ing in currentRecipe.ingredients) {
                    ing.measurement = convert.AdjustIngredientMeasurement(ing.measurement, oldYield, currentRecipe.yield);
                }
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
    }
}

