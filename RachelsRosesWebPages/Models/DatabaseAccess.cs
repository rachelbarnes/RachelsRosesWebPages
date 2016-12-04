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

        public void UpdateRecipe(Recipe r) {
            SqlConnection sqlConnection1 = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "update recipes set name=@name, yield=@yield where recipe_id = @rid;";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.AddWithValue("@name", r.name);
            cmd.Parameters.AddWithValue("@rid", r.id);
            cmd.Parameters.AddWithValue("@yield", r.yield);
            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();
        }
        public void InsertRecipe(Recipe r) {
            SqlConnection sqlConnection1 = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Insert into recipes (name, yield) values (@name, @yield);";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.AddWithValue("@name", r.name);
            cmd.Parameters.AddWithValue("@yield", r.yield);
            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();
        }

        public void InsertIngredient(Ingredient i, Recipe r) {
            SqlConnection sqlConnection1 = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Insert into ingredients(name, recipe_id) values (@name, @rid);";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.AddWithValue("@name", i.name);
            cmd.Parameters.AddWithValue("@rid", r.id);
            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();
        }
        public List<Recipe> queryRecipe() {
            SqlConnection sqlConnection1 = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            cmd.CommandText =  "select * from recipes;";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            sqlConnection1.Open();
            reader = cmd.ExecuteReader();
            List<Recipe> r = new List<Recipe>();
            while (reader.Read()) {
                r.Add(new Recipe(reader["name"].ToString()) {
                    yield = (int)reader["yield"],
                    id = (int)reader["recipe_id"]
                });
            }
            sqlConnection1.Close();
            return r;
        }
        // read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it
        public void initializeDatabase() {
            dropTableIfExists("recipes");
            executeVoidQuery(@"create table recipes (
                        recipe_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        yield int
                     );");

            dropTableIfExists("ingredients");
            executeVoidQuery(@"create table ingredients (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        recipe_id Int,
                        name nvarchar(max), 
                        measurement nvarchar(max)
                     );");
        }
        public void dropTableIfExists(string table) {
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            executeVoidQuery(drop);
        }

        public void executeVoidQuery(string command) {
            var con = new SqlConnection(connString);
            try {
                con.Open();
                SqlCommand createTestTable = new SqlCommand(command, con);
                createTestTable.ExecuteNonQuery();
                con.Close();
            } catch (Exception e) {
                Console.WriteLine("Query Faild somehow");
                throw e;
            }
        }
    }

}