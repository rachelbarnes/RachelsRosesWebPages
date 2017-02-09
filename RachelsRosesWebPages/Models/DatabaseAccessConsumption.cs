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
                        density decimal(4,2), 
                        ounces_consumed decimal(5,2),
                        ounces_remaining decimal(5,2),
                        measurement nvarchar(250),
                        refill int default 0
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
            if (count > 0) {
                var aggregatedOuncesConsumed = 0m;
                foreach (var measurement in listOfIngredientOuncesConsumed)
                    aggregatedOuncesConsumed += measurement;
                return Math.Round((aggregatedOuncesConsumed / count) * 2, 2);
            } else return 0m;
        }
        public List<Ingredient> queryConsumptionTable() {
            var db = new DatabaseAccess();
            //var ingredientInformation = db.queryItems("select * from consumption order by name asc", reader => {
            var commandText = @"SELECT * FROM consumption;";
            //ORDER BY consumption.name ASC;";
            var ingredientInformation = db.queryItems(commandText, reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["id"];
                ingredient.density = (decimal)reader["density"];
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                ingredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                ingredient.restock = (int)reader["refill"];
                ingredient.measurement = (string)reader["measurement"];
                return ingredient;
            });
            return ingredientInformation;
        }
        public void insertIngredientConsumtionData(Ingredient i) {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredientIngredientTable = dbI.queryIngredientFromIngredientsTableByName(i);
            var myConsumptionTable = queryConsumptionTable();
            //i need to make sure i have all my information from my sql tables... that's why im getting null reference exceptions... 
            var temp = new Ingredient();
            bool alreadyContainsIngredient = new bool();
            if (myIngredientIngredientTable.classification.ToLower().Contains("egg")) {
                temp.name = "Egg";
                //i would prefer this to be eggs, but i am matching the ingredient_classification if the ingredient.name doesn't match for querying the ingredients table and the consumption table, and the classifications are singular... 
                //i'm going to have to put a warning or something in the READ ME asking the user not to change the name of the consumption table ignredients... i'm not a big fan of that. I want there to be flexibility for what the user needs
                i.ouncesConsumed = convertWeight.EggsConsumedFromIngredientMeasurement(myIngredientIngredientTable.measurement);
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
                var commandText = @"Insert into consumption (name, density, ounces_consumed, ounces_remaining, measurement) values (@name, @density, @ounces_consumed, @ounces_remaining, @measurement);";
                db.executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@name", temp.name);
                    cmd.Parameters.AddWithValue("@density", i.density);
                    cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                    cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                    cmd.Parameters.AddWithValue("@measurement", i.measurement);
                    return cmd;
                });
                updateConsumptionTable(i);
            } else updateConsumptionTable(i);
            var myUpdatedIngredient = queryConsumptionTable();
            var myConsumptionOuncesConsumedTable = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed();
        }

        public void updateConsumptionTable(Ingredient i) {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var convert = new ConvertWeight();
            var dbD = new DatabaseAccessDensities();
            var myIngredient = dbI.queryIngredientFromIngredientsTableByName(i);
            var myConsumptionTableIngredient = queryConsumptionTableRowByName(i);
            var myDensityTableIngredient = dbD.queryIngredientFromDensityTableByName(i);
            //var myDensityTableIngredient = dbD.queryIngredientFromDensityTableByName(i); 
            var temp = new Ingredient();
            //this handles egg classifications, calculates ounces consumed and ounces remaining
            if (myIngredient.classification.ToLower().Contains("egg")) {
                var currentOuncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                if (myConsumptionTableIngredient.ouncesConsumed != currentOuncesConsumed)
                    i.ouncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                if (myConsumptionTableIngredient.ouncesRemaining == 0m)
                    i.ouncesRemaining = i.sellingWeightInOunces - i.ouncesConsumed;
                else i.ouncesRemaining = myConsumptionTableIngredient.ouncesRemaining - i.ouncesConsumed;
            }
            //this handles other ingredients; eggs have to be calculated by usage of egg, not by an actual measurement
             else {
                //if (i.ouncesConsumed == 0m)
                myConsumptionTableIngredient.ouncesConsumed = dbConsumptionOuncesConsumed.CalculateOuncesConsumedFromMeasurement(i);
                i.ouncesConsumed = myConsumptionTableIngredient.ouncesConsumed;
                if (myConsumptionTableIngredient.ouncesRemaining == 0m) {
                    myConsumptionTableIngredient.ouncesRemaining = myDensityTableIngredient.sellingWeightInOunces - myConsumptionTableIngredient.ouncesConsumed;
                } else
                    myConsumptionTableIngredient.ouncesRemaining = myConsumptionTableIngredient.ouncesRemaining - myConsumptionTableIngredient.ouncesConsumed;
                i.ouncesRemaining = myConsumptionTableIngredient.ouncesRemaining;
            }

            //if (string.IsNullOrEmpty(temp.name) && !(i.classification.ToLower().Contains("egg")))
            if (i.classification.ToLower().Contains("egg"))
                i.name = "Egg";
            //temp.name = i.name;
            //subtractOuncesRemainingIfExpirationDateIsPast(i);
            // this needs to be fixed, maybe for hte moment having a condition for ig it is eggs or dairy... flour and sugar, etc. should be totally fine
            var commandText = "update consumption set ounces_consumed=@ounces_consumed, ounces_remaining=@ounces_remaining, refill=@refill where name=@name;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                cmd.Parameters.AddWithValue("@refill", i.restock);
                return cmd;
            });
            doesIngredientNeedRestocking(i);
            //make sure that hte name for Eggs and something like "egg whites, stiffy beaten" match through the classification for the ounces_consumed...
            //dbConsumptionOuncesConsumed.insertIngredientIntoConsumptionOuncesConsumed(i);
            //still not getting the ouncesRemaining... need to change this
            var consumptionOuncesConsumed = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed();
            //why am i not inserting this into the database? 
            var myUpdatedIngredient = queryConsumptionTableRowByName(i);
            var myUpdatedConsumptionOuncesConsumedTable = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed();
        }
        public void subtractOuncesRemainingIfExpirationDateIsPast(Ingredient i) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var convert = new ConvertWeight();
            var myIngredient = db.queryAllRelevantTablesSQLByIngredientName(i);
            if (i.expirationDate < DateTime.Today && (dbIngredients.convertDateToStringMMDDYYYY(i.expirationDate) != "01/01/0001")) {
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
            var myIngredientTable = dbIngredients.queryAllIngredientsFromIngredientTable();
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
                    ingredient.expirationDate = dbIngredients.convertStringMMDDYYYYToDateYYYYMMDD(newExpirationDate);
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
        //the reason why this is void and access the database apart from the method that calls this (UpdateConsumptionTable) is the ouncesRemaining aren't set in the consumption table yet, i would get incorrect data
        public void doesIngredientNeedRestocking(Ingredient i) {
            var db = new DatabaseAccess();
            var consumptionTableIngredientRow = queryConsumptionTableRowByName(i);
            //var ingredientOuncesRemaining = getOuncesRemainingFromConsumptionTableFromIngredient(i);
            var doubleOunces = doubleAverageOuncesConsumed(i);
            var intBool = consumptionTableIngredientRow.ouncesRemaining <= doubleOunces ? 1 : 0;
            if (consumptionTableIngredientRow.restock != intBool) {
                var commandTextEnterRestockValue = @"UPDATE consumption set refill=@refill where name=@name";
                db.executeVoidQuery(commandTextEnterRestockValue, cmd => {
                    cmd.Parameters.AddWithValue("@refill", intBool);
                    cmd.Parameters.AddWithValue("@name", i.name);
                    return cmd;
                });
            }
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
            var myDensiitiesTable = dbD.queryDensitiesTableAllRows();
            var myUniqueSellingWeights = new List<string>();
            foreach (var ingredient in myDensiitiesTable) {
                if (!myUniqueSellingWeights.Contains(ingredient.sellingWeight))
                    myUniqueSellingWeights.Add(ingredient.sellingWeight);
            }
            myUniqueSellingWeights.Sort();
            return myUniqueSellingWeights;
        }
        public Ingredient queryConsumptionTableRowByName(Ingredient i) {
            var db = new DatabaseAccess();
            var consumptionTableRow = new Ingredient();
            var commandTextGetConsumptionRow = string.Format(@"SELECT * FROM consumption WHERE name='{0}';", i.name);
            db.queryItems(commandTextGetConsumptionRow, reader => {
                consumptionTableRow.name = (string)reader["name"];
                consumptionTableRow.ingredientId = (int)reader["id"];
                consumptionTableRow.density = (decimal)reader["density"];
                consumptionTableRow.ouncesConsumed = (decimal)reader["ounces_consumed"];
                consumptionTableRow.ouncesRemaining = (decimal)reader["ounces_remaining"];
                consumptionTableRow.restock = (int)reader["refill"];
                return consumptionTableRow;
            });
            return consumptionTableRow;
        }
        public List<Ingredient> queryConsumptionTableSorted() {
            var db = new DatabaseAccess();
            var commandText = @"SELECT * FROM consumption
                                ORDER BY name ASC";
            var myConsumptionTable = db.queryItems(commandText, reader => {
                var myConsumptionIngredient = new Ingredient((string)reader["name"]);
                myConsumptionIngredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                myConsumptionIngredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                myConsumptionIngredient.restock = (int)reader["refill"];
                return myConsumptionIngredient;
            });
            return myConsumptionTable;
        }
    }
}