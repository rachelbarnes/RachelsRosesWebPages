using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessConsumptionOuncesConsumed{
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
                        density decimal(4,2),
                        measurement nvarchar(250)
                        );", a => a);
        }
        public List<Ingredient> queryConsumptionOuncesConsumed() {
            var db = new DatabaseAccess(); 
            var ingredientConsumptionInformation = db.queryItems("select * from consumption_ounces_consumed", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
                ingredient.measurement = (string)reader["measurement"];
                return ingredient;
            });
            return ingredientConsumptionInformation;
        }
        public void insertIngredientIntoConsumptionOuncesConsumed(Ingredient i) {
            var db = new DatabaseAccess(); 
            var commandText = @"Insert into consumption_ounces_consumed (name, ounces_consumed, measurement) values (@name, @ounces_consumed, @measurement);";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                return cmd;
            });
            //check: 
            var myConsumptionOuncesConsumedTable = queryConsumptionOuncesConsumed(); 
        }
        public void insertListOfIngredientsIntoConsumptionOuncesConsumed(List<Ingredient> myIngredients) {
            foreach (var ingredient in myIngredients)
                insertIngredientIntoConsumptionOuncesConsumed(ingredient);
        }
        public void updateIngredientInConsumptionouncesConsumed(Ingredient i) {
            var db = new DatabaseAccess(); 
            var commandText = @"Update consumption_ounces_consumed set ounces_consumed=@ounces_consumed where name=@name and measurement=@measurement;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@measurement", i.measurement);
                cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
                cmd.Parameters.AddWithValue("@name", i.name);
                return cmd;
            });
        }
        public decimal CalculateOuncesConsumedFromMeasurement(Ingredient i) {
            var dbConsumption = new DatabaseAccessConsumption();
            var dbIngredients = new DatabaseAccessIngredient(); 
           var convertMeasurement = new ConvertMeasurement();
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredientConsumptionData = dbConsumption.queryConsumptionTable();
            var myIngredients = dbIngredients.queryIngredients();
            var myConsumedOunces = 0m;
            var temp = new Ingredient();
            foreach (var ingredient in myIngredients) {
                if (ingredient.name == i.name) {
                    var accumulatedOunces = convertMeasurement.AccumulatedTeaspoonMeasurement(i.measurement);
                    if (i.classification.ToLower().Contains("egg")) {
                        var splitEggMeasurement = convertWeight.SplitWeightMeasurement(i.sellingWeight);
                        i.sellingWeightInOunces = decimal.Parse(splitEggMeasurement[0]);
                    }
                    myConsumedOunces = convert.CalculateOuncesUsed(i);
                }
            }
            return myConsumedOunces;
        }
    }
}