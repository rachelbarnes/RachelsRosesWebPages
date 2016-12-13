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
                recipe.yield = (int)reader["yield"]; //these are the column names that you're accessing
                return recipe;
            });
            foreach (var recipe in MyRecipeBox)
                recipe.id = count++;
            return MyRecipeBox;
        }
        public void UpdateRecipe(Recipe r) {
            var commandText = "update recipes set name=@name, yield=@yield where recipe_id=@rid;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                return cmd;
            });
        }
        public void InsertRecipe(Recipe r) {
            var commandText = "Insert into recipes (name, yield) values (@name, @yield);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                return cmd;
            });
        }
        public Recipe GetFullRecipe(string myRecipeName) {
            var myRecipeBox = queryRecipes();
            var myIngredients = queryIngredients();
            var myRecipe = new Recipe();
            foreach (var recipe in myRecipeBox) {
                if (recipe.name == myRecipeName) {
                    myRecipe = recipe;
                    break;
                }
            }
            foreach (var ingredient in myIngredients) {
                if (ingredient.recipeId == myRecipe.id)
                    myRecipe.ingredients.Add(ingredient);
            }
            return myRecipe;
        }
        public void DeleteRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            var delete = "DELETE FROM recipes WHERE name=@title";
            executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@title", recipeTitle);
                return cmd;
            });
        }

        //ingredient table methods: 
        public List<Ingredient> queryIngredients() {
            var count = 1;
            var myIngredientBox = queryItems("select * from ingredients", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.measurement = (string)reader["measurement"];
                ingredient.recipeId = (int)reader["recipe_id"];
                return ingredient;
            });
            foreach (var ingredient in myIngredientBox)
                ingredient.ingredientId = count++;
            return myIngredientBox;
        }
        public void InsertIngredient(Ingredient i, Recipe r) {
            var commandText = "Insert into ingredients(recipe_id, name, measurement) values (@rid, @name, @measurement);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                return cmd;
            });
        }
        public void UpdateIngredient(Ingredient i) {
            var commandText = "update ingredients set name=@name, measurement=@measurement, recipe_id=@recipeId where ing_id=@ingredientId";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@recipeId", i.recipeId);
                cmd.Parameters.AddWithValue("ingredientId", i.ingredientId);
                return cmd;
            });
        }

        //densities table methods: 
        public List<Ingredient> queryDensitiesAndPrices() {
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
        public void InsertIngredientDensityAndSellingInformation(Ingredient i) {
            var commandText = @"Insert into densities(name, density, selling_weight, selling_weight_ounces, selling_price, price_per_ounce) 
                            values (@name, @density, @selling_weight, @selling_weight_ounces, @selling_price, @price_per_ounce);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                return cmd;
            });
        }
        public void updateDensitiesAndPrice(Ingredient i) {
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
        public Func<Ingredient, decimal> CalculatePricePerOunce = i => Math.Round((i.sellingPrice / i.sellingWeightInOunces), 2);
        public void GetPricePerOunceFromDensityTable(Ingredient i) {
            var myIngredientBox = queryDensitiesAndPrices();
            var calculatedPricePerOunce = 0m;
            foreach (var ingredient in myIngredientBox) {
                if (ingredient.name == i.name)
                    calculatedPricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 2);
            }
            i.pricePerOunce = calculatedPricePerOunce;
            updateDensitiesAndPrice(i);
        }
        public void UpdateSellingWeightInOunces(Ingredient i) {
            var convert = new ConvertWeight();
            var myIngredientInfo = queryDensitiesAndPrices();
            foreach (var ingredient in myIngredientInfo) {
                if (ingredient.name == i.name) {
                    ingredient.sellingWeightInOunces = convert.ConvertWeightToOunces(ingredient.sellingWeight);
                    updateDensitiesAndPrice(ingredient);
                    break; 
                }
            }
        }

        //next step is a rest call, and to assign those ingredient sellingprices their appropriate assignments 


        //initalize database tables
        public void dropTableIfExists(string table) {
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            executeVoidQuery(drop, a => a);
        }
        public void initializeDatabase() {
            dropTableIfExists("recipes");
            executeVoidQuery(@"create table recipes (
                        recipe_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        yield int
                     );", a => a);

            dropTableIfExists("ingredients");
            executeVoidQuery(@"create table ingredients (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        recipe_id Int,
                        name nvarchar(max), 
                        measurement nvarchar(max)
                     );", a => a);
            dropTableIfExists("densities");
            executeVoidQuery(@"create table densities (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        density decimal (4,2),
                        selling_weight varchar(max),
                        selling_weight_ounces decimal(6,2),
                        selling_price decimal(6,2),
                        price_per_ounce decimal(6,2)
                     );", a => a);
            executeVoidQuery("SET IDENTITY_INSERT densities ON", cmd => cmd);
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it
