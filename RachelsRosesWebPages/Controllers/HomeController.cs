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
        public Ingredient(string _name, string _measurement) {
            name = _name;
            measurement = _measurement;
            recipeId = 0; 
            //what is the best way to assign this recipeId? 
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
        public List<Recipe> getRecipes() {
            var db = new DatabaseAccess();
            return db.queryRecipe("recipes");
            //i can see the abstraction pretty clearly now... i can only imagine if all the needed sql commands were in the controller, how absurd that would be... woah
        }
        public static Recipe currentRecipe = null;
        public static Ingredient currentIngredient = null;
        public ActionResult Recipes() {
            ViewBag.currentrecipe = currentRecipe;
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
            //throw new NotImplementedException("");
            //this doesn't work, it throws the exception
            ingredient = ingredient.Trim(); 
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
            //try {
                recipeTitle = recipeTitle.Trim();
                var db = new DatabaseAccess();
                db.DeleteRecipe(recipeTitle);
            //} catch {
            //    throw new NotImplementedException("create a new db function for delteing where the name matches and call that");
            //}; 
            return Redirect("/home/recipes");
        }
        public ActionResult EditRecipeTitle(string newRecipeTitle) {
            currentRecipe.name = newRecipeTitle;
            var db = new DatabaseAccess();
            db.UpdateRecipe(currentRecipe);
            return Redirect("/home/recipe?name=" + newRecipeTitle);
        }
        //every redirect clears out the viewbag... as a notice and warning
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

//view looks up the view for that action method, it's looking up View called Recipe
//view looks at everything in the ViewBag and renders that

/*
DONE: 
cannot create an ingredient or recipe names that has space after or before (trim ingredient names)
edit a recipe name
edit an ingredient name
edit an ingredient measurement

NOT DONE YET: 
when you create a recipe/ingredient with duplicate name, display an error message on recipe and recipes that say there's a dupcliate name
use ViewBag.errorMessage = "Duplicate name error" or something
create comments for the recipes 
    I have bigger plans for the comments (weight ratios, general notes about the ingredients, density and what the measurement of an ingredient is for the density

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


BUGS: 
when I update an ingredient name or measurement, i update the rest of the ingredients in the recipe to that name and measurement
there's a bug with the conversion of the ingredient measurements, sometimes it gets empty... 



Left to do: 
 
Do i need the first condition in Parse in ParseFraction? 
    have a database that holds the original decimal to retain precision when using the multiplier... i don't want to use a 1/8 when i'm really trying
        to use a multiplier for .1667... so i need to be able to apply it to the decimal value instead of the just the fraction that comes out of it, the precision is impt for me, even if it may remain inconsequencial to this

When i have access to my density database (which can either be done now with reading a file from my computer or having the information in a SQL
    density database...), i can covert the ounces to cups based on their density... then i can choose to put the ounces and cups or density or 
    whatever information in the ingredient comments or choose to put the ingredient measurement back into ounces


*/
