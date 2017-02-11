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
        //public T querySingleItem<T>(string command, Func<SqlDataReader, T> convert) {
        //    var sqlConnection1 = new SqlConnection(connString);
        //    var cmd = new SqlCommand(command, sqlConnection1);
        //    sqlConnection1.Open();
        //    var reader = cmd.ExecuteReader();
        //    T item;
        //    //var items = new <T>();
        //    item = convert(reader); 
        //    //items = convert(reader); 
        //    //while (reader.Read()) {
        //    //    items.Add(convert(reader));
        //    //}
        //    sqlConnection1.Close();
        //    return item;
        //}
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
        public List<Ingredient> queryAllRelevantTablesSQLForListOfIngredients(List<Ingredient> listOfIngredients) {
            var dbI = new DatabaseAccessIngredient();
            var queriedIngredientList = new List<Ingredient>();
            foreach (var ingredient in listOfIngredients) {
                var queriedIngredient = new Ingredient();
                queriedIngredient = queryAllRelevantTablesSQLByIngredientName(ingredient);
                queriedIngredientList.Add(queriedIngredient);
            }
            return queriedIngredientList;
        }
        public Ingredient queryAllRelevantTablesSQLByIngredientName(Ingredient i) {
            var dbI = new DatabaseAccessIngredient();
            var queriedIngredient = new Ingredient();
            var commandText = string.Format(@"SELECT * 
                                               FROM ingredients
                                               JOIN consumption_ounces_consumed
                                               ON ingredients.name=consumption_ounces_consumed.name AND ingredients.ing_id=consumption_ounces_consumed.ing_id
                                               JOIN costs
                                               ON ingredients.name=costs.name AND ingredients.ing_id=costs.ing_id
                                               JOIN densities
                                               ON ingredients.name=densities.name AND ingredients.ing_id=densities.ing_id
                                               WHERE ingredients.name='{0}' AND ingredients.ing_id={1};", i.name, i.ingredientId);
            queryItems(commandText, reader => {
                queriedIngredient.name = (string)(reader["name"]);
                queriedIngredient.ingredientId = (int)(reader["ing_id"]);
                queriedIngredient.recipeId = (int)(reader["recipe_id"]);
                queriedIngredient.measurement = (string)(reader["measurement"]);
                queriedIngredient.ouncesConsumed = (decimal)(reader["ounces_consumed"]);
                queriedIngredient.ouncesRemaining = (decimal)(reader["ounces_remaining"]);
                queriedIngredient.classification = (string)(reader["ingredient_classification"]);
                queriedIngredient.typeOfIngredient = (string)(reader["ingredient_type"]);
                queriedIngredient.pricePerOunce = (decimal)(reader["price_per_ounce"]);
                queriedIngredient.priceOfMeasuredConsumption = (decimal)(reader["price_measured_ingredient"]);
                queriedIngredient.sellingPrice = (decimal)(reader["selling_price"]);
                queriedIngredient.sellingWeight = (string)(reader["selling_weight"]);
                queriedIngredient.sellingWeightInOunces = (decimal)(reader["selling_weight_ounces"]);
                queriedIngredient.density = (decimal)reader["density"];
                var expirationDate = (string)(reader["expiration_date"]);
                queriedIngredient.expirationDate = dbI.convertStringMMDDYYYYToDateYYYYMMDD(expirationDate);
                return queriedIngredient;
            });
            return queriedIngredient;
        }
        public List<Ingredient> queryAllTablesForAllIngredients(List<Ingredient> ListOfIngredients) {
            var queriedListOfIngredients = new List<Ingredient>();
            foreach (var ingredient in ListOfIngredients)
                queriedListOfIngredients.Add(queryAllRelevantTablesSQLByIngredientName(ingredient));
            return queriedListOfIngredients;
        }
        public void insertIngredientIntoAllTables(Ingredient i, Recipe r) {
            var dbRecipes = new DatabaseAccessRecipe();
            var dbIngredients = new DatabaseAccessIngredient();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var dbConsumption = new DatabaseAccessConsumption();
            var dbDensities = new DatabaseAccessDensities();
            var dbDensitiesInformation = new DatabaseAccessDensityInformation();
            var dbCosts = new DatabaseAccessCosts();
            var myRecipes = dbRecipes.queryRecipes();
            //var myIngredientBox = dbIngredients.queryAllIngredientsFromIngredientTable();
            var myIngredients = queryAllRelevantTablesSQLByIngredientName(i);
            var myRecipe = dbRecipes.queryRecipeFromRecipesTableByName(r);

            if (string.IsNullOrEmpty(myRecipe.name)) {
                dbRecipes.InsertRecipe(r);
            }
            dbIngredients.insertIngredient(i, r);
            var myIng = queryAllRelevantTablesSQLByIngredientName(i);
            dbDensitiesInformation.insertIngredientIntoDensityInfoDatabase(i);
            dbDensities.insertIngredientDensityData(i);
            dbConsumption.insertIngredientConsumtionData(i);
            var myIngUpdated = queryAllRelevantTablesSQLByIngredientName(i);
            dbCosts.insertIngredientCostDataCostTable(i);
            var myConsumptionIngredient = dbConsumption.queryConsumptionTableRowByName(i);
            dbIngredients.UpdateIngredient(i);
            var myUpdatedIngredient = queryAllRelevantTablesSQLByIngredientName(i); 
        }
        public void insertListOfIngredientsIntoAllTables(List<Ingredient> ListOfIngredients, Recipe r) {
            var dbR = new DatabaseAccessRecipe(); 
            foreach (var ingredient in ListOfIngredients)
                insertIngredientIntoAllTables(ingredient, r);
            dbR.UpdateRecipe(r);
            var myIngredientsSecond = queryAllTablesForAllIngredients(ListOfIngredients);
        }
        public void updateAllTables(Ingredient i, Recipe r) {
            var dbRecipes = new DatabaseAccessRecipe();
            var dbIngredients = new DatabaseAccessIngredient();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var dbConsumption = new DatabaseAccessConsumption();
            var dbDensities = new DatabaseAccessDensities();
            var dbDensityInformation = new DatabaseAccessDensityInformation();
            var dbCosts = new DatabaseAccessCosts();
            var myCostTable = dbCosts.queryCostTable();
            foreach (var ingredient in myCostTable) {
                if (ingredient.ingredientId == i.ingredientId) {
                    if (ingredient.sellingPrice == 0m && i.sellingPrice != 0m) {
                        dbCosts.updateCostDataTable(i);
                        break;
                    }
                }
            }
            dbRecipes.UpdateRecipe(r);
            dbIngredients.UpdateIngredient(i);
            var updatedIngredient = queryAllRelevantTablesSQLByIngredientName(i);
            dbDensityInformation.updateDensityInfoTable(i);
            dbDensities.updateDensityTable(i);
            dbCosts.updateCostDataTable(i);
        }
        public void updateAllTablesForAllIngredients(List<Ingredient> myListOfIngredients, Recipe r) {
            foreach (var ingredient in myListOfIngredients) {
                updateAllTables(ingredient, r);
            }
            var myUpdatedIngredients = queryAllTablesForAllIngredients(myListOfIngredients);
        }
        public void dropAllTablesIfTheyExist(string table) {
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
            dropAllTablesIfTheyExist("recipes");
            executeVoidQuery(@"create table recipes (
                        recipe_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        recipe_name nvarchar(max), 
                        yield int,
                        aggregated_price decimal(5, 2), 
                        price_per_serving decimal (5,2)
                     );", a => a);

            dropAllTablesIfTheyExist("ingredients");
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
            dropAllTablesIfTheyExist("densities");
            executeVoidQuery(@"create table densities (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        density decimal (4,2),
                        selling_weight varchar(250),
                        selling_weight_ounces decimal(6,2),
                        selling_price decimal(6,2),
                        price_per_ounce decimal(8,4)
                     );", a => a);
            dropAllTablesIfTheyExist("consumption");
            executeVoidQuery(@"create table consumption (
                        id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        name varchar(max),
                        measurement nvarchar(250),
                        density decimal (4,2),
                        ounces_consumed decimal (5,2),
                        ounces_remaining decimal(6,2),
                        refill int default 0,
                     );", a => a);
            dropAllTablesIfTheyExist("costs");
            executeVoidQuery(@"create table costs (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        name varchar (max),
                        selling_weight varchar(max),
                        selling_price decimal(6,2),
                        price_per_ounce decimal (6,4),
                        item_id int
                    );", a => a);
            dropAllTablesIfTheyExist("densityInfo");
            executeVoidQuery(@"create table densityInfo (
                        ing_id int,
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
            dropAllTablesIfTheyExist("consumption_ounces_consumed");
            executeVoidQuery(@"create table consumption_ounces_consumed (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                        name nvarchar(max), 
                        measurement nvarchar(max),
                        ounces_consumed decimal(5,2),
                        ounces_remaining decimal (6,2)
                        );", a => a);
            executeVoidQuery("SET IDENTITY_INSERT densities ON", cmd => cmd);
        }
    }
}