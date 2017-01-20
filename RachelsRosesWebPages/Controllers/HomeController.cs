using RachelsRosesWebPages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RachelsRosesWebPages.Controllers {
    public class HomeController : Controller {
        public static Recipe currentRecipe = new Recipe();
        public static Recipe myDatabaseRecipe = new Recipe();
        public static Ingredient currentIngredient = new Ingredient();
        public static void RegisterRoutes(RouteCollection routes) {
            routes.MapRoute(
                "Default",
                "home/recipes?name=" + currentRecipe.name); 
        }
        public List<Recipe> getRecipes() {
            var db = new DatabaseAccess();
            return db.MyRecipeBox();
        }
        public ActionResult RecipeBox() {
            var myRecipes = getRecipes();
            ViewBag.recipes = myRecipes;
            ViewBag.recipeboxcount = getRecipes().Count();
            return View();
        }
        public ActionResult Recipe(string name) {
            var t = new DatabaseAccess();
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/RecipeBox");
            name = name.Trim();
            myDatabaseRecipe = getRecipes().First(x => x.name == name);
            currentRecipe = myDatabaseRecipe;
            ViewBag.currentingredient = currentIngredient;
            ViewBag.currentingredienttable = t.getListOfDistintIngredients();  
            ViewBag.currentrecipe = currentRecipe;
            ViewBag.recipeboxcount = getRecipes().Count();
            return View();
        }
        public ActionResult Ingredient(string name, string measurement) {
            var rest = new MakeRESTCalls();
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/RecipeBox");
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(measurement))
                return Redirect("/home/recipe?name=" + currentRecipe.name);
            foreach (var ingredient in currentRecipe.ingredients) {
                if (ingredient.name == name && ingredient.measurement == measurement)
                    currentIngredient = ingredient;
            }
            ViewBag.currentrecipe = currentRecipe;
            ViewBag.currentingredient = currentIngredient;
            ViewBag.currentitemresponselist = rest.GetListItemResponses(currentIngredient);
            return View();
        }
        public ActionResult EditIng(string updatedName, string updatedMeasurement, string updatedType, string updatedDensity, string updatedSellingWeight, string updatedSellingPrice, string updatedClassification) {
            var t = new DatabaseAccess();
            var updatedDensityDecimal = 0m;
            var updatedSellingPriceDecimal = 0m;
            if (!string.IsNullOrEmpty(updatedDensity))
                updatedDensityDecimal = decimal.Parse(updatedDensity);
            if (!string.IsNullOrEmpty(updatedSellingPrice))
                updatedSellingPriceDecimal = decimal.Parse(updatedSellingPrice);
            var updatedIngredient = new Ingredient(updatedName, updatedMeasurement);
            foreach (var ing in currentRecipe.ingredients) {
                if (ing.name == currentIngredient.name) {
                    if (ing.name != updatedName && !(string.IsNullOrEmpty(updatedName))) {
                        ing.name = updatedName;
                    } else { updatedName = ing.name; }
                    if (ing.measurement != updatedMeasurement && !(string.IsNullOrEmpty(updatedMeasurement))) {
                        ing.measurement = updatedMeasurement;
                    } else { updatedMeasurement = ing.measurement; }
                    if (ing.typeOfIngredient != updatedType && !(string.IsNullOrEmpty(updatedType))) {
                        ing.typeOfIngredient = updatedType;
                    } else { updatedType = ing.typeOfIngredient; }
                    if (ing.density != updatedDensityDecimal && !(string.IsNullOrEmpty(updatedDensity))) {
                        ing.density = updatedDensityDecimal;
                    } else { updatedDensityDecimal = ing.density; }
                    if (ing.sellingWeight != updatedSellingWeight && !(string.IsNullOrEmpty(updatedSellingWeight))) {
                        ing.sellingWeight = updatedSellingWeight;
                    } else { updatedSellingWeight = ing.sellingWeight; }
                    if (ing.sellingPrice != updatedSellingPriceDecimal && !(string.IsNullOrEmpty(updatedSellingPrice))) {
                        ing.sellingPrice = updatedSellingPriceDecimal;
                    } else { updatedSellingPriceDecimal = ing.sellingPrice; }
                    if (ing.classification != updatedClassification && !(string.IsNullOrEmpty(updatedClassification))) {
                        ing.classification = updatedClassification;
                    } else { updatedClassification = ing.classification; }
                    t.updateAllTables(currentIngredient, currentRecipe);
                    currentIngredient = t.queryAllTablesForIngredient(currentIngredient);
                }
            }
            return Redirect("/home/ingredient?name=" + currentIngredient.name + "&measurement=" + currentIngredient.measurement);
        }
        public ActionResult ResetSellingPrice() {
            var t = new DatabaseAccess();
            var rest = new MakeRESTCalls();
            currentIngredient.sellingPrice = rest.GetItemResponse(currentIngredient).salePrice;
            t.updateAllTables(currentIngredient, currentRecipe);
            return Redirect("/home/ingredient?name=" + currentIngredient.name + "&measurement=" + currentIngredient.measurement);
        }
        //add selling weight to the recipes page
        public ActionResult CreateIngredient(string ingredient, string measurement, string classification, string type, string sellingweight, string sellingprice) {
            var db = new DatabaseAccess();
            ingredient = ingredient.Trim();
            measurement = measurement.Trim();
            if (string.IsNullOrEmpty(classification))
                classification = " ";
            else classification = classification.Trim(); 
            if (string.IsNullOrEmpty(type))
                type = " "; 
            else type = type.Trim();
            sellingweight = sellingweight.Trim();
            sellingprice = sellingprice.Trim();
            var newIngredient = new Ingredient();
            if ((!(string.IsNullOrEmpty(ingredient)) || !(string.IsNullOrEmpty(measurement))) && (!(string.IsNullOrEmpty(classification)) && !(string.IsNullOrEmpty(type)))) {
                newIngredient.name = ingredient;
                newIngredient.measurement = measurement;
                newIngredient.classification = classification;
                newIngredient.typeOfIngredient = type;
                newIngredient.recipeId = currentRecipe.id;
                newIngredient.sellingWeight = sellingweight;
                if (!string.IsNullOrEmpty(sellingprice))
                    newIngredient.sellingPrice = decimal.Parse(sellingprice);
                currentRecipe.ingredients.Add(newIngredient);
                currentIngredient = newIngredient;
                db.insertIngredient(currentIngredient, currentRecipe);
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult
        public ActionResult CreateRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            Recipe newrecipe = new Recipe(recipeTitle);
            var db = new DatabaseAccess();
            db.InsertRecipe(newrecipe);
            return Redirect("/home/RecipeBox");
        }
        public ActionResult DeleteRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            var db = new DatabaseAccess();
            var recipes = db.queryRecipes();
            foreach (var recipe in recipes) {
                if (recipe.name == recipeTitle)
                    currentRecipe = recipe;
                db.DeleteRecipeAndRecipeIngredients(currentRecipe);
            }
            return Redirect("/home/RecipeBox");
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
            var convert = new ConvertMeasurement();
            //int n;
            if (currentRecipe.yield == 0) {
                currentRecipe.yield = updatedYield;
            } else {
                var oldYield = currentRecipe.yield;
                currentRecipe.yield = updatedYield;
                t.UpdateRecipeYield(currentRecipe);
            }
            t.UpdateRecipe(currentRecipe);
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult DeleteIngredient(string name) {
            name = name.Trim();
            var db = new DatabaseAccess();
            var myIngredientsTable = db.queryIngredients();
            foreach (var ingredient in myIngredientsTable) {
                if (ingredient.name == name) {
                    db.DeleteIngredientFromAllRelevantTables(ingredient);
                }
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult IngredientBox() {
            return View(); 
        }
        public ActionResult DensityTable() {
            return View();
        }
        public ActionResult ConsumptionTable() {
            return View();
        }
        public ActionResult CostTable() {
            return View();
        }
        public ActionResult DensityInformationTable() {
            return View();
        }
        public ActionResult ReadMeInformation() {
            //there's some quirks, like needing to put in the type of ingredient and the classification that will be really helpful to someone new coming in, 
            //this needs to happen 
            return View(); 
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
