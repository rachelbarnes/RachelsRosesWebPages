using RachelsRosesWebPages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RachelsRosesWebPages.Controllers {
    public class Ingredient {
        public string name;
        public string measurement;
        public int recipeId;
        public int ingredientId;
        public Ingredient(string _name, string _measurement) {
            name = _name;
            measurement = _measurement;
            recipeId = 0;
            ingredientId = 0;

        }
        public Ingredient(string _name) {
            name = _name;
        }
        public Ingredient() { }
    }
    public class Recipe {
        public string name;
        public int id;
        public List<Ingredient> ingredients;
        public int yield;
        public Recipe(string _name) {
            name = _name;
            id = 0;
            ingredients = new List<Ingredient>();
            yield = 0;
        }
        public Recipe(int _id) {
            name = "";
            id = _id;
            ingredients = new List<Ingredient>();
            yield = 0;
        }
        public Recipe() { }
    }
    public class Error {
        public string repeatedRecipeName = "This recipe is already in your recipe box.";
        public Error() { }
    }
    public class HomeController : Controller {
        public static Recipe currentRecipe = new Recipe();
        public static Recipe myDatabaseRecipe = new Recipe();
        public static Ingredient currentIngredient = new Ingredient();
        public List<Recipe> getRecipes() {
            var db = new DatabaseAccess();
            return db.queryRecipes();
        }
        public ActionResult Recipes() {
            var myRecipes = getRecipes();
            ViewBag.recipes = myRecipes;
            return View();
        }
        public ActionResult Recipe(string name) {
            var t = new DatabaseAccess();
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            name = name.Trim();
            myDatabaseRecipe = getRecipes().First(x => x.name == name);
            if (currentRecipe.ingredients == null || currentRecipe.ingredients.Count() == 0) {
                currentRecipe.ingredients = myDatabaseRecipe.ingredients; 
            } else {
                myDatabaseRecipe.ingredients = currentRecipe.ingredients; 
            }
            currentRecipe.name = myDatabaseRecipe.name;
            currentRecipe.id = myDatabaseRecipe.id;
            currentRecipe.yield = myDatabaseRecipe.yield;

            ViewBag.currentingredient = currentIngredient;
            ViewBag.currentrecipe = currentRecipe;
            return View();
        }
        public ActionResult Ingredient(string name, string measurement) {
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
            var t = new DatabaseAccess();
            if ((string.IsNullOrEmpty(updatedName)) && (string.IsNullOrEmpty(updatedMeasurement))) {
                ViewBag.ErrorMessage = "Please enter an ingredient name and measurement";
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            }
            var updatedIngredient = new Ingredient(updatedName, updatedMeasurement);
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == currentIngredient.name) {
                    if (ing.name != updatedName && !(string.IsNullOrEmpty(updatedName))) {
                        ing.name = updatedName;
                    } else { updatedName = ing.name; }
                    if (ing.measurement != updatedMeasurement && !(string.IsNullOrEmpty(updatedMeasurement))) {
                        ing.measurement = updatedMeasurement;
                    } else { updatedMeasurement = ing.measurement; }
                    currentIngredient = ing;
                    t.UpdateIngredient(currentIngredient);
                }
            }
            return Redirect("/home/ingredient?name=" + currentIngredient.name + "&measurement=" + currentIngredient.measurement);
        }
        public ActionResult DeleteIngredient(string ingredient) {
            currentRecipe.ingredients = currentRecipe.ingredients.Where(x => x.name != ingredient).ToList();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateIngredient(string ingredient, string measurement) {
            var db = new DatabaseAccess();
            ingredient = ingredient.Trim();
            measurement = measurement.Trim();
            var newIngredient = new Ingredient();
            if (!(string.IsNullOrEmpty(ingredient)) || !(string.IsNullOrEmpty(measurement))) {
                newIngredient.name = ingredient;
                newIngredient.measurement = measurement;
                newIngredient.recipeId = currentRecipe.id;
                currentRecipe.ingredients.Add(newIngredient);
                currentIngredient = newIngredient;
                db.InsertIngredient(currentIngredient, currentRecipe);
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
            recipeTitle = recipeTitle.Trim();
            var db = new DatabaseAccess();
            db.DeleteRecipe(recipeTitle);
            return Redirect("/home/recipes");
        }
        public ActionResult EditRecipeTitle(string newRecipeTitle) {
            var db = new DatabaseAccess();
            currentRecipe.name = newRecipeTitle;
            db.UpdateRecipe(currentRecipe);
            var myRecipeBox = getRecipes();
            return Redirect("/home/recipe?name=" + newRecipeTitle);
        }
        public ActionResult AdjustYield(int updatedYield) {
            var t = new DatabaseAccess();
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
            t.UpdateRecipe(currentRecipe);
            return Redirect("/home/recipe?name=" + currentRecipe.name);
            //as a note to myself, a pattern to follow, if you're updating an ingredient or a recipe, or anything of the matter,
            //whenever an item is updated, update the database, and then call the database for the ingredient or the recipe!!!
        }
    }
}


/*
Every redirect clears out the viewbag... as a notice and warning

Have a database that holds the original decimal to retain precision when using the multiplier... i don't want to use a 1/8 when i'm really trying
    to use a multiplier for .1667... so i need to be able to apply it to the decimal value instead of the just the fraction that comes out of it, the precision is impt for me, even if it may remain inconsequencial to this

When i have access to my density database (which can either be done now with reading a file from my computer or having the information in a SQL
    density database...), i can covert the ounces to cups based on their density... then i can choose to put the ounces and cups or density or 
    whatever information in the ingredient comments or choose to put the ingredient measurement back into ounces

I would like to eventually have all the general information under each ingredient page...
    the price and selling weight, the desnity, how much you have in your MyPantry logs... then it would make more sense for it to have it's own module and everything

view looks up the view for that action method, it's looking up View called Recipe
view looks at everything in the ViewBag and renders that
*/
