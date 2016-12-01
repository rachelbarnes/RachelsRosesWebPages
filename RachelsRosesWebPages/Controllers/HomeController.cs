﻿using System;
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
        public Ingredient(string _name) {
            name = _name;
        }
        public Ingredient() { }
    }
    public class Recipe {
        public string name;
        public List<Ingredient> ingredients;
        public int yield;
        public Recipe(string _name) {
            name = _name;
            ingredients = new List<Ingredient>();
            yield = 0;
        }
    }
    public class Error {
        public string repeatedRecipeName = "This recipe is already in your recipe box.";
        public Error() { }
    }
    public class HomeController : Controller {
        public static List<Recipe> recipes = new List<Recipe>();
        public static Recipe currentRecipe = null;
        public static Ingredient currentIngredient = null;
        public ActionResult Recipes() {
            ViewBag.recipes = recipes;
            return View();
        }
        public ActionResult Recipe(string name) {
            var error = new Error(); 
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            name = name.Trim();
            currentRecipe = recipes.First(x => x.name == name);
            ViewBag.ingredients = currentRecipe.ingredients;
            ViewBag.recipename = currentRecipe.name;
            ViewBag.currentrecipe = currentRecipe;
            ViewBag.repeatedrecipetitle = error.repeatedRecipeName; 
            return View();
        }
        public ActionResult Ingredient(string name, string measurement) {
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(measurement))
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            ViewBag.currentrecipe = currentRecipe.name;
            return View();
        }
        public ActionResult EditIng(string updatedName, string updatedMeasurement) {
            if ((string.IsNullOrEmpty(updatedName)) && (string.IsNullOrEmpty(updatedMeasurement))) {
                ViewBag.ErrorMessage = "Please enter an ingredient name and measurement";
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            }
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name != updatedName && !(string.IsNullOrEmpty(updatedName))) {
                    ing.name = updatedName;
                } else { updatedName = ing.name; }
                if (ing.measurement != updatedMeasurement && !(string.IsNullOrEmpty(updatedMeasurement))) {
                    ing.measurement = updatedMeasurement;
                } else { updatedMeasurement = ing.measurement; }
                currentIngredient = ing;
            }
            return Redirect("/home/ingredient?name=" + currentIngredient.name + "&measurement=" + currentIngredient.measurement);
        }
        public ActionResult DeleteIngredient(string ingredient) {
            currentRecipe.ingredients = currentRecipe.ingredients.Where(x => x.name != ingredient).ToList();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult CreateIngredient(string ingredient, string measurement) {
            ingredient = ingredient.Trim();
            measurement = measurement.Trim();
            if (!(string.IsNullOrEmpty(ingredient)) || !(string.IsNullOrEmpty(measurement))) {
                Ingredient newingredient = new Ingredient(ingredient, measurement);
                currentRecipe.ingredients.Add(newingredient);
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        //i have to have a functionality and action if a recipe is repeated. 
        public ActionResult CreateRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            Recipe newrecipe = new Recipe(recipeTitle);
            recipes.Add(newrecipe);
            return Redirect("/home/recipes");
        }
        public ActionResult DeleteRecipe(string recipeTitle) {
            recipes = recipes.Where(x => x.name != recipeTitle).ToList();
            return Redirect("/home/recipes");
        }
        public ActionResult EditRecipeTitle(string newRecipeTitle) {
            currentRecipe.name = newRecipeTitle;
            //every redirect clears out the viewbag... as a notice and warning
            return Redirect("/home/recipe?name=" + newRecipeTitle);
        }
        public ActionResult AdjustYield(int updatedYield) {
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

//view looks up the view for that action method, it's looking up View called Recipe
//View looks at everything in the ViewBag and renders that
//the client/browser requests CreateRecipe, and CreateRecipe replies with go to "/home/recipes"
//browser automatically goes to "/home/recipes" and displays the recipes


/*
DONE: 
cannot create an ingredient or recipe names that has space after or before (trim ingredient names)
edit a recipe name
edit an ingredient name
edit an ingredient measurement

NOT DONE YET: 
when you create a recipe/ingredient with duplicate name, display an error message on recipe and recipes that say there's a dupcliate name
use ViewBag.errorMessage = "Duplicate name error" or something
create comments for the recipes (I'm still trying to figure out the best way to do this... 

I would like to eventually have all the general information under each ingredient page...
    the price and selling weight, the desnity, how much you have in your MyPantry logs... then it would make more sense for it to have it's own module and everything



cannot create an ingredient with null name: thinking about this... i actually want to allow multiples of the same name, like when a more complicated
    recipe calls for butter 2 or three times because it's needed in different parts of the recipe... which i have recipes that i want to use 1/4 cup butter twice, which makes the second after this
    one in the same category of maybe not wanting to get rid of (however, that design is not the same with the recipe names... that's impt to be different and nonrepetitive

*/

/*
Questions for Steve:
What is the difference between setting an object as a new instance of an object vs setting an object as null?
ie: 
    public static Ingredient ex = null; 
    public static Ingredient ex = new Ingredient(); 

How do you set up a database for your program? Is it easier to set up a database and get some tables created before you add it into your program, or is it 
    generally easier to do this with an empty database? 

Is there a specific way you have to make classes in the MVC model outside of controllers, views and models? 
    (ie i want to have a conversion class, one that I can manipulate measurements and values and yielding sizes, but I don't want to clutter up a 
        controller with such functionalities and methods (it's already getting kind of cluttered for my taste unfortunately)
    I don't know where to put it in the project...

Is there a better way to keep track of the ViewBag variables between pages and methods other than having class properties (31-43)



Left to do: 

debug the adjust ingredients action method... it changed all of the ingredient measurements to the same measurement, although correctly adjusted. 

Do i need the first condition in Parse in ParseFraction? 
*/
