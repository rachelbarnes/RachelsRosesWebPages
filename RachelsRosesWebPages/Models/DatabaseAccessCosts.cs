using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessCosts {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public void dropCostTableIfExists(string table) {
            var db = new DatabaseAccess();
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void initializeCostTable() {
            var db = new DatabaseAccess();
            dropCostTableIfExists("costs");
            db.executeVoidQuery(@"create table costs (
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
        }
        public ItemResponse returnItemResponse(Ingredient i) {
            var rest = new MakeRESTCalls();
            return rest.GetItemResponse(i);
        }
        public static ItemResponse myItemResponse = new ItemResponse();
        public List<Ingredient> queryCostTable() {
            var db = new DatabaseAccess();
            var ingredientInformation = db.queryItems("select * from costs", reader => {
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
            var db = new DatabaseAccess();
            var convert = new ConvertWeight();
            var myCostTable = queryCostTable();
            var temp = new Ingredient();
            temp.sellingPrice = i.sellingPrice;
            if (i.classification.ToLower().Contains("egg")) {
                i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
                i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
            }
            var commandText = @"Insert into costs (name, selling_weight, selling_price, price_per_ounce, item_id) values (@name, @selling_weight, @selling_price, @price_per_ounce, @item_id);";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", temp.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
        }
        public void updateCostDataTable(Ingredient i) {
            var db = new DatabaseAccess();
            var myCostTable = queryCostTable();
            var commandText = @"Update costs set name=@name, selling_weight=@selling_weight, selling_price=@selling_price, price_per_ounce=@price_per_ounce where ing_id=@ing_id;";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", getPricePerOunce(i));
                cmd.Parameters.AddWithValue("@item_id", i.itemId);
                return cmd;
            });
            var myUpdatedCostTable = queryCostTable();
        }
        public decimal getPricePerOunce(Ingredient i) {
            var convert = new ConvertWeight();
            var myCostTableIngredients = queryCostTable();
            var pricePerOunce = 0m;
            foreach (var ingredient in myCostTableIngredients) {
                if (ingredient.name == i.name) {
                    i.sellingPrice = ingredient.sellingPrice;
                    if (i.classification.ToLower().Contains("egg"))
                        i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
                    else i.sellingWeightInOunces = convert.ConvertWeightToOunces(ingredient.sellingWeight);
                    i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
                    pricePerOunce = i.pricePerOunce;
                }
            }
            return pricePerOunce;
        }
        public void DeleteIngredientFromCostTable(Ingredient i) {
            var db = new DatabaseAccess(); 
            var deleteCommand = "delete from costs where ing_id=@ing_id"; //if needed, : AND name=@name;
            db.executeVoidQuery(deleteCommand, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                return cmd;
            });
        }
        public Ingredient queryCostsTableByName(Ingredient i) {
            var db = new DatabaseAccess();
            var myIngredient = new Ingredient(); 
            var commandTextQueryCostTableIngredient = string.Format(@"SELECT * FROM costs WHERE name='{0}';", i.name);
            db.queryItems(commandTextQueryCostTableIngredient, reader => {
                myIngredient.name = (string)reader["name"];
                myIngredient.ingredientId = (int)reader["ing_id"];
                myIngredient.sellingWeight = (string)reader["selling_weight"];
                //myIngredient.sellingWeightInOunces = (decimal)reader["selling_weight_ounces"];
                myIngredient.pricePerOunce = (decimal)reader["price_per_ounce"];
                myIngredient.itemId = (int)reader["item_id"];
                return myIngredient;
            });
            return myIngredient; 
        } 
    }
}