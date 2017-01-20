using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccess {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public ItemResponse returnItemResponse(Ingredient i) {
            var rest = new MakeRESTCalls();
            return rest.GetItemResponse(i);
        }
        public static ItemResponse myItemResponse = new ItemResponse();
        //helper functions: 
        public void executeVoidQuery(string command, Func<SqlCommand, SqlCommand> modifyCommand) {
            var con = new SqlConnection(connString);
            try {
                con.Open();
                var cmd = new SqlCommand(command, con);
                modifyCommand(cmd).ExecuteNonQuery();
                con.Close();
            } catch (Exception e) {
                Console.WriteLine("Query Failed somehow");
                throw e;
            }
        }
        /*
        to do tomorrow/next:
        need to allow eggs for the selling weight, need to make sure the conversion works for eggs,
        convert for getting into fractions, instead of just having the decimals... 

        eggs are always the black sheep... i gotta be able to do stuff with this. :)
        */
        public List<T> queryItems<T>(string command, Func<SqlDataReader, T> convert) {
            var sqlConnection1 = new SqlConnection(connString);
            var cmd = new SqlCommand(command, sqlConnection1);
            sqlConnection1.Open();
            var reader = cmd.ExecuteReader();
            List<T> items = new List<T>();
            while (reader.Read()) {
                items.Add(convert(reader));
            }
            sqlConnection1.Close();
            return items;
        }
        //recipe table methods: 
        public List<Recipe> queryRecipes() {
            var count = 1;
            var MyRecipeBox = queryItems("select * from recipes", reader => {
                var recipe = new Recipe(reader["name"].ToString());
                recipe.yield = (int)reader["yield"];
                recipe.aggregatedPrice = (decimal)reader["aggregated_price"];
                return recipe;
            });
            foreach (var recipe in MyRecipeBox) {
                recipe.id = count++;
            }
            return MyRecipeBox;
        }
        public void UpdateRecipe(Recipe r) {
            var myRecipes = queryRecipes();
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id) {
                    if (recipe.yield != r.yield) {
                        recipe.yield = r.yield;
                        //UpdateRecipeYield(recipe);
                    }
                }
            }
            var commandText = "update recipes set name=@name, yield=@yield, aggregated_price=@aggregated_price where recipe_id=@rid;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                cmd.Parameters.AddWithValue("@aggregated_price", r.aggregatedPrice);
                return cmd;
            });
        }
        public void InsertRecipe(Recipe r) {
            var commandText = "Insert into recipes (name, yield, aggregated_price) values (@name, @yield, @aggregated_price);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                cmd.Parameters.AddWithValue("@aggregated_price", r.aggregatedPrice);
                return cmd;
            });
        }
        public Recipe GetFullRecipe(Recipe r) {
            var aggregatedPrice = 0m;
            var myRecipeBox = queryRecipes();
            var myIngredients = queryIngredients();
            var myRecipeIngredients = new List<Ingredient>();
            var myRecipe = new Recipe();
            foreach (var recipe in myRecipeBox) {
                if (recipe.id == r.id) {
                    myRecipe = recipe;
                    break;
                }
            }
            foreach (var ingredient in myIngredients) {
                if (ingredient.recipeId == myRecipe.id) {
                    var currentIngredient = queryAllTablesForIngredient(ingredient);
                    aggregatedPrice += currentIngredient.priceOfMeasuredConsumption;
                    myRecipe.ingredients.Add(ingredient);
                }
            }
            myRecipe.aggregatedPrice = aggregatedPrice;
            myRecipe.pricePerServing = ReturnRecipePricePerServing(myRecipe);
            return myRecipe;
        }
        public List<Recipe> MyRecipeBox() {
            var myRecipes = queryRecipes();
            var myRecipeBox = new List<Recipe>();
            foreach (var recipe in myRecipes) {
                recipe.ingredients = ReturnRecipeIngredients(recipe);
                recipe.aggregatedPrice = ReturnFullRecipePrice(recipe);
                recipe.pricePerServing = ReturnRecipePricePerServing(recipe);
                myRecipeBox.Add(recipe);
            }
            return myRecipeBox;
        }
        public void GetFullRecipePrice(Recipe r) {
            var myIngredients = GetFullRecipe(r).ingredients;
            foreach (var ing in myIngredients) {
                if (ing.recipeId == r.id) {
                    getIngredientMeasuredPrice(ing, r);
                    r.ingredients.Add(ing);
                    updateAllTables(ing, r);
                    var currentIngredient = queryAllTablesForIngredient(ing);
                }
            }
            var aggregatedPrice = 0m;
            foreach (var ing in r.ingredients) {
                aggregatedPrice += ing.priceOfMeasuredConsumption;
            }
            r.aggregatedPrice = aggregatedPrice;
            UpdateRecipe(r);
        }
        public decimal ReturnRecipePricePerServing(Recipe r) {
            //var pricePerServing = 0m;
            var myRecipes = queryRecipes();
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id)
                    if (recipe.yield == 0)
                        r.pricePerServing = 0m;
                    else r.pricePerServing = Math.Round((recipe.aggregatedPrice / recipe.yield), 2);
            }
            return r.pricePerServing;
        }
        public List<Ingredient> ReturnRecipeIngredients(Recipe r) {
            var myIngredients = queryIngredients();
            foreach (var ing in myIngredients) {
                if (ing.recipeId == r.id) {
                    queryAllTablesForIngredient(ing);
                    r.ingredients.Add(ing);
                }
            }
            return r.ingredients;
        }
        public decimal ReturnFullRecipePrice(Recipe r) {
            var myIngredients = GetFullRecipe(r).ingredients;
            var aggregatedPrice = 0m;
            foreach (var ing in r.ingredients) {
                aggregatedPrice += ing.priceOfMeasuredConsumption;
            }
            r.aggregatedPrice = aggregatedPrice;
            UpdateRecipe(r);
            return aggregatedPrice;
        }
        public void DeleteRecipeAndRecipeIngredients(Recipe r) {
            r.name = r.name.Trim();
            var myRecipe = GetFullRecipe(r);
            var myIngredients = queryIngredients();
            foreach (var ingredient in myRecipe.ingredients) {
                DeleteIngredientFromAllRelevantTables(ingredient);
            }
            var delete = "DELETE FROM recipes WHERE name=@title";
            executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@title", r.name);
                return cmd;
            });
        }
        public void DeleteIngredientFromIngredientTable(Ingredient i) {
            i.name = i.name.Trim();
            var delete = "delete from ingredients where ing_id=@ing_id";
            executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                return cmd;
            });
        }
        public void DeleteIngredientFromCostTable(Ingredient i) {
            var deleteCommand = "delete from costs where ing_id=@ing_id";
            executeVoidQuery(deleteCommand, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                return cmd;
            });
        }
        public void DeleteIngredientFromDensitiesTable(Ingredient i) {
            var deleteCommand = "delete from densities where ing_id=@ing_id";
            executeVoidQuery(deleteCommand, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                return cmd;
            });
        }
        public void DeleteIngredientFromConsumptionTable(Ingredient i) {
            var deleteCommand = "delete from consumption where name=@name";
            executeVoidQuery(deleteCommand, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                return cmd;
            });
        }
        //this will delete the ingredient from the consumption table, although
        //for more every day use, i want to just adjust the ounces remaining and the ounces consumed... 
        public void DeleteIngredientFromAllRelevantTables(Ingredient i) {
            var myIngredients = queryAllTablesForIngredient(i);
            var myIngredientsTable = queryIngredients();
            var myCostTable = queryCostTable();
            var myDensityTable = queryDensitiesTable();
            var myConsumptionTable = queryConsumptionTable();
            foreach (var ingredient in myIngredientsTable) {
                if (ingredient.ingredientId == i.ingredientId)
                    DeleteIngredientFromIngredientTable(ingredient);
                break;
            }
            foreach (var ingredient in myCostTable) {
                if (ingredient.ingredientId == i.ingredientId)
                    DeleteIngredientFromCostTable(ingredient);
                break;
            }
            foreach (var ingredient in myDensityTable) {
                if (ingredient.ingredientId == i.ingredientId)
                    DeleteIngredientFromDensitiesTable(ingredient);
                break;
            }
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower())
                    DeleteIngredientFromConsumptionTable(ingredient);
                break;
            }
            var myNewIngredients = queryAllTablesForIngredient(i);
            //i dont want to keep the consumption table stuff... that's pretty impt
            //i'm still trying to determine the pros of having the consumption? 
        }
        public void UpdateRecipeYield(Recipe r) {
            var convert = new ConvertMeasurement();
            var myRecipeBox = MyRecipeBox();
            foreach (var recipe in myRecipeBox) {
                var myIngredients = queryAllTablesForAllIngredients(recipe.ingredients);
                var tempIngredient = new Ingredient();
                if (recipe.id == r.id) {
                    foreach (var ingredient in recipe.ingredients) {
                        tempIngredient = ingredient;
                        if (tempIngredient.density == 0)
                            tempIngredient.density = returnIngredientDensityFromDensityTable(ingredient);
                        tempIngredient.measurement = convert.AdjustIngredientMeasurement(ingredient.measurement, recipe.yield, r.yield);
                        tempIngredient.ouncesConsumed = ingredient.ouncesConsumed * (recipe.yield / r.yield);
                        updateAllTables(tempIngredient, r);
                        var myUpdatedIngredient = queryAllTablesForIngredient(tempIngredient);
                    }
                    recipe.yield = r.yield;
                    GetFullRecipePrice(recipe);
                    var myUpdatedIngredients = queryAllTablesForAllIngredients(recipe.ingredients);
                }
            }
        }
        public void UpdateListOfRecipeYields(List<Recipe> myListOfRecipes) {
            //var myRecipeBox = MyRecipeBox();
            var myListOfRecipesIds = new List<int>();
            foreach (var recipe in myListOfRecipes) {
                if (!myListOfRecipesIds.Contains(recipe.id)) {
                    myListOfRecipesIds.Add(recipe.id);
                }
            }
            var myUpdatedRecipeBox = MyRecipeBox();
            foreach (var recipe in myListOfRecipes) {
                var currentRecipe = GetFullRecipe(recipe);
                UpdateRecipeYield(recipe);
            }
        }
        //ingredient table methods: 
        public List<Ingredient> queryIngredients() {
            var count = 1;
            var myIngredientBox = queryItems("select * from ingredients", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.measurement = (string)reader["measurement"];
                ingredient.recipeId = (int)reader["recipe_id"];
                ingredient.priceOfMeasuredConsumption = (decimal)reader["price_measured_ingredient"];
                ingredient.itemId = (int)reader["item_id"];
                ingredient.typeOfIngredient = (string)reader["ingredient_type"];
                ingredient.classification = (string)reader["ingredient_classification"];
                ingredient.expirationDate = convertStringToDateYYYYMMDD((string)reader["expiration_date"]);
                return ingredient;
            });
            foreach (var ingredient in myIngredientBox)
                ingredient.ingredientId = count++;
            return myIngredientBox;
        }
        //public List<Ingredient> queryAllTablesForIngredientNoParam() {
        //    var rest = new MakeRESTCalls();
        //    var myRecipes = queryRecipes();
        //    var myIngredients = queryIngredients();
        //    var myIngredientConsumption = queryConsumptionTable();
        //    var myIngredientDensity = queryDensityTable();
        //    var myIngredientCost = queryCostTable();
        //    var myDensityInfoTable = queryDensityInfoTable();
        //    var tempListOfIngredients = new List<Ingredient>(); 
        //    foreach (var ing in myIngredients) {
        //        tempListOfIngredients.Add(ing); 
        //    }

        //    var temp = new Recipe();
        //    foreach (var ing in myIngredients) {
        //        if (ing.ingredientId == i.ingredientId) {
        //            i.recipeId = ing.recipeId;
        //            i.measurement = ing.measurement;
        //            i.typeOfIngredient = ing.typeOfIngredient;
        //            if (i.itemId == 0)
        //                i.itemId = rest.GetItemResponse(i).itemId;
        //            else i.itemId = ing.itemId;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredientConsumption) {
        //        if (ing.ingredientId == i.ingredientId) {
        //            i.density = ing.density;
        //            i.ouncesConsumed = ing.ouncesConsumed;
        //            i.ouncesRemaining = ing.ouncesRemaining;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myDensityInfoTable) {
        //        if (ing.name == i.typeOfIngredient) {
        //            i.density = ing.density;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredientDensity) {
        //        if (ing.ingredientId == i.ingredientId) {
        //            i.sellingWeight = ing.sellingWeight;
        //            i.sellingWeightInOunces = ing.sellingWeightInOunces;
        //            i.itemId = ing.itemId;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredientCost) {
        //        if (ing.ingredientId == i.ingredientId) {
        //            if (ing.sellingPrice == 0m)
        //                i.sellingPrice = rest.GetItemResponse(i).salePrice;
        //            else i.sellingPrice = ing.sellingPrice;
        //            if (ing.pricePerOunce == 0m)
        //                i.pricePerOunce = (i.sellingPrice / i.sellingWeightInOunces);
        //            else i.pricePerOunce = ing.pricePerOunce;
        //            i.itemId = ing.itemId;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredients) {
        //        if (ing.ingredientId == i.ingredientId) {
        //            i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
        //            break;
        //        }
        //    }
        //    return i;
        //}
        public Ingredient queryAllTablesForIngredient(Ingredient i) {
            var rest = new MakeRESTCalls();
            var myRecipes = queryRecipes();
            var myIngredients = queryIngredients();
            var myIngredientConsumption = queryConsumptionTable();
            var myDensityInfoTable = queryDensityInfoTable();
            var myIngredientDensity = queryDensitiesTable();
            var myIngredientCost = queryCostTable();
            var temp = new Recipe();
            foreach (var rec in myRecipes) {
                if (rec.id == i.recipeId) {
                    temp = rec;
                    break;
                }
            }
            foreach (var ing in myIngredients) {
                if (ing.ingredientId == i.ingredientId) {
                    i.recipeId = ing.recipeId;
                    i.measurement = ing.measurement;
                    i.typeOfIngredient = ing.typeOfIngredient;
                    if (i.itemId == 0 && !i.classification.ToLower().Contains("egg") && !i.classification.ToLower().Contains("dairy") && !string.IsNullOrEmpty(i.classification))
                        i.itemId = myItemResponse.itemId;
                    else i.itemId = ing.itemId;
                    i.expirationDate = ing.expirationDate;
                    break;
                }
            }
            foreach (var ing in myIngredientConsumption) {
                if (ing.name == i.name) {
                    i.density = ing.density;
                    i.ouncesConsumed = ing.ouncesConsumed;
                    i.ouncesRemaining = ing.ouncesRemaining;
                    break;
                }
            }
            foreach (var ing in myDensityInfoTable) {
                if (ing.name == i.typeOfIngredient) {
                    i.density = ing.density;
                    break;
                }
            }
            foreach (var ing in myIngredientDensity) {
                if (ing.ingredientId == i.ingredientId) {
                    i.sellingWeight = ing.sellingWeight;
                    i.sellingWeightInOunces = ing.sellingWeightInOunces;
                    i.itemId = ing.itemId;
                    break;
                }
            }
            foreach (var ing in myIngredientCost) {
                if (ing.ingredientId == i.ingredientId) {
                    if (ing.sellingPrice == 0m && !i.classification.ToLower().Contains("egg") && !i.classification.ToLower().Contains("dairy"))
                        i.sellingPrice = myItemResponse.salePrice;
                    else i.sellingPrice = ing.sellingPrice;
                    if (ing.pricePerOunce == 0m)
                        i.pricePerOunce = (i.sellingPrice / i.sellingWeightInOunces);
                    else i.pricePerOunce = ing.pricePerOunce;
                    i.itemId = ing.itemId;
                    break;
                }
            }
            if (i.ouncesConsumed != 0m && i.ouncesRemaining != 0m && i.priceOfMeasuredConsumption == 0m) {
                foreach (var ing in myIngredients) {
                    if (ing.ingredientId == i.ingredientId) {
                        i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
                        break;
                    }
                }
            }
            return i;
        }
        public List<Ingredient> queryAllTablesForAllIngredients(List<Ingredient> ListOfIngredients) {
            var queriedListOfIngredients = new List<Ingredient>();
            foreach (var ingredient in ListOfIngredients)
                queriedListOfIngredients.Add(queryAllTablesForIngredient(ingredient));
            return queriedListOfIngredients;
        }
        public void insertIngredient(Ingredient i, Recipe r) {
            if (i.sellingPrice != 0m) { //this is assuming that if they're providing their own selling price, they're not going to be looking for walmart's price, item id or name for the product
                myItemResponse = returnItemResponse(i);
                if (i.itemId == 0)
                    i.itemId = myItemResponse.itemId;
                if (string.IsNullOrEmpty(i.itemResponseName))
                    i.itemResponseName = myItemResponse.name;
                if (string.IsNullOrEmpty(i.itemResponseName)) // || (i.classification.ToLower().Contains("dairy") || i.classification.ToLower().Contains("egg"))
                    i.itemResponseName = " ";
            }
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            var commandText = "Insert into ingredients(recipe_id, name, measurement, price_measured_ingredient, item_id, ingredient_type, ingredient_classification, item_response_name, expiration_date) values (@rid, @name, @measurement, @price_measured_ingredient, @item_id, @ingredient_type, @ingredient_classification, @item_response_name, @expiration_date);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@price_measured_ingredient", i.priceOfMeasuredConsumption);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                cmd.Parameters.AddWithValue("@ingredient_type", i.typeOfIngredient);
                cmd.Parameters.AddWithValue("@ingredient_classification", i.classification);
                cmd.Parameters.AddWithValue("@item_response_name", i.itemResponseName);
                //cmd.Parameters.AddWithValue("@expiration_date", i.expirationDate); 
                cmd.Parameters.AddWithValue("@expiration_date", convertDateToStringMMDDYYYY(i.expirationDate));
                //this convertDateToString formats the string in MM/DD/YYYY
                return cmd;
            });
            var myIngredients = queryIngredients();
        }
        public void UpdateIngredient(Ingredient i) {
            var myIngredients = queryIngredients();
            myItemResponse = returnItemResponse(i);
            if (i.itemId == 0)
                i.itemId = myItemResponse.itemId;
            if (string.IsNullOrEmpty(i.itemResponseName))
                i.itemResponseName = myItemResponse.name;
            if (i.priceOfMeasuredConsumption == 0)
                i.priceOfMeasuredConsumption = returnIngredientMeasuredPrice(i);
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            var myIngredientId = i.ingredientId;
            var commandText = "update ingredients set name=@name, measurement=@measurement, recipe_id=@recipeId, price_measured_ingredient=@price_measured_ingredient, item_id=@item_id, ingredient_type=@ingredient_type, ingredient_classification=@ingredient_classification, item_response_name=@item_response_name, expiration_date=@expiration_date where ing_id=@ing_id;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@recipeId", i.recipeId);
                cmd.Parameters.AddWithValue("@ingredientId", i.ingredientId);
                cmd.Parameters.AddWithValue("@price_measured_ingredient", i.priceOfMeasuredConsumption);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                cmd.Parameters.AddWithValue("@ingredient_type", i.typeOfIngredient);
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@ingredient_classification", i.classification);
                cmd.Parameters.AddWithValue("@item_response_name", i.itemResponseName);
                cmd.Parameters.AddWithValue("@expiration_date", convertDateToStringMMDDYYYY(i.expirationDate));
                //this convertDateToString formats the string in MM/DD/YYYY
                return cmd;
            });
        }
        public decimal MeasuredIngredientPrice(Ingredient i) {
            var convertWeight = new ConvertWeight();
            var convert = new ConvertMeasurement();
            var myCostData = queryCostTable();
            var myIngredients = queryIngredients();
            var myDensityData = queryDensitiesTable();
            var myConsumptionData = queryConsumptionTable();
            var temp = new Ingredient();
            var measuredIngredientPrice = 0m;
            foreach (var ingredient in myConsumptionData) {
                if (ingredient.name.ToLower() == i.name.ToLower() || (ingredient.name.ToLower().Contains(i.classification.ToLower()) && i.classification != " ")) {
                    temp.ouncesConsumed = ingredient.ouncesConsumed;
                    break;
                }
            }
            foreach (var ingredient in myDensityData) {
                if (ingredient.name == i.name) {
                    temp.density = ingredient.density;
                    temp.sellingWeightInOunces = ingredient.sellingWeightInOunces;
                    break;
                }
            }
            foreach (var ingredient in myCostData) {
                if (ingredient.ingredientId == i.ingredientId) {
                    temp.sellingPrice = ingredient.sellingPrice;
                    temp.pricePerOunce = ingredient.pricePerOunce;
                    break;
                }
            }
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name) {
                    ingredient.ouncesConsumed = temp.ouncesConsumed;
                    ingredient.sellingPrice = temp.sellingPrice;
                    //var accumulatedTeaspoons = convert.AccumulatedTeaspoonMeasurement(ingredient.measurement);
                    var measuredOuncesDividedBySellingWeight = 0m;
                    if (temp.sellingWeightInOunces != 0)
                        measuredOuncesDividedBySellingWeight = Math.Round((ingredient.ouncesConsumed / temp.sellingWeightInOunces), 4);
                    measuredIngredientPrice = Math.Round((measuredOuncesDividedBySellingWeight * temp.sellingPrice), 2);
                    break;
                }
            }
            return measuredIngredientPrice;
        }
        public void insertIngredientIntoAllTables(Ingredient i, Recipe r) {
            var myRecipes = queryRecipes();
            var myIngredientBox = queryIngredients();
            var myIngredients = queryAllTablesForIngredient(i);
            var count = 0;
            var countIngredients = 0;
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id)
                    count++;
            }
            if (count == 0)
                InsertRecipe(r);
            foreach (var ingredient in myIngredientBox) {
                if (ingredient.ingredientId == i.ingredientId) {
                    countIngredients++;
                    break;
                }
            }
            if (countIngredients == 0) {
                insertIngredient(i, r);
                var myIng = queryAllTablesForIngredient(i);
                insertIngredientIntoDensityInfoDatabase(i);
                var myIng2 = queryAllTablesForIngredient(i);
                insertIngredientDensityData(i);
                var myIng3 = queryAllTablesForIngredient(i);
                insertIngredientConsumtionData(i);
                var myIng4 = queryAllTablesForIngredient(i);
                insertIngredientCostDataCostTable(i);
                var myIng5 = queryAllTablesForIngredient(i);
                updateAllTables(i, r);
                var queriedIngredients = queryAllTablesForIngredient(i);
            } else {
                UpdateIngredient(i);
                var updatedIngredient = queryAllTablesForIngredient(i);
                updateDensityInfoTable(i);
                var updatedIngredient2 = queryAllTablesForIngredient(i);
                updateDensityTable(i);
                var updatedIngredient3 = queryAllTablesForIngredient(i);
                updateCostDataTable(i);
                var updatedIngredient4 = queryAllTablesForIngredient(i);
                UpdateIngredient(i);
                var updatedIngredient5 = queryAllTablesForIngredient(i);
            }
        }
        public List<Ingredient> getListOfDistintIngredients() {
            var myIngredientsTable = queryIngredients();
            var myUniqueIngredientNames = new List<string>();
            var myUniqueIngredients = new List<Ingredient>();
            foreach (var ingredient in myIngredientsTable) {
                if (!myUniqueIngredientNames.Contains(ingredient.name)) {
                    myUniqueIngredientNames.Add(ingredient.name);
                    myUniqueIngredients.Add(queryAllTablesForIngredient(ingredient));
                }
            }
            return myUniqueIngredients;
        }
        //return myIngredientsTable.Select(x => x).Distinct();
        //need to put a cast on this or something? this should be easily refactorable with a .Distinct(), it's just giving me a type problem for the moment and save a few lines of code
        //public Func<List<Ingredient>, List<Ingredient>> GetDistinctListOfIngredientsFromQueryIngredients = queriedIngredientsFromIngredientsTable => queriedIngredientsFromIngredientsTable.Select(x => x).Distinct();
        public void insertListOfIngredientsIntoAllTables(List<Ingredient> ListOfIngredients, Recipe r) {
            var myListOfIngredientIds = new List<int>();
            var myListOfRecipeIds = new List<int>();
            foreach (var ingredient in ListOfIngredients) {
                if (!myListOfIngredientIds.Contains(ingredient.ingredientId)) {
                    insertIngredientIntoAllTables(ingredient, r);
                    myListOfIngredientIds.Add(ingredient.ingredientId);
                }
            }
            var myRecipes = queryRecipes();
            var myIngredients = queryIngredients();
            foreach (var recipe in myRecipes)
                myListOfRecipeIds.Add(recipe.id);
            if (!myListOfRecipeIds.Contains(r.id))
                InsertRecipe(r);
            foreach (var ingredient in myIngredients) {
                if (!myListOfIngredientIds.Contains(ingredient.ingredientId))
                    myListOfIngredientIds.Add(ingredient.ingredientId);
            }
            var myRecipe = GetFullRecipe(r);
            foreach (var ingredient in myRecipe.ingredients) {
                if (!myListOfIngredientIds.Contains(ingredient.ingredientId))
                    insertIngredientIntoAllTables(ingredient, r);
            }
            var myIngredientsSecond = queryAllTablesForAllIngredients(ListOfIngredients);
        }
        public decimal returnIngredientMeasuredPrice(Ingredient i) {
            queryAllTablesForIngredient(i);
            return MeasuredIngredientPrice(i);
        }
        public void getIngredientMeasuredPrice(Ingredient i, Recipe r) {
            queryAllTablesForIngredient(i);
            i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
            UpdateIngredient(i);
        }
        public void updateAllTables(Ingredient i, Recipe r) {
            var myCostTable = queryCostTable();
            foreach (var ingredient in myCostTable) {
                if (ingredient.ingredientId == i.ingredientId) {
                    if (ingredient.sellingPrice == 0m && i.sellingPrice != 0m) {
                        updateCostDataTable(i);
                        break;
                    }
                }
            }
            UpdateRecipe(r);
            UpdateIngredient(i);
            var updatedIngredient = queryAllTablesForIngredient(i);
            updateDensityInfoTable(i);
            var updatedIngredient2 = queryAllTablesForIngredient(i);
            updateDensityTable(i);
            var updatedIngredient3 = queryAllTablesForIngredient(i);
            updateCostDataTable(i);
            var updatedIngredient4 = queryAllTablesForIngredient(i);
        }
        public void updateAllTablesForAllIngredients(List<Ingredient> myListOfIngredients, Recipe r) {
            foreach (var ingredient in myListOfIngredients) {
                updateAllTables(ingredient, r);
            }
            var myUpdatedIngredients = queryAllTablesForAllIngredients(myListOfIngredients);
        }
        public decimal doubleAverageOuncesConsumed(Ingredient i) {
            var convert = new ConvertMeasurement();
            var listOfIngredientOuncesConsumed = new List<decimal>();
            var myIngredientOuncesConsumedTable = queryConsumptionOuncesConsumed();
            foreach (var ingredient in myIngredientOuncesConsumedTable) {
                if (ingredient.name == i.name)
                    listOfIngredientOuncesConsumed.Add(ingredient.ouncesConsumed);
            }
            var count = listOfIngredientOuncesConsumed.Count();
            var aggregatedOuncesConsumed = 0m;
            foreach (var measurement in listOfIngredientOuncesConsumed)
                aggregatedOuncesConsumed += measurement;
            return Math.Round((aggregatedOuncesConsumed / count) * 2, 2);
        }
        //densities table methods: 
        public List<Ingredient> queryDensitiesTable() {
            var ingredientInformation = queryItems("select * from densities", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["ing_id"];
                ingredient.density = (decimal)reader["density"];
                ingredient.sellingWeight = (string)reader["selling_weight"];
                ingredient.sellingWeightInOunces = (decimal)reader["selling_weight_ounces"];
                ingredient.sellingPrice = (decimal)reader["selling_price"];
                ingredient.pricePerOunce = (decimal)reader["price_per_ounce"];
                return ingredient;
            });
            return ingredientInformation;
        }
        public void insertIngredientDensityData(Ingredient i) {
            var convert = new ConvertWeight();
            myItemResponse = returnItemResponse(i);
            i.density = returnIngredientDensityFromDensityTable(i);
            if (i.sellingPrice == 0m)
                i.sellingPrice = myItemResponse.salePrice;
            if (i.classification.ToLower().Contains("egg")) {
                i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
            } else i.sellingWeightInOunces = convert.ConvertWeightToOunces(i.sellingWeight);
            i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            var commandText = @"Insert into densities (name, density, selling_weight, selling_weight_ounces, selling_price, price_per_ounce) 
                            values (@name, @density, @selling_weight, @selling_weight_ounces, @selling_price, @price_per_ounce);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                return cmd;
            });
        }
        public void updateDensityTable(Ingredient i) {
            var commandText = "update densities set name=@name, density=@density, selling_weight=@selling_weight, selling_weight_ounces=@selling_weight_ounces, selling_price=@selling_price, price_per_ounce=@price_per_ounce where ing_id=@ing_id";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                return cmd;
            });
        }

        //i'm going to have to say that if an ingredient is deleted from any of these, that I have to add the ounces consumed back
        //and have a box select (if the recipe was made, then actually remove the ouncesConsumed from the ouncesRemaining)
        //if it hasn't been made, show a potential of what the ouncesRemaining would be, vs what it actually is
        //but before i get that, i want to make sure my view is doing what it needs to do
        //make a check box that indicates if the recipe has been made... if it has been made, then enact this updateConsumptionTable method

        //consumption table methods: 
        public List<Ingredient> queryConsumptionTable() {
            var ingredientInformation = queryItems("select * from consumption", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["id"];
                ingredient.density = (decimal)reader["density"];
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                ingredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                return ingredient;
            });
            return ingredientInformation;
        }
        public void insertIngredientConsumtionData(Ingredient i) {
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredient = queryAllTablesForIngredient(i);
            var myConsumptionTable = queryConsumptionTable();
            var temp = new Ingredient();
            bool alreadyContainsIngredient = new bool();
            if (myIngredient.classification.ToLower().Contains("egg")) {
                temp.name = "eggs";
                i.ouncesConsumed = convertWeight.EggsConsumedFromIngredientMeasurement(myIngredient.measurement);
            } else i.ouncesConsumed = CalculateOuncesConsumedFromMeasurement(i);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower() || (ingredient.name.ToLower().Contains(i.classification.ToLower()) && i.classification != " ")) {
                    alreadyContainsIngredient = true;
                    break;
                }
            }
            if (string.IsNullOrEmpty(temp.name))
                temp.name = i.name;
            if (alreadyContainsIngredient == false) {
                var commandText = @"Insert into consumption (name, density, ounces_consumed, ounces_remaining) values (@name, @density, @ounces_consumed, @ounces_remaining);";
                executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@name", temp.name);
                    cmd.Parameters.AddWithValue("@density", i.density);
                    cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                    //when the time comes, i want to change any negative ouncesRemaining to be 0 so i can start fresh when i refill the ingredient in my consumption table
                    //although, it would be nice to say "you need 2 tablespoons more granulated sugar to make this recipe"... maybe if you refill, then put at 0 first, if not, then leave negative?
                    cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                    return cmd;
                });
                updateConsumptionTable(i);
            } else updateConsumptionTable(i);
            var myUpdatedIngredient = queryConsumptionTable();
        }
        public void updateConsumptionTable(Ingredient i) {
            var convert = new ConvertWeight();
            var myIngredient = queryAllTablesForIngredient(i);
            var myConsumptionTable = queryConsumptionTable();
            var temp = new Ingredient();
            foreach (var ingredient in myConsumptionTable) {
                //problems: my ouncesConsumed should be 2 instead of 3, and my priceofmeasuredconsumption is 0 for the moment... 
                if (i.classification.ToLower().Contains("egg") && ingredient.name.ToLower().Contains("egg")) {
                    temp.name = ingredient.name;
                    var currentOuncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                    if (ingredient.ouncesConsumed != currentOuncesConsumed)
                        i.ouncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                    if (ingredient.ouncesRemaining == 0m)
                        i.ouncesRemaining = i.sellingWeightInOunces - i.ouncesConsumed;
                    else i.ouncesRemaining = ingredient.ouncesRemaining - i.ouncesConsumed;
                    break;
                } else {
                    //this above is a catch all for eggs... i don't want cake flour and bread flour to come from the same source for ounces remaining, but i want all eggs to be coming from the same place, the egg carton :)
                    if (ingredient.name.ToLower() == i.name.ToLower()) {
                        i.ouncesConsumed = CalculateOuncesConsumedFromMeasurement(i);
                        if (ingredient.ouncesRemaining == 0m)
                            i.ouncesRemaining = i.sellingWeightInOunces - i.ouncesConsumed;
                        else
                            i.ouncesRemaining = ingredient.ouncesRemaining - i.ouncesConsumed;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(temp.name))
                temp.name = i.name;
            var commandText = "update consumption set ounces_consumed=@ounces_consumed, ounces_remaining=@ounces_remaining where name=@name;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", temp.name);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                return cmd;
            });
            insertIngredientIntoConsumptionOuncesConsumed(i);
            subtractOuncesRemainingIfExpirationDateIsPast(i);
            var myUpdatedIngredient = queryConsumptionTable();
        }
        public void updateExpirationDate(Ingredient i) { }
        public void subtractOuncesRemainingIfExpirationDateIsPast(Ingredient i) {
            var convert = new ConvertWeight();
            var myIngredient = queryAllTablesForIngredient(i);
            if (i.expirationDate < DateTime.Today) {
                myIngredient.ouncesRemaining = myIngredient.ouncesRemaining - i.sellingWeightInOunces;
                if (myIngredient.ouncesRemaining < 0m)
                    myIngredient.ouncesRemaining = 0m;
                var commandText = @"update consumption set ounces_remaining=@ounces_remaining where name=@name";
                executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@name", myIngredient.name);
                    cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                    return cmd;
                });
            }
            var myUpdatedIngredient = queryConsumptionTable();
        }
        public void refillIngredientInConsumptionDatabase(Ingredient i, string sellingWeightToRefill) {
            var convert = new ConvertWeight();
            var myConsumptionTable = queryConsumptionTable();
            var sellingWeightToRefillInOunces = convert.ConvertWeightToOunces(sellingWeightToRefill);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower()) {
                    if (i.ouncesRemaining < 0m)
                        i.ouncesRemaining = 0m;
                    i.ouncesRemaining = ingredient.ouncesRemaining + sellingWeightToRefillInOunces;
                    break;
                }
            }
            var commandText = "update consumption set ounces_remaining=@ounces_remaining where name=@name;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                return cmd;
            });
        }
        public void refillIngredientInConsumptionDatabase(Ingredient i, string sellingWeightToRefill, string newExpirationDate) {
            var convert = new ConvertWeight();
            var myConsumptionTable = queryConsumptionTable();
            var myIngredientTable = queryIngredients();
            var sellingWeightToRefillOunces = convert.ConvertWeightToOunces(sellingWeightToRefill);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower()) {
                    if (i.ouncesRemaining < 0m)
                        i.ouncesRemaining = 0m;
                    i.ouncesRemaining = ingredient.ouncesRemaining + sellingWeightToRefillOunces;
                    var commandText = "update consumption set ounces_remaining=@ounces_remaining where name=@name;";
                    executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@name", i.name);
                        cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                        return cmd;
                    });
                    break;
                }
            }
            foreach (var ingredient in myIngredientTable) {
                if (ingredient.ingredientId == i.ingredientId && ingredient.name.ToLower() == i.name.ToLower()) {
                    ingredient.expirationDate = convertStringToDateYYYYMMDD(newExpirationDate);
                    var commandText = "update ingredients set expiration_date=@expiration_date where ing_id=@ing_id";
                    executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@expiration_date", convertDateToStringMMDDYYYY(ingredient.expirationDate));
                        cmd.Parameters.AddWithValue("@ing_id", ingredient.ingredientId);
                        return cmd;
                    });
                    break;
                }
            }
        }
        //date data type helper function: 
        public DateTime convertIntToDate(int dateInInt) {
            var dateString = dateInInt.ToString();
            if (dateString.Length != 8) {
                return new DateTime();
            }
            var year = dateString.Substring(0, 4);
            var month = dateString.Substring(4, 2);
            var day = dateString.Substring(6, 2);
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }
        public DateTime convertStringToDateYYYYMMDD(string dateString) {
            if (dateString.Length < 8)
                return new DateTime();
            var dateStringArray = new string[] { };
            if (dateString.Contains('.'))
                dateStringArray = dateString.Split('.');
            if (dateString.Contains('/'))
                dateStringArray = dateString.Split('/');
            if (dateString.Contains('-'))
                dateStringArray = dateString.Split('-');
            var year = int.Parse(dateStringArray[2]);
            var month = int.Parse(dateStringArray[0]);
            var day = int.Parse(dateStringArray[1]);
            return new DateTime(year, month, day);
        }
        public string convertDateToStringMMDDYYYY(DateTime date) {
            var dateTimeArray = date.ToString().Split(' ');
            var dateString = dateTimeArray[0];
            var dateStringArray = dateString.Split('/');
            var month = dateStringArray[0];
            if (int.Parse(month) < 10)
                month = "0" + month;
            var day = dateStringArray[1];
            var year = dateStringArray[2];
            return string.Format("{0}/{1}/{2}", month, day, year);
        }
        public DateTime getExpirationDateFromIngredientsTable(Ingredient i) {
            var myIngredients = queryIngredients();
            var myIngredientExpirationDate = new DateTime();
            foreach (var ingredient in myIngredients) {
                if (ingredient.ingredientId == i.ingredientId) {
                    (ingredient.expirationDate) = myIngredientExpirationDate;
                    break;
                }
            }
            return myIngredientExpirationDate;
        }
        public bool doesIngredientNeedRestocking(Ingredient i) {
            var currentIngredientOuncesRemaining = queryAllTablesForIngredient(i).ouncesRemaining;
            var doubleOunces = doubleAverageOuncesConsumed(i);
            return currentIngredientOuncesRemaining <= doubleOunces ? true : false;
            //this can be continue to be true if someone refills their ingredient and the ingredient ouncesRemaining is still less than the doubleOunces
            //for example: if i use 10.7 ounces of chocolate chips (2 cups) of a 12 oz bag, and I add another 12 lb bag to my pantry, the doubleAverage is still going to be 21.4 oz... more than 1.3 + 12 oz
            //as soon as this is done, i really have to play around with the mvc stuff... i really want to get more acquainted with that!!
        }
        public List<Ingredient> queryConsumptionOuncesConsumed() {
            var ingredientConsumptionInformation = queryItems("select * from consumption_ounces_consumed", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                //ingredient.ingredientId = (int)reader["ing_id"]; 
                return ingredient;
            });
            return ingredientConsumptionInformation;
        }
        public void insertIngredientIntoConsumptionOuncesConsumed(Ingredient i) {
            var commandText = @"Insert into consumption_ounces_consumed (ing_id, name, ounces_consumed) values (@ing_id, @name, @ounces_consumed);";
            executeVoidQuery(commandText, cmd => {
                //cmd.Parameters.AddWithValue("@ing_id", i.ingredientId); 
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                return cmd;
            });
        }
        public void insertListOfIngredientsIntoConsumptionOuncesConsumed(List<Ingredient> myIngredients) {
            foreach (var ingredient in myIngredients)
                insertIngredientIntoConsumptionOuncesConsumed(ingredient);
        }
        public void updateIngredientInConsumptionouncesConsumed(Ingredient i) {
            var commandText = @"Update consumption_ounces_consumed set ounces_consumed=@ounces_consumed, name=@name where ing_id=@ing_id;";
            executeVoidQuery(commandText, cmd => {
                //cmd.Parameters.AddWithValue("@ing_id", i.ingredientId); 
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@name", i.name);
                return cmd;
            });
        }
        public decimal CalculateOuncesConsumedFromMeasurement(Ingredient i) {
            var convertMeasurement = new ConvertMeasurement();
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredientConsumptionData = queryConsumptionTable();
            var myIngredients = queryIngredients();
            var myConsumedOunces = 0m;
            var temp = new Ingredient();
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name) {
                    var accumulatedOunces = convertMeasurement.AccumulatedTeaspoonMeasurement(i.measurement);
                    if (i.classification.ToLower().Contains("egg")) {
                        var splitEggMeasurement = convertWeight.SplitWeightMeasurement(i.sellingWeight);
                        i.sellingWeightInOunces = decimal.Parse(splitEggMeasurement[0]);
                    }
                    myConsumedOunces = convert.CalculateOuncesUsed(i);
                }
            }
            return myConsumedOunces;
        }
        //cost table 
        public List<Ingredient> queryCostTable() {
            var ingredientInformation = queryItems("select * from costs", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["ing_id"];
                ingredient.sellingWeight = (string)reader["selling_weight"];
                ingredient.sellingPrice = (decimal)reader["selling_price"];
                ingredient.pricePerOunce = (decimal)reader["price_per_ounce"];
                ingredient.itemId = (int)reader["item_id"];
                return ingredient;
            });
            return ingredientInformation;
        }
        public void insertIngredientCostDataCostTable(Ingredient i) {
            var convert = new ConvertWeight();
            var myCostTable = queryCostTable();
            var temp = new Ingredient();
            temp.sellingPrice = i.sellingPrice;
            if (i.classification.ToLower().Contains("egg")) {
                i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
                i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
            }
            var commandText = @"Insert into costs (name, selling_weight, selling_price, price_per_ounce, item_id) values (@name, @selling_weight, @selling_price, @price_per_ounce, @item_id);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", temp.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }
        public void updateCostDataTable(Ingredient i) {
            var myCostTable = queryCostTable();
            var commandText = @"Update costs set name=@name, selling_weight=@selling_weight, selling_price=@selling_price, price_per_ounce=@price_per_ounce where ing_id=@ing_id;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", getPricePerOunce(i));
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
            var myUpdatedCostTable = queryCostTable();
        }
        public decimal getPricePerOunce(Ingredient i) {
            var convert = new ConvertWeight();
            var myCostTableIngredients = queryCostTable();
            var pricePerOunce = 0m;
            foreach (var ingredient in myCostTableIngredients) {
                if (ingredient.name == i.name) {
                    i.sellingPrice = ingredient.sellingPrice;
                    if (i.classification.ToLower().Contains("egg"))
                        i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
                    else i.sellingWeightInOunces = convert.ConvertWeightToOunces(ingredient.sellingWeight);
                    i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
                    pricePerOunce = i.pricePerOunce;
                }
            }
            return pricePerOunce;
        }
        public List<Ingredient> queryDensityInfoTable() {
            var DensityInfo = queryItems("select * from densityInfo", reader => {
                var densityIngredientInformation = new Ingredient(reader["ingredient"].ToString());
                densityIngredientInformation.density = (decimal)reader["density"];
                return densityIngredientInformation;
            });
            return DensityInfo;
        }
        public void insertIngredientIntoDensityInfoDatabase(Ingredient i) {
            var rest = new MakeRESTCalls();
            var myDensityInfoTable = queryDensityInfoTable();
            if (myDensityInfoTable.Count() == 0)
                insertDensityTextFileIntoDensityInfoDatabase(@"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
            var myUpdatedDensityInfoTable = queryDensityInfoTable();
            var myMilkAndEggDensityInfoIngredients = new List<Ingredient>();
            foreach (var ingredient in myUpdatedDensityInfoTable) {
                if (ingredient.name.ToLower().Contains("milk") || ingredient.name.ToLower().Contains("egg"))
                    myMilkAndEggDensityInfoIngredients.Add(ingredient);
            }
            var countSimilarIngredients = 0;
            foreach (var ingredient in myUpdatedDensityInfoTable) {
                if (i.typeOfIngredient.ToLower().Contains("milk") || i.typeOfIngredient.ToLower().Contains("egg")) {
                    foreach (var dairyOrEggIngredient in myMilkAndEggDensityInfoIngredients) {
                        if (i.typeOfIngredient == dairyOrEggIngredient.name) {
                            countSimilarIngredients++;
                            break;
                        }
                    }
                    break;
                } else {
                    if (rest.SimilaritesInStrings(i.typeOfIngredient, ingredient.name)) {
                        countSimilarIngredients++;
                        break;
                    }
                }
            }
            if (countSimilarIngredients == 0) {
                var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
                executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@ingredient", i.typeOfIngredient);
                    cmd.Parameters.AddWithValue("@density", i.density);
                    return cmd;
                });
            }
            //all this is doing is determining if the density table already has an ingredient with said name, if so, then it won't add it, if the table doesn't have that name, it will insert it with the density
            var myDensityInfoDatabase = queryDensityInfoTable();
        }
        public List<Ingredient> assignIngredientDensityDictionaryValuesToListIngredients(Dictionary<string, decimal> myDensityIngredientDictionary) {
            var myIngredients = new List<Ingredient>();
            foreach (var pair in myDensityIngredientDictionary) {
                var currentIngredient = new Ingredient(pair.Key) {
                    density = pair.Value
                };
                myIngredients.Add(currentIngredient);
            }
            return myIngredients;
        }
        public void insertDensityTextFileIntoDensityInfoDatabase(string filename) {
            //filename = @"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt";
            var read = new Reader(); //the filename below for the moment is hardcoded... 
            var DensityTextDatabaseDictionary = read.ReadDensityTextFile(@"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
            var myDensityTable = queryDensityInfoTable();
            var myDensityTableNames = new List<string>();
            foreach (var ingredient in myDensityTable)
                myDensityTableNames.Add(ingredient.name);
            //this is going to need to allow for user error and grace in the name... need to have a similaries check, or make sure the name.tolower contains the ingredient's name, as opposed to == it
            //i may have fixed this with the type of ingredient.... but i'll have to do more tests around that to see if it's intuitive
            foreach (var ingredient in DensityTextDatabaseDictionary) {
                if (!myDensityTableNames.Contains(ingredient.Key)) {
                    var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
                    executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@ingredient", ingredient.Key);
                        cmd.Parameters.AddWithValue("@density", ingredient.Value);
                        return cmd;
                    });
                }
            }
            var myDensityTableAfter = queryDensityInfoTable();
        }
        public void insertListIntoDensityInfoDatabase(List<Ingredient> MyIngredients) {
            var read = new Reader(); //the filename below for the moment is hardcoded... but i would prefer to not keep it that way... bad business
            var myDensityTable = queryDensityInfoTable();
            var myDensityInfoTableIngredients = new List<string>();
            foreach (var ingredient in myDensityTable)
                myDensityInfoTableIngredients.Add(ingredient.typeOfIngredient);
            for (int i = 0; i < MyIngredients.Count(); i++) {
                if (!myDensityInfoTableIngredients.Contains(MyIngredients[i].typeOfIngredient)) {
                    var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
                    executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@ingredient", MyIngredients[i].name);
                        cmd.Parameters.AddWithValue("@density", MyIngredients[i].density);
                        return cmd;
                    });
                }
            }
            var myDensityInfoTable = queryDensityInfoTable();
        }
        public void updateDensityInfoTable(Ingredient myIngredient) {
            var myDensityTableInfo = queryDensityInfoTable();
            var myDensityTableInfoNames = new List<string>();
            foreach (var ingredient in myDensityTableInfo)
                myDensityTableInfoNames.Add(ingredient.name);
            if (!myDensityTableInfoNames.Contains(myIngredient.typeOfIngredient))
                insertIngredientIntoDensityInfoDatabase(myIngredient);
            else {
                var commandText = @"Update densityInfo set density=@density where ingredient=@ingredient;";
                executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@ingredient", myIngredient.typeOfIngredient);
                    cmd.Parameters.AddWithValue("@density", myIngredient.density);
                    return cmd;
                });
            }
        }
        public decimal returnIngredientDensityFromDensityTable(Ingredient i) {
            var rest = new MakeRESTCalls();
            var myIngredients = queryIngredients();
            var myDensityIngredients = queryDensityInfoTable();
            var myIngredientDensity = 0m;
            foreach (var ingredient in myDensityIngredients) {
                if (rest.SimilaritesInStrings(i.typeOfIngredient, ingredient.name)) {
                    myIngredientDensity = ingredient.density;
                    break;
                }
            }
            return myIngredientDensity;
        }
        public void updateListOfIngredientsInDensityInfoTable(List<Ingredient> MyIngredients) {
            var myDensityTableInfo = queryDensityInfoTable();
            var myDensityTableInfoNames = new List<string>();
            foreach (var ingredient in myDensityTableInfo)
                myDensityTableInfoNames.Add(ingredient.name);
            for (int i = 0; i < MyIngredients.Count(); i++) {
                if (!myDensityTableInfoNames.Contains(MyIngredients[i].name))
                    insertIngredientIntoDensityInfoDatabase(MyIngredients[i]);
                else
                    updateDensityInfoTable(MyIngredients[i]);
            }
        }

        //initalize database tables
        public void dropTableIfExists(string table) {
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            executeVoidQuery(drop, a => a);
        }
        public void createDensityDatabase() {
            executeVoidQuery(@"create table densityInfo (
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
        }
        public void initializeDatabase() {
            dropTableIfExists("recipes");
            executeVoidQuery(@"create table recipes (
                        recipe_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        yield int,
                        aggregated_price decimal(5, 2)
                     );", a => a);

            dropTableIfExists("ingredients");
            executeVoidQuery(@"create table ingredients (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        recipe_id Int,
                        name nvarchar(max), 
                        item_id int, 
                        measurement nvarchar(max),
                        ingredient_classification nvarchar(max),
                        ingredient_type nvarchar(max),
                        price_measured_ingredient decimal(6,2),
                        item_response_name varchar(max),
                        expiration_date nvarchar(25)
                     );", a => a);
            dropTableIfExists("densities");
            executeVoidQuery(@"create table densities (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        density decimal (4,2),
                        selling_weight varchar(250),
                        selling_weight_ounces decimal(6,2),
                        selling_price decimal(6,2),
                        price_per_ounce decimal(8,4)
                     );", a => a);
            dropTableIfExists("consumption");
            executeVoidQuery(@"create table consumption (
                        id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        name varchar(max),
                        density decimal (4,2),
                        ounces_consumed decimal (5,2),
                        ounces_remaining decimal(6,2),
                     );", a => a);
            dropTableIfExists("costs");
            executeVoidQuery(@"create table costs (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        name varchar (max),
                        selling_weight varchar(max),
                        selling_price decimal(6,2),
                        price_per_ounce decimal (6,4),
                        item_id int
                    );", a => a);
            dropTableIfExists("densityInfo");
            executeVoidQuery(@"create table densityInfo (
                        ing_id int,
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
            dropTableIfExists("consumption_ounces_consumed");
            executeVoidQuery(@"create table consumption_ounces_consumed (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        name nvarchar(max), 
                        ounces_consumed decimal(5,2)
                        );", a => a);
            //this ingredient name is to represent the ingredient.typeOfIngredient
            executeVoidQuery("SET IDENTITY_INSERT densities ON", cmd => cmd);
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it