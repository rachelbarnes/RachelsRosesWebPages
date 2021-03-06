﻿using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessRecipe {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public void dropIfRecipeTableExists(string table) {
            var db = new DatabaseAccess();
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void initializeRecipeTable() {
            var db = new DatabaseAccess();
            dropIfRecipeTableExists("recipes");
            db.executeVoidQuery(@"create table recipes (
                           recipe_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                           recipe_name nvarchar(max), 
                           yield int,
                           aggregated_price decimal(5, 2), 
                           price_per_serving decimal (5,2)
                         );", a => a);
        }
        public ItemResponse returnItemResponse(Ingredient i) {
            var rest = new MakeRESTCalls();
            return rest.GetItemResponse(i);
        }
        public static ItemResponse myItemResponse = new ItemResponse();

        public List<Recipe> queryRecipes() {
            var db = new DatabaseAccess();
            var count = 1;
            var MyRecipeBox = db.queryItems("select * from recipes", reader => {
                var recipe = new Recipe(reader["recipe_name"].ToString());
                recipe.yield = (int)reader["yield"];
                recipe.id = (int)reader["recipe_id"];
                recipe.aggregatedPrice = (decimal)reader["aggregated_price"];
                recipe.pricePerServing = (decimal)reader["price_per_serving"];
                return recipe;
            });
            foreach (var recipe in MyRecipeBox) {
                recipe.id = count++;
            }
            return MyRecipeBox;
        }
        public void UpdateRecipe(Recipe r) {
            var db = new DatabaseAccess();
            var myRecipe = queryRecipeFromRecipesTableByName(r);
            if (myRecipe.yield != r.yield) {
                myRecipe.yield = r.yield;
            }
            var updatedRecipe = GetFullRecipeAndFullIngredientsForRecipe(r);
            var commandText = "update recipes set recipe_name=@recipe_name, yield=@yield, aggregated_price=@aggregated_price where recipe_id=@rid;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@recipe_name", r.name);
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                cmd.Parameters.AddWithValue("@aggregated_price", updatedRecipe.aggregatedPrice);
                cmd.Parameters.AddWithValue("@price_per_serving", updatedRecipe.pricePerServing);
                return cmd;
            });
        }
        public void InsertRecipe(Recipe r) {
            var db = new DatabaseAccess();
            var commandText = "Insert into recipes (recipe_name, yield, aggregated_price, price_per_serving) values (@recipe_name, @yield, @aggregated_price, @price_per_serving);";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@recipe_name", r.name);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                cmd.Parameters.AddWithValue("@aggregated_price", r.aggregatedPrice);
                cmd.Parameters.AddWithValue("@price_per_serving", r.pricePerServing);
                return cmd;
            });
        }
        public Recipe GetFullRecipeAndFullIngredientsForRecipe(Recipe r) {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed();
            var myRecipe = new Recipe();
            //var myIngredient = new Ingredient();
            var myListOfIngredients = new List<Ingredient>();
            var myIngredientTable = dbI.queryAllIngredientsFromIngredientTable();
            var myConsumptionTable = dbCOC.queryConsumptionOuncesConsumed();
            var myRecipeTableName = queryRecipeFromRecipesTableByName(r);
            //maybe this is going off of order... the ingredients is the first table, the consumption_ounces_consumed, then the name
            var commandText = string.Format(@"SELECT * FROM ingredients
                                                JOIN consumption_ounces_consumed
                                                ON ingredients.name=consumption_ounces_consumed.name AND ingredients.ing_id=consumption_ounces_consumed.ing_id
                                                JOIN recipes
                                                ON ingredients.recipe_id=recipes.recipe_id
                                                WHERE recipes.recipe_id={0};", r.id);
            myListOfIngredients = db.queryItems(commandText, reader => {
                var myIngredient = new Ingredient((string)(reader["name"]));
                myIngredient.ingredientId = (int)reader["ing_id"];
                myIngredient.measurement = (string)reader["measurement"];
                myIngredient.classification = (string)reader["ingredient_classification"];
                myIngredient.typeOfIngredient = (string)reader["ingredient_type"];
                myIngredient.priceOfMeasuredConsumption = (decimal)reader["price_measured_ingredient"];
                myIngredient.recipeId = (int)reader["recipe_id"];
                myIngredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                myIngredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                myIngredient.itemId = (int)reader["item_id"];
                myIngredient.itemResponseName = (string)reader["item_response_name"];
                var expirationDate = dbI.convertStringMMDDYYYYToDateYYYYMMDD((string)reader["expiration_date"]);
                myIngredient.expirationDate = expirationDate;
                return myIngredient;
            });
            db.queryItems(commandText, reader => {
                myRecipe.id = (int)reader["recipe_id"];
                myRecipe.name = (string)reader["recipe_name"];
                myRecipe.yield = (int)reader["yield"];
                myRecipe.aggregatedPrice = (decimal)reader["aggregated_price"];
                myRecipe.pricePerServing = (decimal)reader["price_per_serving"];
                return myRecipe;
            });
            myRecipe.ingredients = myListOfIngredients;
            if (myRecipe.aggregatedPrice == 0) {
                foreach (var ingredient in myRecipe.ingredients)
                    myRecipe.aggregatedPrice += ingredient.priceOfMeasuredConsumption;
            }
            myRecipe.pricePerServing = ReturnRecipePricePerServing(myRecipe);
            //UpdateRecipe(myRecipe);
            var myUpdatedRecipe = queryRecipeFromRecipesTableByName(myRecipe);
            return myRecipe;
        }
        public List<Recipe> MyRecipeBox() {
            var myRecipes = queryRecipes();
            var myRecipeBox = new List<Recipe>();
            foreach (var recipe in myRecipes) {
                recipe.ingredients = GetRecipeIngredients(recipe);
                recipe.aggregatedPrice = ReturnFullRecipePrice(recipe);
                recipe.pricePerServing = ReturnRecipePricePerServing(recipe);
                myRecipeBox.Add(recipe);
            }
            return myRecipeBox;
        }
        public void GetFullRecipePrice(Recipe r) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var myIngredients = GetFullRecipeAndFullIngredientsForRecipe(r).ingredients;
            foreach (var ing in myIngredients) {
                if (ing.recipeId == r.id) {
                    dbIngredients.getIngredientMeasuredPrice(ing, r);
                    r.ingredients.Add(ing);
                    db.updateAllTables(ing, r);
                    var currentIngredient = db.queryAllRelevantTablesSQLByIngredientName(ing);
                }
            }
            var aggregatedPrice = 0m;
            foreach (var ing in r.ingredients) {
                aggregatedPrice += ing.priceOfMeasuredConsumption;
            }
            r.aggregatedPrice = aggregatedPrice;
            UpdateRecipe(r);
        }
        //public Func<Recipe, decimal> ReturnRecipePricePerServing = r => Math.Round((r.aggregatedPrice /r.yield), 2);
        public decimal ReturnRecipePricePerServing(Recipe r) {
            if (r.yield == 0)
                return 0m;
            else return (Math.Round((r.aggregatedPrice / r.yield), 2));
        }
        public decimal ReturnFullRecipePrice(Recipe r) {
            var myIngredients = GetFullRecipeAndFullIngredientsForRecipe(r).ingredients;
            var aggregatedPrice = 0m;
            foreach (var ing in r.ingredients) {
                aggregatedPrice += ing.priceOfMeasuredConsumption;
            }
            r.aggregatedPrice = aggregatedPrice;
            UpdateRecipe(r);
            return aggregatedPrice;
        }
        public void DeleteRecipeAndRecipeIngredients(Recipe r) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            r.name = r.name.Trim();
            var myRecipe = GetFullRecipeAndFullIngredientsForRecipe(r);
            var myIngredients = dbIngredients.queryAllIngredientsFromIngredientTable();
            foreach (var ingredient in myRecipe.ingredients) {
                dbIngredients.DeleteIngredientFromIngredientTable(ingredient);
            }
            //this will change the ingredient ids... i may have to go through and make sure all my logic for comparing ids will still be compatible when i start deleting stuff... lots of integrative testing needs to be done with that
            var delete = "DELETE FROM recipes WHERE name=@name";
            db.executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                return cmd;
            });
        }
        //i need to rewrite the query to call a different () to get all the information for all the ingredients... i'm not getting an ing.id, densities...
        //only the name and the measurement

        public void UpdateRecipeYield(Recipe r) {
            var db = new DatabaseAccess();
            var dbDensities = new DatabaseAccessDensityInformation();
            var convert = new ConvertMeasurement();
            var myRecipeBox = MyRecipeBox();
            foreach (var recipe in myRecipeBox) {
                var myIngredients = db.queryAllTablesForAllIngredients(recipe.ingredients);
                var tempIngredient = new Ingredient();
                if (recipe.id == r.id) {
                    foreach (var ingredient in recipe.ingredients) {
                        tempIngredient = ingredient;
                        if (tempIngredient.density == 0)
                            tempIngredient.density = dbDensities.queryDensityTableRowDensityValueByName(ingredient);
                        tempIngredient.measurement = convert.AdjustIngredientMeasurement(ingredient.measurement, recipe.yield, r.yield);
                        tempIngredient.ouncesConsumed = ingredient.ouncesConsumed * (HomeController.currentRecipe.yield / r.yield);
                        db.updateAllTables(tempIngredient, r);
                        var myUpdatedIngredient = db.queryAllRelevantTablesSQLByIngredientName(tempIngredient);
                    }
                    recipe.yield = r.yield;
                    GetFullRecipePrice(recipe);
                    var myUpdatedIngredients = db.queryAllTablesForAllIngredients(recipe.ingredients);
                }
            }
        }
        public void UpdateListOfRecipeYields(List<Recipe> myListOfRecipes) {
            var myListOfRecipesIds = new List<int>();
            foreach (var recipe in myListOfRecipes) {
                if (!myListOfRecipesIds.Contains(recipe.id)) {
                    myListOfRecipesIds.Add(recipe.id);
                }
            }
            var myUpdatedRecipeBox = MyRecipeBox();
            foreach (var recipe in myListOfRecipes) {
                var currentRecipe = GetFullRecipeAndFullIngredientsForRecipe(recipe);
                UpdateRecipeYield(recipe);
            }
        }
        public List<Ingredient> GetRecipeIngredients(Recipe r) {
            var db = new DatabaseAccess();
            var joinCommand = @"SELECT ingredients.name, 
                                    ingredients.measurement, 
                                    ingredients.price_measured_ingredient
                                FROM recipes
                                JOIN ingredients
                                ON recipes.recipe_id=ingredients.recipe_id;";
            var myRecipeIngredients = db.queryItems(joinCommand, reader => {
                //var recipe = new Recipe(reader["recipes.name"].ToString());
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.measurement = (string)reader["measurement"].ToString();
                ingredient.priceOfMeasuredConsumption = (decimal)reader["price_measured_ingredient"];
                return ingredient;
            });
            return myRecipeIngredients;
        }
        public Recipe queryRecipeFromRecipesTableByName(Recipe r) {
            var db = new DatabaseAccess();
            var myRecipe = new Recipe();
            var commandTextQueryRecipeByName = string.Format(@"SELECT * FROM recipes where recipe_name='{0}';", r.name);
            db.queryItems(commandTextQueryRecipeByName, reader => {
                myRecipe.name = (string)(reader["recipe_name"]);
                myRecipe.id = (int)(reader["recipe_id"]);
                myRecipe.yield = (int)(reader["yield"]);
                myRecipe.aggregatedPrice = (decimal)(reader["aggregated_price"]);
                myRecipe.pricePerServing = (decimal)(reader["price_per_serving"]);
                return myRecipe;
            });
            return myRecipe;
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it