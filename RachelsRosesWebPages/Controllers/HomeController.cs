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
            var db = new DatabaseAccessRecipe();
            return db.MyRecipeBox();
        }
        public ActionResult RecipeBox() {
            var myRecipes = getRecipes();
            ViewBag.recipes = myRecipes;
            ViewBag.recipeboxcount = getRecipes().Count();
            return View();
        }
        public ActionResult Recipe(string name) {
            var rest = new MakeRESTCalls();
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbD = new DatabaseAccessDensityInformation();
            var dbC = new DatabaseAccessConsumption();
            if (string.IsNullOrEmpty(name))
                return Redirect("/home/RecipeBox");
            name = name.Trim();
            var distinctIngredientNamesSorted = dbI.myDistinctIngredientNamesSorted();
            var distinctIngredientClassifications = dbI.myDistinctIngredientClassificationsSorted();
            var distinctIngredientTypes = dbI.myDistinctIngredientTypesSorted();
            myDatabaseRecipe = getRecipes().First(x => x.name == name);
            currentRecipe = myDatabaseRecipe;
            ViewBag.currentingredient = currentIngredient;
            if (distinctIngredientNamesSorted.Count() == 0 || distinctIngredientNamesSorted == null) {
                ViewBag.ingredientnames = new List<string>();
            } else ViewBag.ingredientnames = distinctIngredientNamesSorted;
            if (distinctIngredientTypes.Count() == 0 || distinctIngredientTypes == null) {
                ViewBag.types = new List<string>();
            } else ViewBag.types = distinctIngredientTypes;
            if (distinctIngredientClassifications.Count() == 0 || distinctIngredientClassifications == null) {
                ViewBag.classifications = new List<string>();
            } else ViewBag.classifications = distinctIngredientClassifications;

            ViewBag.distinctsellingweights = dbC.getListOfDistinctSellingWeights();
            ViewBag.currentrecipe = currentRecipe;
            ViewBag.recipeboxcount = getRecipes().Count();
            //ViewBag.distinctingredienttypes = dbD.getListOfIngredientTypesFromDensityTable();
            if (!string.IsNullOrEmpty(currentIngredient.name)) {
                if (string.IsNullOrEmpty(currentIngredient.measurement))
                    ViewBag.itemresponselist = rest.GetListItemResponseNoSellingWeights(currentIngredient);
                else {
                    ViewBag.itemresponselist = rest.GetListItemResponses(currentIngredient);
                    ViewBag.itemresponselistcombined = rest.CombineItemResponses(currentIngredient);
                }
            } else {
                ViewBag.itemresponselist = new List<ItemResponse>();
                ViewBag.itemresponselistNoWeight = new List<ItemResponse>();
            }
            return View();
            /*
            foreach of these item responses in the list of item responses i think it would be wise to have an 
            algorithm to filter out the top 2 prices (or put them at the end of the list and gray them out so they're visible but not prominent...
            there's a product of 6 5 lb bags of king arthur organic unbleached flour, but in the itemr esponse name, it doesn't give any specification of packs in it's name, so my algorithm wouldn't be able to do anything with it   
            unless i took the largest price and divided it by the average of the other prices and compared them, seeing if it was a pack that way...  
            */
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
            ViewBag.currentitemresponselistnoweight = rest.GetListItemResponseNoSellingWeights(currentIngredient);
            return View();
        }
        public ActionResult EditIng(string updatedName, string updatedMeasurement, string updatedType, string updatedDensity, string updatedSellingWeight, string updatedSellingPrice, string updatedClassification, string updatedExpirationDate) {
            var dbI = new DatabaseAccessIngredient();
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

                    if (ing.expirationDate != dbI.convertStringMMDDYYYYToDateYYYYMMDD(updatedExpirationDate) && !(string.IsNullOrEmpty(updatedExpirationDate))) {
                        ing.expirationDate = dbI.convertStringMMDDYYYYToDateYYYYMMDD(updatedExpirationDate);
                    } else { updatedExpirationDate = dbI.convertDateToStringMMDDYYYY(ing.expirationDate); }

                    t.updateAllTables(currentIngredient, currentRecipe);
                    currentIngredient = t.queryAllRelevantTablesSQLByIngredientName(currentIngredient);
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
        public ActionResult CreateIngredient(string ingredient, string measurement, string classification, string type, string sellingweight, string sellingprice, string expirationdate) {
            var dbI = new DatabaseAccessIngredient();
            var db = new DatabaseAccess();
            ingredient = ingredient.Trim();
            measurement = measurement.Trim();
            if (string.IsNullOrEmpty(classification))
                classification = " ";
            else classification = classification.Trim();
            if (string.IsNullOrEmpty(type))
                throw new Exception("No type was given.");
            else type = type.Trim();
            sellingweight = sellingweight.Trim();
            if (!string.IsNullOrEmpty(sellingprice))
                sellingprice = sellingprice.Trim();
            var newIngredient = new Ingredient();
            if ((!(string.IsNullOrEmpty(ingredient)) && !(string.IsNullOrEmpty(measurement)))) {
                newIngredient.recipeId = currentRecipe.id;
                newIngredient.name = ingredient;
                newIngredient.measurement = measurement;
                newIngredient.classification = classification;
                newIngredient.typeOfIngredient = type;
                newIngredient.sellingWeight = sellingweight;
                newIngredient.expirationDate = dbI.convertStringMMDDYYYYToDateYYYYMMDD(expirationdate);
                currentRecipe.ingredients.Add(newIngredient);
                currentIngredient = newIngredient;
                db.insertIngredientIntoAllTables(currentIngredient, currentRecipe);
                var newIngredientData = db.queryAllRelevantTablesSQLByIngredientName(currentIngredient);
            }
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult assignCurrentIngredientNameForSearching(string ingredientName) {
            var rest = new MakeRESTCalls();
            currentIngredient = new Ingredient(rest.CapitalizeString(ingredientName));
            return Redirect(string.Format("/home/recipe?name={0}", currentRecipe.name));
        }
        public Ingredient ReturnCurrentIngredientFromQueriedItemResponse(string itemresponsename, string itemresponsesaleprice) {
            var rest = new MakeRESTCalls();
            var currentItemResponse = new ItemResponse();
            currentItemResponse.name = itemresponsename;
            currentItemResponse.salePrice = decimal.Parse(itemresponsesaleprice);
            currentIngredient = rest.SplitItemResponseName(currentItemResponse);
            return currentIngredient;
            //now i want to autopopuate the fields of name, density, selling price, selling weight...
            //i can give an attempt of guessing type by seeing if it matches any types... after all it's not final if i can put it as the placeholder or if
            //for now, we can just do placeholders... 
        }

        //so, if we're searching for something, it's not in our database... 
        //it may share a type, but it's not in the database...
        //so when i search for it, i can give the user what i think the name, the selling weigh, the selling price, type and classificatoin (i could provide a list of classifications too, similar to my density text database... 
        //so, first give a list of the name, sellingweight, sellingprice, priceperounce, type, density, classification... 
        //if the user confirms that the information is correct and gives the measurement, then i can add the ingredient (i should be able to call updatealltables... it should check to make sure i have if the ingredients table doesn't have this ingredient to insert the ingredient
        //i should also have a form that allows hte user to create a short name for it (eg: instead of King Arthur Bread Flour, the short name can be Bread Flour that the user inputs...)

        //let's get the other views working first...
        public ActionResult CreateRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            Recipe newrecipe = new Recipe(recipeTitle);
            var db = new DatabaseAccessRecipe();
            db.InsertRecipe(newrecipe);
            return Redirect("/home/RecipeBox");
        }
        public ActionResult DeleteRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            var db = new DatabaseAccessRecipe();
            var recipes = db.queryRecipes();
            foreach (var recipe in recipes) {
                if (recipe.name == recipeTitle)
                    currentRecipe = recipe;
                db.DeleteRecipeAndRecipeIngredients(currentRecipe);
            }
            return Redirect("/home/RecipeBox");
        }
        public ActionResult EditRecipeTitle(string newRecipeTitle) {
            var db = new DatabaseAccessRecipe();
            currentRecipe.name = newRecipeTitle;
            db.UpdateRecipe(currentRecipe);
            var myRecipeBox = getRecipes();
            return Redirect("/home/recipe?name=" + newRecipeTitle);
        }
        public ActionResult AdjustYield(int updatedYield) {
            var t = new DatabaseAccessRecipe();
            var convert = new ConvertMeasurement();
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
        public ActionResult DeleteIngredient(string name, string measurement) {
            name = name.Trim();
            var dbI = new DatabaseAccessIngredient();
            foreach (var ingredient in currentRecipe.ingredients) {
                if (ingredient.name == name && ingredient.measurement == measurement) {
                    dbI.DeleteIngredientFromIngredientTable(ingredient);
                    break;
                }
            }
            //manual check:
            var countRecipeIngredients = currentRecipe.ingredients.Count();
            var myIngredientsTable = dbI.queryAllIngredientsFromIngredientTable();
            var countIngredientTable = myIngredientsTable.Count();
            return Redirect("/home/recipe?name=" + currentRecipe.name);
        }
        public ActionResult InitializeDatabase() {
            var db = new DatabaseAccess();
            db.initializeDatabase();
            return Redirect("/home/recipeBox");
        }
        public ActionResult IngredientBox() {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var myIngredientBox = dbI.queryAllIngredientsFromIngredientTable();
            ViewBag.ingredientbox = myIngredientBox;
            ViewBag.fullingredientbox = db.queryAllTablesForAllIngredients(myIngredientBox);
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
