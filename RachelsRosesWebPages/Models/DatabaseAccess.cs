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



        public void UpdateRecipe(Recipe r) {
            var commandText = "update recipes set name=@name, yield=@yield where recipe_id = @rid;";
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
        public void InsertIngredient(Ingredient i, Recipe r) {
            var commandText = "Insert into ingredients(recipe_id, name, measurement) values (@rid, @name, @measurement);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                return cmd;
            });
        }
        public void DeleteRecipe(string recipeTitle) {
            recipeTitle = recipeTitle.Trim();
            var delete = "DELETE FROM recipes WHERE name=@title";
            executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@title", recipeTitle);
                return cmd;
            });
        }
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
        public List<Ingredient> queryIngredients() {
            return queryItems("select * from ingredients", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.measurement = (string)reader["measurement"];
                ingredient.recipeId = (int)reader["recipe_id"];
                return ingredient;
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
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it
