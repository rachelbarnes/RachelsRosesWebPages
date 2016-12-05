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
            //if thing start blowing up, tryi implementing cmd.CommandType = CommandType.Text; 
            sqlConnection1.Open();
            var reader = cmd.ExecuteReader();
            List<T> items = new List<T>();
            while (reader.Read()) {
                items.Add(convert(reader));
            }
            sqlConnection1.Close();
            return items;
            //this convert method, the second parameter which is a lambda, is defined in the methods that then call queryItems and has hte command, and then the convert method
        }



        public void UpdateRecipe(Recipe r) {
            var commandText = "update recipes set name=@name, yield=@yield where recipe_id = @rid;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                return cmd;
            });
            //modifying the inside of the function by assigning the parameters
            //passing a lambda of what to do in the middle
        }
        /*for a method for a sql connection and read/write method: 
            make the sql connection, make an instance of the SqlCommand class
            cmd.CommandText is the sequal command that you're reading or writing
            cmd.CommandType is the command type, which in this case is text
            cmd.Connection is the sql connection with the SqlConnection, with the connection string as a parameter
            cmd.Parameters will occur for each parameter, with a method of AddWithValue and having the parameter from the command text (look above for an example)
                can these cmd.Parameters.AddWithValue be "grouped"? (so for example ("@name", r.name, "@rid", r.id, "@yield", r.yield)
            open the sqlconnection, execute the query form the cmd instance, close the sql connection

            for the record, Steve said it is normal to have the majority of the code in a project be back tier/bottom tier sql commands 
        */
        public void InsertRecipe(Recipe r) {
            var commandText = "Insert into recipes (name, yield) values (@name, @yield);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", r.name);
                cmd.Parameters.AddWithValue("@yield", r.yield);
                return cmd;
            });
        }
        //cmd.CommandText = "Insert into recipes (name, yield) values (@name, @yield);";
        //cmd.CommandType = CommandType.Text;//both of these don't need to be set because the ExecuteVoidQuery does this action for us (look at the () again, closer)
        //cmd.Connection = sqlConnection1;// and this
        //duplication : helper function, execute and modify commands 
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
            var delete = "DELETE FROM recipes WHERE @name = @title"; 
            executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@name", "name");
                cmd.Parameters.AddWithValue("@title", recipeTitle);
                return cmd; 
            });
            //this is breaking on line 18
        }
        public List<Recipe> queryRecipe(string tableName) {
            return queryItems("select * from " + tableName, reader => {
                var recipe = new Recipe(reader["name"].ToString());
                recipe.id = (int)reader["recipe_id"];
                recipe.yield = (int)reader["yield"]; //these are the column names that you're accessing
                return recipe;
            });
        }
        //the second part of this 
        //get the recipe with the ingredients populated
        //gives a list of ingredients with the recipe id (which is a table in the database, and match that up with the recipe id from a recipe, and return the populated recipe
        //public Recipe querySingleRecipe(string recipeTitle) {
        //    executeVoidQuery("SELECT name, yield FROM recipes WHERE name = " + recipeTitle, cmd => cmd);
        //}
        public List<Ingredient> queryIngredient() {
            return queryItems("select * from ingredients", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.measurement = (string)reader["measurement"];
                ingredient.recipeId = (int)reader["recipe_id"];
                return ingredient;
            });
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
