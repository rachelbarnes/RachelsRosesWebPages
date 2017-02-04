using RachelsRosesWebPages.Controllers;
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
                        ingredient nvarchar(max),
                        density decimal(4,2)
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
                var recipe = new Recipe(reader["name"].ToString());
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
            var myRecipes = queryRecipes();
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id) {
                    if (recipe.yield != r.yield) {
                        recipe.yield = r.yield;
                    }
                }
            }
            var commandText = "update recipes set name=@name, yield=@yield, aggregated_price=@aggregated_price where recipe_id=@rid;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                cmd.Parameters.AddWithValue("@aggregated_price", r.aggregatedPrice);
                cmd.Parameters.AddWithValue("@price_per_serving", r.pricePerServing); 
                return cmd;
            });
        }
        public void InsertRecipe(Recipe r) {
            var db = new DatabaseAccess();
            var commandText = "Insert into recipes (name, yield, aggregated_price, price_per_serving) values (@name, @yield, @aggregated_price, @price_per_serving);";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                cmd.Parameters.AddWithValue("@aggregated_price", r.aggregatedPrice);
                cmd.Parameters.AddWithValue("@price_per_serving", r.pricePerServing); 
                return cmd;
            });
        }
        public Recipe GetFullRecipe(Recipe r) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var aggregatedPrice = 0m;
            var myRecipeBox = queryRecipes();
            var myIngredients = dbIngredients.queryAllIngredientsFromIngredientTable();
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
                    var currentIngredient = db.queryAllRelevantTablesSQL(ingredient);
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
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var myIngredients = GetFullRecipe(r).ingredients;
            foreach (var ing in myIngredients) {
                if (ing.recipeId == r.id) {
                    dbIngredients.getIngredientMeasuredPrice(ing, r);
                    r.ingredients.Add(ing);
                    db.updateAllTables(ing, r);
                    var currentIngredient = db.queryAllRelevantTablesSQL(ing);
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
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var myIngredients = dbIngredients.queryAllIngredientsFromIngredientTable();
            foreach (var ing in myIngredients) {
                if (ing.recipeId == r.id) {
                    db.queryAllRelevantTablesSQL(ing);
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
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            r.name = r.name.Trim();
            var myRecipe = GetFullRecipe(r);
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
                        var myUpdatedIngredient = db.queryAllRelevantTablesSQL(tempIngredient);
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
                var currentRecipe = GetFullRecipe(recipe);
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
            var commandTextQueryRecipeByName = string.Format(@"SELECT * FROM recipes where name='{0}';", r.name);
            db.queryItems(commandTextQueryRecipeByName, reader => {
                myRecipe.name = (string)(reader["name"]);
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