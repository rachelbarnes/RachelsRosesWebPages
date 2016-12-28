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
                recipe.yield = (int)reader["yield"];
                recipe.aggregatedPrice = (decimal)reader["aggregated_price"];
                return recipe;
            });
            foreach (var recipe in MyRecipeBox)
                recipe.id = count++;
            return MyRecipeBox;
        }
        public void UpdateRecipe(Recipe r) {
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
        public void GetFullRecipePrice(Recipe r) {
            var myIngredients = queryIngredients();
            //of course this isn't working... i'm not querying all the tables, i'm only querying the ingredients table, which has no cost, selling weight, etc. 
            foreach (var ing in myIngredients) {
                //ok, so my chocolateChips's recipeId is changed to 1
                if (ing.recipeId == r.id) {
                    getIngredientMeasuredPrice(ing, r);
                    r.ingredients.Add(ing);
                    updateAllTables(ing, r);
                }
            }
            var aggregatedPrice = 0m;
            foreach (var ing in r.ingredients) {
                aggregatedPrice += ing.priceOfMeasuredConsumption;
            }
            r.aggregatedPrice = aggregatedPrice;
            UpdateRecipe(r);
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
                ingredient.priceOfMeasuredConsumption = (decimal)reader["price_measured_ingredient"];
                ingredient.itemId = (int)reader["item_id"];
                return ingredient;
            });
            foreach (var ingredient in myIngredientBox)
                ingredient.ingredientId = count++;
            return myIngredientBox;
        }
        public Ingredient queryAllTablesForIngredient(Ingredient i) {
            //this isn't doing what it needs to do in the cost table???
            var rest = new MakeRESTCalls();
            var myRecipes = queryRecipes();
            var myIngredients = queryIngredients();
            var myIngredientConsumption = queryConsumptionTable();
            var myIngredientDensity = queryDensityTable();
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
                    i.measurement = ing.measurement;
                    i.itemId = ing.itemId;
                    break;
                }
            }
            foreach (var ing in myIngredientConsumption) {
                if (ing.ingredientId == i.ingredientId) {
                    i.density = ing.density;
                    i.ouncesConsumed = ing.ouncesConsumed;
                    i.ouncesRemaining = ing.ouncesRemaining;
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
                    if (ing.sellingPrice == 0m)
                        i.sellingPrice = rest.GetItemResponsePrice(i);
                    else i.sellingPrice = ing.sellingPrice;
                    //why am i having so much trouble here? 
                    if (ing.pricePerOunce == 0m)
                        i.pricePerOunce = (i.sellingPrice / i.sellingWeightInOunces);
                    else i.pricePerOunce = ing.pricePerOunce;
                    i.sellingWeight = ing.sellingWeight;
                    i.itemId = ing.itemId;
                    break;
                }
            }
            foreach (var ing in myIngredients) {
                if (ing.ingredientId == i.ingredientId) // || i.priceOfMeasuredConsumption != 0m)
                    i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
            }
            return i;
        }
        public List<Ingredient> queryAllTablesForAllIngredients(List<Ingredient> ListOfIngredients, Recipe r) {
            var queriedListOfIngredients = new List<Ingredient>();
            foreach (var ingredient in ListOfIngredients)

                queriedListOfIngredients.Add(queryAllTablesForIngredient(ingredient)); //, r)); 
            return queriedListOfIngredients;
        }
        public void insertIngredient(Ingredient i, Recipe r) {
            var commandText = "Insert into ingredients(recipe_id, name, measurement, price_measured_ingredient, item_id) values (@rid, @name, @measurement, @price_measured_ingredient, @item_id);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@price_measured_ingredient", i.priceOfMeasuredConsumption);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }
        public void UpdateIngredient(Ingredient i) {
            var commandText = "update ingredients set name=@name, measurement=@measurement, recipe_id=@recipeId, price_measured_ingredient=@price_measured_ingredient, item_id=@item_id where ing_id=@ingredientId;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@recipeId", i.recipeId);
                cmd.Parameters.AddWithValue("@ingredientId", i.ingredientId);
                cmd.Parameters.AddWithValue("@price_measured_ingredient", i.priceOfMeasuredConsumption);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }
        //maybe it would be worth to have a sql command that will update every ingredient table... while it'll be more work to maintain, i think that's a good idea, instead of calling each method, incase i forget one or something... or i can just have a method that calls all of the methods, and then call the overarhcing method....    
        public decimal MeasuredIngredientPrice(Ingredient i) {
            var convertWeight = new ConvertWeight();
            var convert = new ConvertMeasurement();
            var myCostData = queryCostTable();
            var myIngredients = queryIngredients();
            var myDensityData = queryDensityTable();
            var myConsumptionData = queryConsumptionTable();
            var temp = new Ingredient();
            var measuredIngredientPrice = 0m;
            foreach (var ingredient in myConsumptionData) {
                if (ingredient.name == i.name)
                    temp.ouncesConsumed = ingredient.ouncesConsumed;
            }
            foreach (var ingredient in myDensityData) {
                if (ingredient.name == i.name) {
                    temp.sellingPrice = ingredient.sellingPrice;
                    temp.density = ingredient.density;
                    temp.sellingWeightInOunces = ingredient.sellingWeightInOunces;
                }
            }
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name) {
                    ingredient.pricePerOunce = temp.pricePerOunce;
                    ingredient.ouncesConsumed = temp.ouncesConsumed;
                    ingredient.sellingPrice = temp.sellingPrice;
                    var accumulatedTeaspoons = convert.AccumulatedTeaspoonMeasurement(ingredient.measurement);
                    var measuredOuncesDividedBySellingWeight = 0m;
                    if (temp.sellingWeightInOunces != 0)
                        measuredOuncesDividedBySellingWeight = Math.Round((ingredient.ouncesConsumed / temp.sellingWeightInOunces), 4);
                    measuredIngredientPrice = Math.Round((measuredOuncesDividedBySellingWeight * temp.sellingPrice), 2);
                }
            }
            return measuredIngredientPrice;
        }
        public void insertIngredientIntoAllTables(Ingredient i, Recipe r) {
            var myRecipes = queryRecipes();
            var count = 0;
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id)
                    count++;
            }
            if (count == 0)
                InsertRecipe(r);
            insertIngredient(i, r);
            insertIngredientConsumtionData(i);
            insertIngredientDensityData(i);
            insertIngredientCostDataCostTable(i);
        }
        public void insertAllIngredientsIntoAllTables(List<Ingredient> ListOfIngredients, Recipe r) {
            var myRecipes = queryRecipes();
            var myIngredients = queryIngredients();
            var myListOfIngredientIds = new List<int>();
            var countRecipes = 0;
            var countIngredients = 0;
            foreach (var recipe in myRecipes) {
                if (recipe.id == r.id)
                    countRecipes++;
            }
            foreach (var ingredient in myIngredients) 
                countIngredients++;  
            if (countRecipes == 0)
                InsertRecipe(r);
            if (countIngredients == 0) {
                foreach (var ingredient in ListOfIngredients)
                    insertIngredientIntoAllTables(ingredient, r);
            } else {
                for (int i = 0; i < ListOfIngredients.Count(); i++)
                    myListOfIngredientIds.Add(ListOfIngredients[i].ingredientId);
                for (int i = 0; i < myIngredients.Count(); i++) {
                    if (!myListOfIngredientIds.Contains(myIngredients[i].ingredientId))
                        insertIngredientIntoAllTables(myIngredients[i], r);
                    //else updateAllTables(myIngredients[i], r);
                }
            }
            //foreach (var ingredient in ListOfIngredients) {
            //    if (!myIngredients.Contains(ingredient.ingredientId))
            //        insertIngredientIntoAllTables(ingredient, r);
            //    else
            //        updateAllTables(ingredient, r);
            //}
        }
        //for(int i = 0; i < myIngredients.Count(); i++) {
        //    if (.Contains(myListOfIngredientIds)
        //        updateAllTables(, r); 
        //    else 
        //}
        //foreach (var i in ListOfIngredients) {
        //    if (myIngredients.Contains(i))
        //        updateAllTables(i, r);
        //    else insertIngredientIntoAllTables(i, r);
        //need to put a trap here... if the ingredientId is already in the database, then just update the database, don't insert it

        //insertIngredient(i, r);
        //insertIngredientConsumtionData(i);
        //insertIngredientDensityData(i);
        //insertIngredientCostDataCostTable(i);
        //    }
        //}
        public void getIngredientMeasuredPrice(Ingredient i, Recipe r) {
            queryAllTablesForIngredient(i);
            i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
            UpdateIngredient(i);
        }
        public void updateAllTables(Ingredient i, Recipe r) {
            UpdateRecipe(r);
            UpdateIngredient(i); //this has the recipe id as well as the ingredient id, so unless these are missing or are not a part of the updateIngredient method (which they are), then we're good here
            updateDensityTable(i);
            updateConsumptionTable(i);
            updateCostDataTable(i);
        }
        //densities table methods: 
        public List<Ingredient> queryDensityTable() {
            var ingredientInformation = queryItems("select * from densities", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["ing_id"];
                ingredient.itemId = (int)reader["item_id"];
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
            var rest = new MakeRESTCalls();
            i.sellingPrice = rest.GetItemResponsePrice(i);
            i.sellingWeightInOunces = convert.ConvertWeightToOunces(i.sellingWeight);
            i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
            var commandText = @"Insert into densities (name, density, selling_weight, selling_weight_ounces, selling_price, price_per_ounce, item_id) 
                            values (@name, @density, @selling_weight, @selling_weight_ounces, @selling_price, @price_per_ounce, @item_id);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }
        public void updateDensityTable(Ingredient i) {
            var commandText = "update densities set name=@name, density=@density, selling_weight=@selling_weight, selling_weight_ounces=@selling_weight_ounces, selling_price=@selling_price, price_per_ounce=@price_per_ounce, item_id=@item_id where ing_id=@ing_id";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }

        //consumption table methods: 
        public List<Ingredient> queryConsumptionTable() {
            var ingredientInformation = queryItems("select * from consumption", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["ing_id"];
                ingredient.density = (decimal)reader["density"];
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                ingredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                return ingredient;
            });
            return ingredientInformation;
        }
        public void insertIngredientConsumtionData(Ingredient i) {
            var convert = new ConvertDensity();
            var myIngredients = queryIngredients();
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name)
                    i.measurement = ingredient.measurement;
            }
            i.ouncesConsumed = convert.CalculateOuncesUsed(i);
            var commandText = @"Insert into consumption (name, density, ounces_consumed, ounces_remaining) values (@name, @density, @ounces_consumed, @ounces_remaining);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@ounces_remaining", CalculateOuncesRemaining(i));
                return cmd;
            });
        }
        public void updateConsumptionTable(Ingredient i) {
            var convert = new ConvertDensity();
            var commandText = "update consumption set name=@name, density=@density, ounces_consumed=@ounces_consumed, ounces_remaining=@ounces_remaining where ing_id=@ing_id;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@ounces_consumed", convert.CalculateOuncesUsed(i));
                cmd.Parameters.AddWithValue("@ounces_remaining", CalculateOuncesRemaining(i));
                return cmd;
            });
        }
        public decimal CalculateOuncesConsumedFromMeasurement(Ingredient i) {
            var convert = new ConvertDensity();
            var myIngredientConsumptionData = queryConsumptionTable();
            var myIngredients = queryIngredients();
            var myConsumedOunces = 0m;
            var temp = new Ingredient();
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name) {
                    temp.measurement = ingredient.measurement;
                    myConsumedOunces = convert.CalculateOuncesUsed(i);
                }
            }
            return myConsumedOunces;
        }
        public decimal CalculateOuncesRemaining(Ingredient i) {
            var myIngredientConsumptionData = queryConsumptionTable();
            var ouncesRemaining = 0m;
            ouncesRemaining = i.ouncesRemaining - CalculateOuncesConsumedFromMeasurement(i);
            return ouncesRemaining;
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
            var commandText = @"Insert into costs (name, selling_weight, selling_price, price_per_ounce, item_id) values (@name, @selling_weight, @selling_price, @price_per_ounce, @item_id);";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }
        public void updateCostDataTable(Ingredient i) {
            var commandText = @"Update costs set name=@name, selling_weight=@selling_weight, selling_price=@selling_price, price_per_ounce=@price_per_ounce, item_id=@item_id where ing_id=@ing_id;";
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", getPricePerOunce(i));
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                //cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                return cmd;
            });
        }
        public decimal getPricePerOunce(Ingredient i) {
            var convert = new ConvertWeight();
            var myCostTableIngredients = queryCostTable();
            var pricePerOunce = 0m;
            foreach (var ingredient in myCostTableIngredients) {
                if (ingredient.name == i.name) {
                    i.sellingPrice = ingredient.sellingPrice;
                    i.sellingWeightInOunces = convert.ConvertWeightToOunces(ingredient.sellingWeight);
                    i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
                    pricePerOunce = i.pricePerOunce;
                }
            }
            return pricePerOunce;
        }

        //need to have a way to add more to ouncesRemaining

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
                        yield int,
                        aggregated_price decimal(5, 2)
                     );", a => a);

            dropTableIfExists("ingredients");
            executeVoidQuery(@"create table ingredients (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        recipe_id Int,
                        item_id int, 
                        name nvarchar(max), 
                        measurement nvarchar(max),
                        price_measured_ingredient decimal(6,2)
                     );", a => a);
            dropTableIfExists("densities");
            executeVoidQuery(@"create table densities (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
                        name nvarchar(max), 
                        item_id int,
                        density decimal (4,2),
                        selling_weight varchar(250),
                        selling_weight_ounces decimal(6,2),
                        selling_price decimal(6,2),
                        price_per_ounce decimal(8,4)
                     );", a => a);
            dropTableIfExists("consumption");
            executeVoidQuery(@"create table consumption (
                        ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
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
            executeVoidQuery("SET IDENTITY_INSERT densities ON", cmd => cmd);
            //this column was originally in the cost table, but i think it would be better in the ingredients table
            //price_measured_ingredient decimal(6, 2)
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it
//i'm not a big fan of having that price_meausured_ingredient in the cost database... i think that should belong inthe ingredient database