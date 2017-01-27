using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessConsumption {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public void dropConsumptionTableIfExists(string table) {
            var db = new DatabaseAccess();
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void initializeRecipeTable() {
            var db = new DatabaseAccess();
            dropConsumptionTableIfExists("consumption");
            db.executeVoidQuery(@"create table consumption (
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
        }
        public decimal doubleAverageOuncesConsumed(Ingredient i) {
            var convert = new ConvertMeasurement();
            var ouncesConsumedTable = new DatabaseAccessConsumptionOuncesConsumed();
            var listOfIngredientOuncesConsumed = new List<decimal>();
            var myIngredientOuncesConsumedTable = ouncesConsumedTable.queryConsumptionOuncesConsumed();
            foreach (var ingredient in myIngredientOuncesConsumedTable) {
                if (ingredient.name == i.name)
                    listOfIngredientOuncesConsumed.Add(ingredient.ouncesConsumed);
            }
            var count = listOfIngredientOuncesConsumed.Count();
            //var count = 1;
            var aggregatedOuncesConsumed = 0m;
            foreach (var measurement in listOfIngredientOuncesConsumed)
                aggregatedOuncesConsumed += measurement;
            return Math.Round((aggregatedOuncesConsumed / count) * 2, 2);
        }
        public List<Ingredient> queryConsumptionTable() {
            var db = new DatabaseAccess();
            var ingredientInformation = db.queryItems("select * from consumption", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["id"];
                ingredient.density = (decimal)reader["density"];
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                ingredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                ingredient.restock = (int)reader["refill"];
                return ingredient;
            });
            return ingredientInformation;
        }
        public void insertIngredientConsumtionData(Ingredient i) {
            var db = new DatabaseAccess();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredient = db.queryAllTablesForIngredient(i);
            var myConsumptionTable = queryConsumptionTable();
            var temp = new Ingredient();
            bool alreadyContainsIngredient = new bool();
            if (myIngredient.classification.ToLower().Contains("egg")) {
                temp.name = "eggs";
                i.ouncesConsumed = convertWeight.EggsConsumedFromIngredientMeasurement(myIngredient.measurement);
            } else i.ouncesConsumed = dbConsumptionOuncesConsumed.CalculateOuncesConsumedFromMeasurement(i);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower() || (ingredient.name.ToLower().Contains(i.classification.ToLower()) && i.classification != " ")) {
                    alreadyContainsIngredient = true;
                    break;
                }
            }
            if (string.IsNullOrEmpty(temp.name))
                temp.name = i.name;
            if (alreadyContainsIngredient == false) {
                var commandText = @"Insert into consumption (name, density, ounces_consumed, ounces_remaining) values (@name, @density, @ounces_consumed, @ounces_remaining);";
                db.executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@name", temp.name);
                    cmd.Parameters.AddWithValue("@density", i.density);
                    cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                    //when the time comes, i want to change any negative ouncesRemaining to be 0 so i can start fresh when i refill the ingredient in my consumption table
                    //although, it would be nice to say "you need 2 tablespoons more granulated sugar to make this recipe"... maybe if you refill, then put at 0 first, if not, then leave negative?
                    cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                    return cmd;
                });
                updateConsumptionTable(i);
            } else updateConsumptionTable(i);
            var myUpdatedIngredient = queryConsumptionTable();
        }
        public void updateConsumptionTable(Ingredient i) {
            var db = new DatabaseAccess();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var convert = new ConvertWeight();
            var myIngredient = db.queryAllTablesForIngredient(i);
            var myConsumptionTable = queryConsumptionTable();
            var temp = new Ingredient();
            foreach (var ingredient in myConsumptionTable) {
                if (i.classification.ToLower().Contains("egg") && ingredient.name.ToLower().Contains("egg")) {
                    temp.name = ingredient.name;
                    var currentOuncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                    if (ingredient.ouncesConsumed != currentOuncesConsumed)
                        i.ouncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                    if (ingredient.ouncesRemaining == 0m)
                        i.ouncesRemaining = i.sellingWeightInOunces - i.ouncesConsumed;
                    else i.ouncesRemaining = ingredient.ouncesRemaining - i.ouncesConsumed;
                    break;
                } else {
                    if (ingredient.name.ToLower() == i.name.ToLower()) {
                        ingredient.ouncesConsumed = dbConsumptionOuncesConsumed.CalculateOuncesConsumedFromMeasurement(i);
                        i.ouncesConsumed = ingredient.ouncesConsumed;
                        dbConsumptionOuncesConsumed.insertIngredientIntoConsumptionOuncesConsumed(myIngredient);
                        if (ingredient.ouncesRemaining == 0m) {
                            myIngredient.ouncesRemaining = myIngredient.sellingWeightInOunces - ingredient.ouncesConsumed;
                        } else
                            myIngredient.ouncesRemaining = ingredient.ouncesRemaining - ingredient.ouncesConsumed;
                        i.ouncesRemaining = myIngredient.ouncesRemaining;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(temp.name))
                temp.name = i.name;
            //subtractOuncesRemainingIfExpirationDateIsPast(i);
            //this is my problem, because i don't have an expiration date, it's saying that my ingredient is out of date, with 01/01/0001
            var commandText = "update consumption set ounces_consumed=@ounces_consumed, ounces_remaining=@ounces_remaining where name=@name;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", temp.name);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                return cmd;
            });
            i.ouncesRemaining = getOuncesRemainingFromConsumptionTableFromIngredient(i);
            if (doesIngredientNeedRestocking(i))
                i.restock = 0;
            else i.restock = 1;
            var refillCommandText = "update consumption set refill=@refill where name=@name;";
            db.executeVoidQuery(refillCommandText, cmd => {
                cmd.Parameters.AddWithValue("@refill", i.restock);
                cmd.Parameters.AddWithValue("@name", i.name);
                return cmd;
            });

            var myUpdatedIngredient = queryConsumptionTable();
            var myUpdatedConsumptionOuncesConsumedTable = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed();
        }
        public void subtractOuncesRemainingIfExpirationDateIsPast(Ingredient i) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var convert = new ConvertWeight();
            var myIngredient = db.queryAllTablesForIngredient(i);
            if (i.expirationDate < DateTime.Today && (dbIngredients.convertDateToStringMMDDYYYY(i.expirationDate) != "01/01/0001")) {
                //i.expirationDate != new DateTime()) {
                myIngredient.ouncesRemaining = myIngredient.ouncesRemaining - i.sellingWeightInOunces;
                if (myIngredient.ouncesRemaining < 0m)
                    myIngredient.ouncesRemaining = 0m;
                var commandText = @"update consumption set ounces_remaining=@ounces_remaining where name=@name";
                db.executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@name", myIngredient.name);
                    cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                    return cmd;
                });
            }
            var myUpdatedIngredient = queryConsumptionTable();
        }
        public void refillIngredientInConsumptionDatabase(Ingredient i, string sellingWeightToRefill) {
            var db = new DatabaseAccess();
            var convert = new ConvertWeight();
            var myConsumptionTable = queryConsumptionTable();
            var sellingWeightToRefillInOunces = convert.ConvertWeightToOunces(sellingWeightToRefill);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower()) {
                    if (i.ouncesRemaining < 0m)
                        i.ouncesRemaining = 0m;
                    i.ouncesRemaining = ingredient.ouncesRemaining + sellingWeightToRefillInOunces;
                    break;
                }
            }
            var commandText = "update consumption set ounces_remaining=@ounces_remaining where name=@name;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                return cmd;
            });
        }
        public void refillIngredientInConsumptionDatabase(Ingredient i, string sellingWeightToRefill, string newExpirationDate) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var convert = new ConvertWeight();
            var myConsumptionTable = queryConsumptionTable();
            var myIngredientTable = dbIngredients.queryIngredients();
            var sellingWeightToRefillOunces = convert.ConvertWeightToOunces(sellingWeightToRefill);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower()) {
                    if (i.ouncesRemaining < 0m)
                        i.ouncesRemaining = 0m;
                    i.ouncesRemaining = ingredient.ouncesRemaining + sellingWeightToRefillOunces;
                    var commandText = "update consumption set ounces_remaining=@ounces_remaining where name=@name;";
                    db.executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@name", i.name);
                        cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                        return cmd;
                    });
                    break;
                }
            }
            foreach (var ingredient in myIngredientTable) {
                if (ingredient.ingredientId == i.ingredientId && ingredient.name.ToLower() == i.name.ToLower()) {
                    ingredient.expirationDate = dbIngredients.convertStringToDateYYYYMMDD(newExpirationDate);
                    var commandText = "update ingredients set expiration_date=@expiration_date where ing_id=@ing_id";
                    db.executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@expiration_date", dbIngredients.convertDateToStringMMDDYYYY(ingredient.expirationDate));
                        cmd.Parameters.AddWithValue("@ing_id", ingredient.ingredientId);
                        return cmd;
                    });
                    break;
                }
            }
        }
        public decimal getOuncesRemainingFromConsumptionTableFromIngredient(Ingredient i) {
            var consumptionTable = queryConsumptionTable();
            foreach (var ingredient in consumptionTable) {
                if (ingredient.name == i.name) {
                    i.ouncesRemaining = ingredient.ouncesRemaining;
                    break;
                }
            }
            return i.ouncesRemaining;
        }
        public bool doesIngredientNeedRestocking(Ingredient i) {
            var ingredientOuncesRemaining = getOuncesRemainingFromConsumptionTableFromIngredient(i);
            var doubleOunces = doubleAverageOuncesConsumed(i);
            return ingredientOuncesRemaining <= doubleOunces ? true : false;
        }
        public void DeleteIngredientFromConsumptionTable(Ingredient i) {
            var db = new DatabaseAccess();
            var deleteCommand = "delete from consumption where name=@name";
            db.executeVoidQuery(deleteCommand, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                return cmd;
            });
        }
    public List<string> getListOfDistinctSellingWeights() {
            var dbD = new DatabaseAccessDensities(); 
        var myDensiitiesTable = dbD.queryDensitiesTable();
        var myUniqueSellingWeights = new List<string>();
        foreach (var ingredient in myDensiitiesTable) {
            if (!myUniqueSellingWeights.Contains(ingredient.sellingWeight))
                myUniqueSellingWeights.Add(ingredient.sellingWeight);
        }
        myUniqueSellingWeights.Sort();
        return myUniqueSellingWeights;
    }
    }
}