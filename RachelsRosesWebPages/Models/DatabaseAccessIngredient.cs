using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessIngredient {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public void dropIfIngredientsTableExists(string table) {
            var db = new DatabaseAccess();
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void initializeIngredientsTable
            () {
            var db = new DatabaseAccess();
            dropIfIngredientsTableExists("ingredients");
            db.executeVoidQuery(@"create table ingredients (
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
        }
        public ItemResponse returnItemResponse(Ingredient i) {
            var rest = new MakeRESTCalls();
            return rest.GetItemResponse(i);
        }
        public static ItemResponse myItemResponse = new ItemResponse();
        public void DeleteIngredientFromIngredientTable(Ingredient i) {
            var db = new DatabaseAccess();
            i.name = i.name.Trim();
            i.measurement = i.measurement.Trim();
            var delete = "delete from ingredients where name=@name AND measurement=@measurement;";
            db.executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                return cmd;
            });
        }

        public void DeleteIngredientFromIngredientTableIngIds(Ingredient i) {
            var db = new DatabaseAccess();
            i.name = i.name.Trim();
            i.measurement = i.measurement.Trim();
            var delete = "delete from ingredients where name=@name AND ing_id=@ing_id";
            db.executeVoidQuery(delete, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                return cmd;
            });
        }
        public List<Ingredient> queryAllIngredientsFromIngredientTable() {
            var db = new DatabaseAccess();
            var count = 1;
            var myIngredientBox = db.queryItems("select * from ingredients", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.measurement = (string)reader["measurement"];
                ingredient.recipeId = (int)reader["recipe_id"];
                ingredient.priceOfMeasuredConsumption = (decimal)reader["price_measured_ingredient"];
                ingredient.itemId = (int)reader["item_id"];
                ingredient.typeOfIngredient = (string)reader["ingredient_type"];
                ingredient.classification = (string)reader["ingredient_classification"];
                ingredient.expirationDate = convertStringMMDDYYYYToDateYYYYMMDD((string)reader["expiration_date"]);
                return ingredient;
            });
            foreach (var ingredient in myIngredientBox)
                ingredient.ingredientId = count++;
            return myIngredientBox;
        }
        public void insertIngredient(Ingredient i, Recipe r) {
            var db = new DatabaseAccess();
            if ((i.sellingPrice == 0m && !i.classification.ToLower().Contains("dairy")) || (i.sellingPrice == 0m && !i.classification.ToLower().Contains("eggs"))) {
                //    if (i.sellingPrice == 0m && (!i.classification.ToLower().Contains("dairy")) || (!i.classification.ToLower().Contains("egg"))) {
                myItemResponse = returnItemResponse(i);
                if (i.itemId == 0)
                    i.itemId = myItemResponse.itemId;
                if (string.IsNullOrEmpty(i.itemResponseName))
                    i.itemResponseName = myItemResponse.name;
                if (i.sellingPrice == 0m)
                    i.sellingPrice = myItemResponse.salePrice;
            }
            if ((i.classification.ToLower().Contains("dairy")) || (i.classification.ToLower().Contains("egg")))
                i.itemResponseName = " ";
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            if (i.expirationDate == null)
                i.expirationDate = new DateTime();
            var expirationDateString = convertDateToStringMMDDYYYY(i.expirationDate);
            if ((i.classification.ToLower() == "dairy" || i.classification.ToLower() == "egg" || i.classification.ToLower() == "eggs") && expirationDateString == "01/01/0000")
                throw new Exception("Please enter an expiration date for dairy and egg items");
            var commandText = @"Insert into ingredients(recipe_id, name, measurement, price_measured_ingredient, item_id, ingredient_type, ingredient_classification, item_response_name, expiration_date) 
                                values (@rid, @name, @measurement, @price_measured_ingredient, @item_id, @ingredient_type, @ingredient_classification, @item_response_name, @expiration_date);";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@rid", r.id);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@price_measured_ingredient", i.priceOfMeasuredConsumption);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                cmd.Parameters.AddWithValue("@ingredient_type", i.typeOfIngredient);
                cmd.Parameters.AddWithValue("@ingredient_classification", i.classification);
                cmd.Parameters.AddWithValue("@item_response_name", i.itemResponseName);
                cmd.Parameters.AddWithValue("@expiration_date", convertDateToStringMMDDYYYY(i.expirationDate));
                return cmd;
            });
            //var myIngredients = queryAllIngredientsFromIngredientTable();
            //var myIngredientFull = db.queryAllRelevantTablesSQL(i);
        }
        public void UpdateIngredient(Ingredient i) {
            var db = new DatabaseAccess();
            var rest = new MakeRESTCalls();
            if (i.sellingPrice == 0m)
                myItemResponse = rest.GetItemResponse(i);
            if (i.sellingPrice == 0m && (!i.classification.ToLower().Contains("dairy")) || (!i.classification.ToLower().Contains("egg"))) {
                if (i.itemId == 0) {
                    myItemResponse = returnItemResponse(i);
                    i.itemId = myItemResponse.itemId;
                }
                if (string.IsNullOrEmpty(i.itemResponseName))
                    i.itemResponseName = myItemResponse.name;
                if (i.sellingPrice == 0m)
                    i.sellingPrice = myItemResponse.salePrice;
            }
            if ((i.classification.ToLower().Contains("dairy")) || (i.classification.ToLower().Contains("egg"))) {
                i.itemResponseName = " ";
                if (string.IsNullOrEmpty(i.classification))
                    i.classification = " ";
                if (i.expirationDate == null)
                    i.expirationDate = new DateTime();
            }
            if (i.priceOfMeasuredConsumption == 0)
                i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            var myIngredientId = i.ingredientId;
            var commandText = @"update ingredients set name=@name, 
                                measurement=@measurement,
                                recipe_id=@recipeId, 
                                price_measured_ingredient=@price_measured_ingredient, 
                                item_id=@item_id, 
                                ingredient_type=@ingredient_type, 
                                ingredient_classification=@ingredient_classification, 
                                item_response_name=@item_response_name, 
                                expiration_date=@expiration_date 
                                where ing_id=@ing_id AND name=@name;";
            db.executeVoidQuery(commandText, cmd => {
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
                return cmd;
            });
        }
        public decimal MeasuredIngredientPrice(Ingredient i) {
            var db = new DatabaseAccess();
            var myIngredient = db.queryAllRelevantTablesSQLByIngredientName(i);
            var measuredOuncesDividedBySellingWeight = Math.Round((myIngredient.ouncesConsumed / myIngredient.sellingWeightInOunces), 4);
            var measuredIngredientPrice = Math.Round((measuredOuncesDividedBySellingWeight * myIngredient.sellingPrice), 2);
            return measuredIngredientPrice;
        }

        public List<string> myDistinctIngredientNamesSorted() {
            var db = new DatabaseAccess();
            var uniqueIngredientNames = new List<string>();
            var orderIngredientsByName = @"SELECT DISTINCT name
                                           FROM ingredients
                                           ORDER BY name ASC;";
            db.queryItems(orderIngredientsByName, reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                if (!uniqueIngredientNames.Contains(ingredient.name))
                    uniqueIngredientNames.Add(ingredient.name);
                return ingredient;
            });
            return uniqueIngredientNames;
        }
        public decimal returnIngredientMeasuredPrice(Ingredient i) {
            var db = new DatabaseAccess();
            var myIngredient = db.queryAllRelevantTablesSQLByIngredientName(i);
            return MeasuredIngredientPrice(myIngredient);
        }
        public void getIngredientMeasuredPrice(Ingredient i, Recipe r) {
            var db = new DatabaseAccess();
            db.queryAllRelevantTablesSQLByIngredientName(i);
            i.priceOfMeasuredConsumption = MeasuredIngredientPrice(i);
            UpdateIngredient(i);
        }
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
        public DateTime convertStringMMDDYYYYToDateYYYYMMDD(string dateString) {
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
            if (int.Parse(day) < 10)
                day = "0" + day;
            var year = dateStringArray[2];
            return string.Format("{0}/{1}/{2}", month, day, year);
        }
        public DateTime getExpirationDateFromIngredientsTable(Ingredient i) {
            var myIngredients = queryAllIngredientsFromIngredientTable();
            var myIngredientExpirationDate = new DateTime();
            foreach (var ingredient in myIngredients) {
                if (ingredient.ingredientId == i.ingredientId) {
                    (ingredient.expirationDate) = myIngredientExpirationDate;
                    break;
                }
            }
            return myIngredientExpirationDate;
        }
        public List<Ingredient> myIngredientBox() {
            var db = new DatabaseAccess();
            var ingredientBox = new List<Ingredient>();
            var queriedIngredients = queryAllIngredientsFromIngredientTable();
            foreach (var ingredient in queriedIngredients)
                ingredientBox.Add(db.queryAllRelevantTablesSQLByIngredientName(ingredient));
            return ingredientBox;
        }
        public List<string> myDistinctIngredientClassificationsSorted() {
            var db = new DatabaseAccess();
            var distinctClassifications = new List<string>();
            var commandTextDistinctClassifications = @"SELECT DISTINCT ingredient_classification 
                                                        FROM ingredients
                                                        ORDER BY ingredient_classification ASC";
            db.queryItems(commandTextDistinctClassifications, reader => {
                string classification = (string)(reader["ingredient_classification"]);
                //if (!distinctClassifications.Contains(classification))
                distinctClassifications.Add(classification);
                return classification;
            });
            return distinctClassifications;

        }
        public List<string> myDistinctIngredientTypesSorted() {
            var db = new DatabaseAccess();
            var distinctTypes = new List<string>();
            var commandTextDistinctTypes = @"SELECT DISTINCT ingredient_type 
                                            FROM ingredients
                                            ORDER BY ingredient_type ASC;";
            db.queryItems(commandTextDistinctTypes, reader => {
                string type = (string)(reader["ingredient_type"]);
                //if (!commandTextDistinctTypes.Contains(type))
                distinctTypes.Add(type);
                return type;
            });
            return distinctTypes;
        }
        public List<Ingredient> orderIngredientsByPricePerOunce() {
            var db = new DatabaseAccess();
            var orderedIngredients = new List<Ingredient>();
            var commandTextOrderIngredientsPricePerOunce = @"SELECT DISTINCT ingredients.name, price_per_ounce
                                                FROM ingredients
                                                JOIN costs
                                                ON ingredients.name=costs.name
                                                ORDER BY costs.price_per_ounce DESC;";
            db.queryItems(commandTextOrderIngredientsPricePerOunce, reader => {
                var ingredient = new Ingredient((string)(reader["name"]));
                ingredient.pricePerOunce = (decimal)(reader["price_per_ounce"]);
                orderedIngredients.Add(ingredient);
                return ingredient;
            });
            return orderedIngredients;
        }
        public List<Ingredient> orderIngredientsByExpirationDateAsc() {
            var db = new DatabaseAccess();
            var ExpiringIngredients = new List<Ingredient>();
            var commandTextOrderIngredientsByExpirationDate = @"SELECT name, expiration_date 
                                                                FROM ingredients
                                                                ORDER BY expiration_date ASC";
            db.queryItems(commandTextOrderIngredientsByExpirationDate, reader => {
                var ingredient = new Ingredient((string)(reader["name"]));
                var expirationDate = (string)(reader["expiration_date"]);
                ingredient.expirationDate = convertStringMMDDYYYYToDateYYYYMMDD(expirationDate);
                //ingredient.expirationDate = (DateTime)(reader["expiration_date"]);
                ExpiringIngredients.Add(ingredient);
                return ingredient;
            });
            return ExpiringIngredients;
        }
        public Ingredient queryIngredientFromIngredientsTableByName(Ingredient i) {
            var db = new DatabaseAccess();
            var queriedIngredient = new Ingredient();
            var commandTextQueryIngredient = string.Format(@"SELECT * FROM ingredients where name='{0}' AND ing_id={1};", i.name, i.ingredientId);
            db.queryItems(commandTextQueryIngredient, reader => {
                queriedIngredient.name = (string)(reader["name"]);
                queriedIngredient.measurement = (string)(reader["measurement"]);
                queriedIngredient.recipeId = (int)(reader["recipe_id"]);
                queriedIngredient.ingredientId = (int)(reader["ing_id"]);
                queriedIngredient.classification = (string)(reader["ingredient_classification"]);
                queriedIngredient.typeOfIngredient = (string)(reader["ingredient_type"]);
                queriedIngredient.priceOfMeasuredConsumption = (decimal)(reader["price_measured_ingredient"]);
                queriedIngredient.itemResponseName = (string)(reader["item_response_name"]);
                var expirationDate = (string)(reader["expiration_date"]);
                queriedIngredient.expirationDate = convertStringMMDDYYYYToDateYYYYMMDD(expirationDate);
                return queriedIngredient;
            });
            return queriedIngredient;
        }
        public List<Ingredient> queryIngredientIdsAndNamesFromIngredientTable() {
            var db = new DatabaseAccess();
            //var dict = new Dictionary<int, string>();
            var listOfIngredients = new List<Ingredient>();
            var commandText = @"SELECT ing_id, name FROM ingredients;";
            listOfIngredients = db.queryItems(commandText, reader => {
                var ingredient = new Ingredient((string)reader["name"]);
                ingredient.ingredientId = (int)reader["ing_id"];
                return ingredient;
            });
            return listOfIngredients;
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it