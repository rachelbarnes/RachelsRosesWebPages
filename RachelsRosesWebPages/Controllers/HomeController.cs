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
        public Ingredient(string _name) {
            name = _name;
        }
        public Ingredient() { }
        //the above is nice, but I can't do a measurement one too... the parameters are both strings, so this would only work for either the name or hte measurement
        //there's no overloading the method with two different parameters, one constructor for each if both the parameter types are strings (or asre the same data type, basically)
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
        public Ingredient currentIngredientRatio = null;
        public static List<Ingredient> currentIngredients = new List<Ingredient>();
        public static string updatedIngName = "";
        public static string updatedIngMeasurement = "";
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
            return View();
        }
        public ActionResult Ingredient(string name, string measurement) {
            Ingredient currentIng = new Ingredient(name, measurement);
            foreach (var ing in currentIngredients) {
                if (ing == currentIng) {
                    ViewBag.DuplicateIngredientNameErrorMessage = "This ingredient is already in your ingredients list.";
                }
                if (!(ing.name == name) && !(ing.measurement == measurement)) {
                    currentIngredients.Add(currentIng);
                }
            }
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            ViewBag.currentrecipe = currentRecipe.name;
            ViewBag.currentingname = currentIng.name;
            ViewBag.currentingmeasurement = currentIng.measurement;
            ViewBag.updatedname = updatedIngName;
            return View();
        }
        public ActionResult EditIngName(string oldName, string updatedName) {
            if (string.IsNullOrEmpty(updatedName))
                ViewBag.ErrorMessage = "Please enter an ingredient name and measurement";
            foreach (var ing in currentRecipe.ingredients) {
                currentIngredients.Add(ing);
                if (ing.name == oldName)
                    ing.name = updatedName;
            }
            updatedIngName = updatedName;
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult EditIngMeasurement(string oldMeasurement, string updatedMeasurement) {
            if (string.IsNullOrEmpty(updatedMeasurement)) {
                ViewBag.ErrorMessage = "Please enter an ingredient name and measurement";
            }
            foreach (var ing in currentRecipe.ingredients) {
                currentIngredients.Add(ing);
                if (ing.measurement == oldMeasurement)
                    ing.measurement = updatedMeasurement;
            }
                updatedIngMeasurement = updatedMeasurement;
            return Redirect("/home/recipe?name=" + currentRecipe.name); 
        }
        public ActionResult EditIngredientName(string oldName, string newName) {
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == ViewBag.oldName) {
                    ing.name = newName;
                    ViewBag.newName = ing.name;
                }
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult EditIngredientMeasurement(string name, string oldMeasurement, string newMeasurement) {
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == name && ing.measurement == oldMeasurement) {
                    ing.measurement = newMeasurement;
                }
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
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
/*
i've been trying to work on this edit ingredient name for about a day and a half now, the problem rising when 
    i'm trying to access the newName that I enter, which I can't seem to do...

maybe, for a design choice, I could give Ingredient a new field of comments, and have a new view for ingredients that
    shows all of the information for the ingredients listed, and then at the bottom of a recipe, I could have all the comments
    listed out...

unfortunatley, the initial way I see this is having a huge method doing everything so i can access ViewBag variables and such...
    but if this is giving me this much trouble without fruit and Steve isn't here to ask about it, then this may be my best
    solution to this point unfortunatley (or forutnatley)

I'm not very fond of having someone have to click a link to go to another page for each individual ingredient, but there can
    always be "feature fixes" and bug fixes later on... i just really want to get this woi

Dang it... every time i try to find a particular solution, it doesn't work... alright, gotta start working on a view for ingredients

*/


/*
Bugs: 
    when you don't have an ingredient name and you click the link to view the details of the ingredient, it brings you back to the recipe title
        and you can't delete the ingredient because no ingredient matches ""... need to put a conditional to direct you in the proper directino for 
        when that happens

*/
