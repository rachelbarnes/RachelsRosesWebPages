using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessConsumptionOuncesConsumed {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public void dropIfConsumptionOuncesConsumptionTableExists(string table) {
            var db = new DatabaseAccess();
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void initializeConsumptionOuncesConsumedTable() {
            var db = new DatabaseAccess();
            dropIfConsumptionOuncesConsumptionTableExists("consumption_ounces_consumed");
            db.executeVoidQuery(@"create table consumption_ounces_consumed (
                        ingredient nvarchar(max),
                        ounces_consumed decimal(4,2),
                        ounces_remaining decimal(5,2),
                        measurement nvarchar(250)
                        );", a => a);
        }
        public List<Ingredient> queryConsumptionOuncesConsumed() {
            var db = new DatabaseAccess();
            var ingredientConsumptionInformation = db.queryItems("select * from consumption_ounces_consumed", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                ingredient.measurement = (string)reader["measurement"];
                ingredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                return ingredient;
            });
            return ingredientConsumptionInformation;
        }
        public void insertIngredientIntoConsumptionOuncesConsumed(Ingredient i) {
            var db = new DatabaseAccess();
            var dbi = new DatabaseAccessIngredient();
            var myConsumptionOuncesConsumedIngredient = new Ingredient();
            var dbc = new DatabaseAccessConsumption();
            var ingredientTableRow = dbi.queryIngredientFromIngredientsTableByName(i);
            var consumptiontablerow = dbc.queryConsumptionTableRowByName(i);
            if (i.classification.ToLower().Contains("egg"))
                i.classification = char.ToUpper(i.classification[0]) + i.classification.Substring(1, i.classification.Length - 1);
            var commandTextQueryMultipleRows = string.Format(@"SELECT ingredients.name, ingredients.measurement, consumption.ounces_consumed, consumption.ounces_remaining
                                                FROM ingredients
                                                JOIN consumption
                                                ON (ingredients.name=consumption.name AND ingredients.measurement=consumption.measurement) 
                                                    OR (ingredients.ingredient_classification=consumption.name AND ingredients.measurement=consumption.measurement) 
                                                WHERE ingredients.name='{0}' AND ingredients.measurement='{1}' AND ingredients.ingredient_classification='{2}';", i.name, i.measurement, i.classification);
            var myListOfQueriedIngredients = db.queryItems(commandTextQueryMultipleRows, reader => {
                myConsumptionOuncesConsumedIngredient.name = (string)reader["name"];
                myConsumptionOuncesConsumedIngredient.measurement = (string)reader["measurement"];
                myConsumptionOuncesConsumedIngredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                myConsumptionOuncesConsumedIngredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                return myConsumptionOuncesConsumedIngredient;
            });
            var commandTextVarsFilled = string.Format(@"INSERT INTO consumption_ounces_consumed 
                                                        (name, ounces_consumed, ounces_remaining, measurement) 
                                                        VALUES ('{0}', {1}, {2}, '{3}');", myListOfQueriedIngredients[0].name, myListOfQueriedIngredients[0].ouncesConsumed, myListOfQueriedIngredients[0].ouncesRemaining, myListOfQueriedIngredients[0].measurement);
            db.executeVoidQuery(commandTextVarsFilled, cmd => { return cmd; });
            //as a note to self, i was using the querySingleItem from DatabaseAccess, and that's the difference between my working query and my query that reutrned null...
            //something is off w that method. 
            //check: 
            var myConsumptionOuncesConsumedTable = queryConsumptionOuncesConsumed();
        }
        public void insertListOfIngredientsIntoConsumptionOuncesConsumed(List<Ingredient> myIngredients) {
            foreach (var ingredient in myIngredients)
                insertIngredientIntoConsumptionOuncesConsumed(ingredient);
        }
        //will this be enough for records? Should I include the ingredientId? 
        public void updateIngredientInConsumptionouncesConsumed(Ingredient i) {
            var db = new DatabaseAccess();
            var commandText = @"Update consumption_ounces_consumed set ounces_consumed=@ounces_consumed, ounces_remaining=@ounces_remaining where name=@name and measurement=@measurement;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                return cmd;
            });
        }
        public decimal CalculateOuncesConsumedFromMeasurement(Ingredient i) {
            var dbIngredients = new DatabaseAccessIngredient();
            var convertMeasurement = new ConvertMeasurement();
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredientIngredientsTableData = dbIngredients.queryIngredientFromIngredientsTableByName(i);
            var myConsumedOunces = 0m;
            var temp = new Ingredient();
            if (myIngredientIngredientsTableData.classification.ToLower().Contains("egg")) {
                var accumulatedOunces = convertMeasurement.AccumulatedTeaspoonMeasurement(i.measurement);
                if (i.classification.ToLower().Contains("egg")) {
                    var splitEggMeasurement = convertWeight.SplitWeightMeasurement(i.sellingWeight);
                    i.sellingWeightInOunces = decimal.Parse(splitEggMeasurement[0]);
                }
            }
            myConsumedOunces = convert.CalculateOuncesUsed(i);
            return myConsumedOunces;
        }
        public Ingredient queryConsumptionOuncesConsumedTableByName(Ingredient i) {
            var db = new DatabaseAccess();
            var consumptionOucnesConsumedIngredient = new Ingredient();
            var commandTextConsumptionRow = string.Format(@"SELECT * FROM consumption_ounces_consumed WHERE name='{0}'", i.name);
            db.queryItems(commandTextConsumptionRow, reader => {
                consumptionOucnesConsumedIngredient.name = (string)reader["name"];
                consumptionOucnesConsumedIngredient.measurement = (string)reader["measurement"];
                consumptionOucnesConsumedIngredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                consumptionOucnesConsumedIngredient.ouncesRemaining = (decimal)reader["ounces_remaining"];
                return consumptionOucnesConsumedIngredient;
            });
            return consumptionOucnesConsumedIngredient;
        }
    }
}