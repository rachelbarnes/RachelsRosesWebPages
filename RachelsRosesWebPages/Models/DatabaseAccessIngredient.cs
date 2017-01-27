using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessIngredient{
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
        public List<Ingredient> queryIngredients() {
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
                ingredient.expirationDate = convertStringToDateYYYYMMDD((string)reader["expiration_date"]);
                return ingredient;
            });
            foreach (var ingredient in myIngredientBox)
                ingredient.ingredientId = count++;
            return myIngredientBox;
        }
        public void insertIngredient(Ingredient i, Recipe r) {
            var db = new DatabaseAccess(); 
            if (i.sellingPrice == 0m && (!i.classification.ToLower().Contains("dairy")) || (!i.classification.ToLower().Contains("egg"))) {
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
            var commandText = "Insert into ingredients(recipe_id, name, measurement, price_measured_ingredient, item_id, ingredient_type, ingredient_classification, item_response_name, expiration_date) values (@rid, @name, @measurement, @price_measured_ingredient, @item_id, @ingredient_type, @ingredient_classification, @item_response_name, @expiration_date);";
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
            var myIngredients = queryIngredients();
            var myIngredientFull = db.queryAllTablesForIngredient(i);
        }
        public void UpdateIngredient(Ingredient i) {
            var db = new DatabaseAccess(); 
            var myIngredients = queryIngredients();
            if (i.sellingPrice == 0m && (!i.classification.ToLower().Contains("dairy")) || (!i.classification.ToLower().Contains("egg"))) {
                myItemResponse = returnItemResponse(i);
                if (i.itemId == 0)
                    i.itemId = myItemResponse.itemId;
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
            } else {
                myItemResponse = returnItemResponse(i);
                if (i.itemId == 0)
                    i.itemId = myItemResponse.itemId;
                if (string.IsNullOrEmpty(i.itemResponseName))
                    i.itemResponseName = myItemResponse.name;
                if (i.priceOfMeasuredConsumption == 0)
                    i.priceOfMeasuredConsumption = returnIngredientMeasuredPrice(i);
            }
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            var myIngredientId = i.ingredientId;
            var commandText = "update ingredients set name=@name, measurement=@measurement, recipe_id=@recipeId, price_measured_ingredient=@price_measured_ingredient, item_id=@item_id, ingredient_type=@ingredient_type, ingredient_classification=@ingredient_classification, item_response_name=@item_response_name, expiration_date=@expiration_date where ing_id=@ing_id;";
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
                //this convertDateToString formats the string in MM/DD/YYYY
                return cmd;
            });
        }
        public decimal MeasuredIngredientPrice(Ingredient i) {
            var dbRecipes = new DatabaseAccessRecipe();
            var dbIngredients = new DatabaseAccessIngredient();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var dbConsumption = new DatabaseAccessConsumption();
            var dbDensities = new DatabaseAccessDensities();
            var dbDensitiesInformation = new DatabaseAccessDensityInformation(); 
            var dbCosts = new DatabaseAccessCosts(); 
            var convertWeight = new ConvertWeight();
            var convert = new ConvertMeasurement();
            var myCostData = dbCosts.queryCostTable();
            var myIngredients = queryIngredients();
            var myDensityData = dbDensities.queryDensitiesTable();
            var myConsumptionData = dbConsumption.queryConsumptionTable();
            var temp = new Ingredient();
            var measuredIngredientPrice = 0m;
            foreach (var ingredient in myConsumptionData) {
                if (ingredient.name.ToLower() == i.name.ToLower() || (ingredient.name.ToLower().Contains(i.classification.ToLower()) && i.classification != " ")) {
                    temp.ouncesConsumed = ingredient.ouncesConsumed;
                    break;
                }
            }
            foreach (var ingredient in myDensityData) {
                if (ingredient.name == i.name) {
                    temp.density = ingredient.density;
                    temp.sellingWeightInOunces = ingredient.sellingWeightInOunces;
                    break;
                }
            }
            foreach (var ingredient in myCostData) {
                if (ingredient.ingredientId == i.ingredientId) {
                    temp.sellingPrice = ingredient.sellingPrice;
                    temp.pricePerOunce = ingredient.pricePerOunce;
                    break;
                }
            }
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name) {
                    ingredient.ouncesConsumed = temp.ouncesConsumed;
                    ingredient.sellingPrice = temp.sellingPrice;
                    //var accumulatedTeaspoons = convert.AccumulatedTeaspoonMeasurement(ingredient.measurement);
                    var measuredOuncesDividedBySellingWeight = 0m;
                    if (temp.sellingWeightInOunces != 0)
                        measuredOuncesDividedBySellingWeight = Math.Round((ingredient.ouncesConsumed / temp.sellingWeightInOunces), 4);
                    measuredIngredientPrice = Math.Round((measuredOuncesDividedBySellingWeight * temp.sellingPrice), 2);
                    break;
                }
            }
            return measuredIngredientPrice;
        }
        public List<Ingredient> getListOfDistintIngredients() {
            var db = new DatabaseAccess(); 
            var myIngredientsTable = queryIngredients();
            var myUniqueIngredientNames = new List<string>();
            var myUniqueIngredients = new List<Ingredient>();
            foreach (var ingredient in myIngredientsTable) {
                if (!myUniqueIngredientNames.Contains(ingredient.name)) {
                    myUniqueIngredientNames.Add(ingredient.name);
                    myUniqueIngredients.Add(db.queryAllTablesForIngredient(ingredient));
                }
            }
            //myUniqueIngredients.Sort(); 
            return myUniqueIngredients;
        }
        public decimal returnIngredientMeasuredPrice(Ingredient i) {
            var db = new DatabaseAccess(); 
            db.queryAllTablesForIngredient(i);
            return MeasuredIngredientPrice(i);
        }
        public void getIngredientMeasuredPrice(Ingredient i, Recipe r) {
            var db = new DatabaseAccess(); 
            db.queryAllTablesForIngredient(i);
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
        public DateTime convertStringToDateYYYYMMDD(string dateString) {
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
            var myIngredients = queryIngredients();
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
            var queriedIngredients = queryIngredients();
            foreach (var ingredient in queriedIngredients)
                ingredientBox.Add(db.queryAllTablesForIngredient(ingredient));
            return ingredientBox;
        }
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it