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
   //var commandText = 
            //var commandTextQueryAllRelevantColumns = @"SELECT ingredients.ing_id, 
			         //                                   ingredients.recipe_id,
				        //                                ingredients.name, 
            //                                			ingredients.measurement, 
			         //                                   consumption_ounces_consumed.ounces_consumed, 
            //                                            consumption_ounces_consumed.ounces_remaining,
			         //                                   ingredients.ingredient_classification, 
			         //                                   ingredients.ingredient_type, 
			         //                                   ingredients.price_measured_ingredient, 
			         //                                   ingredients.expiration_date, 
			         //                                   costs.selling_price, 
			         //                                   costs.price_per_ounce, 
			         //                                   densities.selling_weight, 
			         //                                   densities.selling_weight_ounces
	           //                                     FROM ingredients 
	           //                                     INNER JOIN consumption_ounces_consumed
		          //                                  ON ingredients.name=consumption_ounces_consumed.name
	           //                                     INNER JOIN costs 
		          //                                  ON ingredients.name=costs.name
	           //                                     INNER JOIN densities
		          //                                  ON ingredients.name=densities.name;";
            ////i'm getting nothing from this... need to look at this when im more awake
            //queryItems(commandTextQueryAllRelevantColumns, reader => {
            //    queriedIngredient.name = (string)(reader["name"]);
            //    queriedIngredient.ingredientId = (int)(reader["ing_id"]);
            //    queriedIngredient.recipeId = (int)(reader["recipe_id"]);
            //    queriedIngredient.measurement = (string)(reader["measurement"]);
            //    queriedIngredient.ouncesConsumed = (decimal)(reader["ounces_consumed"]);
            //    queriedIngredient.ouncesConsumed = (decimal)(reader["ounces_remaining"]);
            //    queriedIngredient.classification = (string)(reader["ingredient_classification"]);
            //    queriedIngredient.typeOfIngredient = (string)(reader["ingredient_type"]);
            //    queriedIngredient.pricePerOunce = (decimal)(reader["price_per_ounce"]);
            //    queriedIngredient.priceOfMeasuredConsumption = (decimal)(reader["price_measured_ingredient"]);
            //    queriedIngredient.sellingPrice = (decimal)(reader["selling_price"]);
            //    queriedIngredient.sellingWeight = (string)(reader["selling_weight"]);
            //    queriedIngredient.sellingWeightInOunces = (decimal)(reader["selling_weight_ounces"]);
            //    var expirationDate = (string)(reader["expiration_date"]);
            //    queriedIngredient.expirationDate = dbI.convertStringMMDDYYYYToDateYYYYMMDD(expirationDate);
            //    queriedIngredientList.Add(queriedIngredient);
            //    return queriedIngredient;
            //});
            ////}
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
        //var commandTextQueryAllRelevantTables = string.Format(@"SELECT ingredients.ing_id, 
        //                                       ingredients.recipe_id,
        //                                    ingredients.name, 
        //                                    			ingredients.measurement, 
        //                                       consumption_ounces_consumed.ounces_consumed, 
        //                                                consumption_ounces_consumed.ounces_remaining,
        //                                       ingredients.ingredient_classification, 
        //                                       ingredients.ingredient_type, 
        //                                       ingredients.price_measured_ingredient, 
        //                                       ingredients.expiration_date, 
        //                                       costs.selling_price, 
        //                                       costs.price_per_ounce, 
        //                                       densities.selling_weight, 
        //                                       densities.selling_weight_ounces
        //                                         FROM ingredients 
        //                                         INNER JOIN consumption_ounces_consumed
        //                                      ON ingredients.name=consumption_ounces_consumed.name
        //                                         INNER JOIN costs 
        //                                      ON ingredients.name=costs.name
        //                                         INNER JOIN densities
        //                                      ON ingredients.name=densities.name
        //                                            WHERE ingredients.name='{0}';", i.name);
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
            //var commandText = string.Format(@"SELECT * FROM ingredients, consumption_ounces_consumed, costs, densities
            //                               WHERE ingredients.name='{0}' AND ingredients.ing_id={1} AND ingredients.measurement='{2}';", i.name, i.ingredientId, i.measurement);

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
                var expirationDate = (string)(reader["expiration_date"]);
                queriedIngredient.expirationDate = dbI.convertStringMMDDYYYYToDateYYYYMMDD(expirationDate);
                return queriedIngredient;
            });
            return queriedIngredient;
        }
        //public Ingredient queryAllRelevantTables(Ingredient i) {
        //    var dbRecipes = new DatabaseAccessRecipe();
        //    var dbIngredients = new DatabaseAccessIngredient();
        //    var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
        //    var dbConsumption = new DatabaseAccessConsumption();
        //    var dbDensities = new DatabaseAccessDensities();
        //    var dbCosts = new DatabaseAccessCosts();
        //    var rest = new MakeRESTCalls();
        //    var myRecipes = dbRecipes.queryRecipes();
        //    var myIngredients = dbIngredients.queryIngredients();
        //    var myIngredientConsumptionOuncesConsumed = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed();
        //    var myIngredientConsumption = dbConsumption.queryConsumptionTable();
        //    var myIngredientDensity = dbDensities.queryDensitiesTable();
        //    var myIngredientCost = dbCosts.queryCostTable();
        //    //i'd be really interested in making these queries singletons in each of these classes... 
        //    //if i'm accessing them all the time, it would save a lot of time... definitely worth checking out
        //    var temp = new Recipe();
        //    foreach (var rec in myRecipes) {
        //        if (rec.id == i.recipeId) {
        //            temp = rec;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredients) {
        //        if (ing.ingredientId == i.ingredientId) {
        //            i.recipeId = ing.recipeId;
        //            i.measurement = ing.measurement;
        //            i.typeOfIngredient = ing.typeOfIngredient;
        //            if (i.itemId == 0 && !i.classification.ToLower().Contains("egg") && !i.classification.ToLower().Contains("dairy") && !string.IsNullOrEmpty(i.classification))
        //                i.itemId = myItemResponse.itemId;
        //            else i.itemId = ing.itemId;
        //            i.expirationDate = ing.expirationDate;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredientConsumptionOuncesConsumed) {
        //        if (ing.name == i.name && ing.measurement == i.measurement) {
        //            i.ouncesConsumed = ing.ouncesConsumed;
        //            break;
        //        }
        //    }
        //    foreach (var ing in myIngredientConsumption) {
        //        if (ing.name == i.name) {
        //            i.density = ing.density;
        //            i.ouncesRemaining = ing.ouncesRemaining;
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
        //            if (ing.sellingPrice == 0m && !i.classification.ToLower().Contains("egg") && !i.classification.ToLower().Contains("dairy"))
        //                i.sellingPrice = myItemResponse.salePrice;
        //            else i.sellingPrice = ing.sellingPrice;
        //            if (ing.pricePerOunce == 0m)
        //                i.pricePerOunce = (i.sellingPrice / i.sellingWeightInOunces);
        //            else i.pricePerOunce = ing.pricePerOunce;
        //            i.itemId = ing.itemId;
        //            break;
        //        }
        //    }
        //    if (i.ouncesConsumed != 0m && i.ouncesRemaining != 0m && i.priceOfMeasuredConsumption == 0m) {
        //        foreach (var ing in myIngredients) {
        //            if (ing.ingredientId == i.ingredientId) {
        //                i.priceOfMeasuredConsumption = dbIngredients.MeasuredIngredientPrice(i);
        //                break;
        //            }
        //        }
        //    }
        //    return i;
        //}
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
            var myIngredientBox = dbIngredients.queryAllIngredientsFromIngredientTable();
            var myIngredients = queryAllRelevantTablesSQLByIngredientName(i);
            var count = 0;
            var countIngredients = 0;
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id)
                    count++;
            }
            if (count == 0)
                dbRecipes.InsertRecipe(r);
            foreach (var ingredient in myIngredientBox) {
                if (ingredient.ingredientId == i.ingredientId) {
                    countIngredients++;
                    break;
                }
            }
            if (countIngredients == 0) {
                dbIngredients.insertIngredient(i, r);
                var myIng = queryAllRelevantTablesSQLByIngredientName(i);
                dbDensitiesInformation.insertIngredientIntoDensityInfoDatabase(i);
                dbDensities.insertIngredientDensityData(i);
                dbConsumption.insertIngredientConsumtionData(i);
                var myIngUpdated = queryAllRelevantTablesSQLByIngredientName(i);
                dbCosts.insertIngredientCostDataCostTable(i);
                dbIngredients.UpdateIngredient(i);
                var myIngUpdated2 = queryAllRelevantTablesSQLByIngredientName(i);
            } else {
                dbIngredients.UpdateIngredient(i);
                var updatedIngredient = queryAllRelevantTablesSQLByIngredientName(i);
                dbDensitiesInformation.updateDensityInfoTable(i);
                dbDensities.updateDensityTable(i);
                dbCosts.updateCostDataTable(i);
                dbIngredients.UpdateIngredient(i);
            }
        }
        public void insertListOfIngredientsIntoAllTables(List<Ingredient> ListOfIngredients, Recipe r) {
            var dbRecipes = new DatabaseAccessRecipe();
            var dbIngredients = new DatabaseAccessIngredient();
            var myListOfIngredientIds = new List<int>();
            var myListOfRecipeIds = new List<int>();
            foreach (var ingredient in ListOfIngredients) {
                if (!myListOfIngredientIds.Contains(ingredient.ingredientId)) {
                    insertIngredientIntoAllTables(ingredient, r);
                    myListOfIngredientIds.Add(ingredient.ingredientId);
                }
            }
            var myRecipes = dbRecipes.queryRecipes();
            var myIngredients = dbIngredients.queryAllIngredientsFromIngredientTable();
            foreach (var recipe in myRecipes)
                myListOfRecipeIds.Add(recipe.id);
            if (!myListOfRecipeIds.Contains(r.id))
                dbRecipes.InsertRecipe(r);
            foreach (var ingredient in myIngredients) {
                if (!myListOfIngredientIds.Contains(ingredient.ingredientId))
                    myListOfIngredientIds.Add(ingredient.ingredientId);
            }
            var myRecipe = dbRecipes.GetFullRecipe(r);
            foreach (var ingredient in myRecipe.ingredients) {
                if (!myListOfIngredientIds.Contains(ingredient.ingredientId))
                    insertIngredientIntoAllTables(ingredient, r);
            }
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
                        name nvarchar(max), 
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
            //this ingredient name is to represent the ingredient.typeOfIngredient
            executeVoidQuery("SET IDENTITY_INSERT densities ON", cmd => cmd);
            //insertDensityTextFileIntoDensityInfoDatabase();
        }
    }
}