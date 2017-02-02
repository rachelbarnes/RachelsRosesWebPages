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
            var ingredientInformation = db.queryItems("select * from consumption", reader => {
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
                temp.name = "eggs";
                i.ouncesConsumed = convertWeight.EggsConsumedFromIngredientMeasurement(myIngredientIngredientTable.measurement);
            } else i.ouncesConsumed = dbConsumptionOuncesConsumed.CalculateOuncesConsumedFromMeasurement(i);
            foreach (var ingredient in myConsumptionTable) {
                //should be able to do a sql query of if the ingredient is in the table... would be quicker than doing a loop i presume
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
        //if (i.classification.ToLower().Contains("egg") && ingredient.name.ToLower().Contains("egg")) {
        //    temp.name = ingredient.name;
        //    var currentOuncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
        //    if (ingredient.ouncesConsumed != currentOuncesConsumed)
        //        i.ouncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
        //    if (ingredient.ouncesRemaining == 0m)
        //        i.ouncesRemaining = i.sellingWeightInOunces - i.ouncesConsumed;
        //    else i.ouncesRemaining = ingredient.ouncesRemaining - i.ouncesConsumed;
        //    break;


        //if (ingredient.name.ToLower() == i.name.ToLower()) {
        //    ingredient.ouncesConsumed = dbConsumptionOuncesConsumed.CalculateOuncesConsumedFromMeasurement(i);
        //    i.ouncesConsumed = ingredient.ouncesConsumed;
        //    break;
        //}
        //    }
        //}
        public void updateConsumptionTable(Ingredient i) {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbConsumptionOuncesConsumed = new DatabaseAccessConsumptionOuncesConsumed();
            var convert = new ConvertWeight();
            var myIngredient = dbI.queryIngredientFromIngredientsTableByName(i);
            var myConsumptionTableIngredient = queryConsumptionTableRowByName(i);
            var temp = new Ingredient();
            //this handles egg classifications, calculates ounces consumed and ounces remaining
            if (myIngredient.classification.ToLower().Contains("egg")) {
                temp.name = myIngredient.name;
                var currentOuncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                if (myConsumptionTableIngredient.ouncesConsumed != currentOuncesConsumed)
                    i.ouncesConsumed = convert.EggsConsumedFromIngredientMeasurement(i.measurement);
                if (myConsumptionTableIngredient.ouncesRemaining == 0m)
                    i.ouncesRemaining = i.sellingWeightInOunces - i.ouncesConsumed;
                else i.ouncesRemaining = myConsumptionTableIngredient.ouncesRemaining - myConsumptionTableIngredient.ouncesConsumed;
            }
            //this handles all other ingredients, calculates ounces consumed and ounces remaining
             else {
                myConsumptionTableIngredient.ouncesConsumed = dbConsumptionOuncesConsumed.CalculateOuncesConsumedFromMeasurement(i);
                i.ouncesConsumed = myConsumptionTableIngredient.ouncesConsumed;
                //i need the density table's selling weight in ounces... i got that from querying all the tables, so i'm not able to do any measureents for the costs...
                if (myConsumptionTableIngredient.ouncesRemaining == 0m) {
                    myConsumptionTableIngredient.ouncesRemaining = myConsumptionTableIngredient.sellingWeightInOunces - myConsumptionTableIngredient.ouncesConsumed;
                } else
                    myConsumptionTableIngredient.ouncesRemaining = myConsumptionTableIngredient.ouncesRemaining - myConsumptionTableIngredient.ouncesConsumed;
                i.ouncesRemaining = myConsumptionTableIngredient.ouncesRemaining;
            }
            dbConsumptionOuncesConsumed.insertIngredientIntoConsumptionOuncesConsumed(i);
            var consumptionOuncesConsumed = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed(); 
            if (string.IsNullOrEmpty(temp.name))
                temp.name = i.name;
            if (doesIngredientNeedRestocking(i))
                i.restock = 0;
            else i.restock = 1;
            //subtractOuncesRemainingIfExpirationDateIsPast(i);
            // this needs to be fixed, maybe for hte moment having a condition for ig it is eggs or dairy... flour and sugar, etc. should be totally fine
            var commandText = "update consumption set ounces_consumed=@ounces_consumed, ounces_remaining=@ounces_remaining, refill=@refill where name=@name;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", temp.name);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                cmd.Parameters.AddWithValue("@refill", i.restock);
                return cmd;
            });
            //i.ouncesRemaining = getOuncesRemainingFromConsumptionTableFromIngredient(i);
            //var refillCommandText = "update consumption set refill=@refill where name=@name;";
            //db.executeVoidQuery(refillCommandText, cmd => {
            //    cmd.Parameters.AddWithValue("@name", i.name);
            //    return cmd;
            //});
            var myUpdatedIngredient = queryConsumptionTableRowByName(i);
            var myUpdatedConsumptionOuncesConsumedTable = dbConsumptionOuncesConsumed.queryConsumptionOuncesConsumed();
        }
        public void subtractOuncesRemainingIfExpirationDateIsPast(Ingredient i) {
            var db = new DatabaseAccess();
            var dbIngredients = new DatabaseAccessIngredient();
            var convert = new ConvertWeight();
            var myIngredient = db.queryAllRelevantTablesSQL(i);
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
    }
}