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
        public Ingredient currentIngredient = null;
        public Ingredient updatedIngredient = null;
        public static List<Ingredient> currentListIngredients = new List<Ingredient>();
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
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/recipes");
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(measurement))
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            Ingredient currentIng = new Ingredient(name, measurement);
            foreach (var ing in currentListIngredients) {
                if (ing == currentIng)
                    ViewBag.DuplicateIngredientNameErrorMessage = "This ingredient is already in your ingredients list.";
                if (!(ing.name == name) && !(ing.measurement == measurement))
                    currentListIngredients.Add(currentIng);
            }
            ViewBag.currentrecipe = currentRecipe.name;
            ViewBag.currentingname = name;
            ViewBag.currentingmeasurement = measurement;
            if (updatedIngredient != null) {
                ViewBag.updatedingname = updatedIngredient.name;
                ViewBag.updatedingmeasurement = updatedIngredient.measurement;
            }
            return View();
        }
        public ActionResult EditIng(string oldName, string updatedName, string oldMeasurement, string updatedMeasurement) {
            //i keep getting really weird responses from my ViewBag... there's gotta be something weird happening here, despite 
            //there being normal/expected results for lines 66-68 when i debug... 
            //but the weird results come from after I edit the ingredient name and/or measurement... which means it has to be 
            //here from when this method is called.
            if ((string.IsNullOrEmpty(updatedName)) && (string.IsNullOrEmpty(updatedMeasurement))) {
                ViewBag.ErrorMessage = "Please enter an ingredient name and measurement";
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            }
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name != updatedName && !(string.IsNullOrEmpty(updatedName))) {
                    ing.name = updatedName;
                    //updatedIngName = updatedName;
                } else { updatedName = oldName; }
                if (ing.measurement != updatedMeasurement && !(string.IsNullOrEmpty(updatedMeasurement))) {
                    ing.measurement = updatedMeasurement;
                    //updatedIngMeasurement = updatedMeasurement;
                } else { updatedMeasurement = oldMeasurement; }
                //this should replace the old string with the new string for either the name and/or the measurement... 
                //currentIngredient is then equaled to the ing, which is the current ingredient being evaluted for the name and mesaurement updates... 
                //so is there something wrong here? 
                updatedIngredient = ing;
            }
            return Redirect("/home/ingredient?name=" + updatedIngredient.name + "&measurement=" + updatedIngredient.measurement);
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
edit an ingredient name

NOT DONE YET: 
edit an ingredient measurement
cannot create an ingredient with null name
cannot create an ingredient/recipe with duplicate name
when you create a recipe/ingredient with duplicate name, display an error message on recipe and recipes that say there's a dupcliate name
use ViewBag.errorMessage = "Duplicate name error" or something
 

I would like to eventually have all the general information under each ingredient page...
    the price and selling weight, the desnity, how much you have in your MyPantry logs... then it would make more sense for it to have it's own module and everything
*/
/*
Bugs: 
    when you don't have an ingredient name and you click the link to view the details of the ingredient, it brings you back to the recipe title
        and you can't delete the ingredient because no ingredient matches ""... need to put a conditional to direct you in the proper directino for 
        when that happens

*/
