using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessDensities{
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
          public void dropDensityInformationTableIfExists(string table) {
            var db = new DatabaseAccess(); 
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void initializeDensitiesTable() {
            var db = new DatabaseAccess();
            dropDensityInformationTableIfExists("densities"); 
            db.executeVoidQuery(@"create table densities(
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
        }
        public static ItemResponse myItemResponse = new ItemResponse();
        public ItemResponse returnItemResponse(Ingredient i) {
            var rest = new MakeRESTCalls();
            return rest.GetItemResponse(i);
        }
        public List<string> getListOfDistinctSellingWeights() {
            var myDensiitiesTable = queryDensitiesTable();
            var myUniqueSellingWeights = new List<string>();
            foreach (var ingredient in myDensiitiesTable) {
                if (!myUniqueSellingWeights.Contains(ingredient.sellingWeight))
                    myUniqueSellingWeights.Add(ingredient.sellingWeight);
            }
            myUniqueSellingWeights.Sort(); return myUniqueSellingWeights;
        }
        public List<Ingredient> queryDensitiesTable() {
            var db = new DatabaseAccess();
            var ingredientInformation = db.queryItems("select * from densities", reader => {
                var ingredient = new Ingredient(reader["name"].ToString());
                ingredient.ingredientId = (int)reader["ing_id"];
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
            var db = new DatabaseAccess(); 
            var dbDensityInformation = new DatabaseAccessDensityInformation(); 
            myItemResponse = returnItemResponse(i);
            i.density = dbDensityInformation.returnIngredientDensityFromDensityTable(i);
            if (i.sellingPrice == 0m)
                i.sellingPrice = myItemResponse.salePrice;
            if (i.classification.ToLower() == "egg" || i.classification.ToLower() == "eggs") {
                i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
            } else i.sellingWeightInOunces = convert.ConvertWeightToOunces(i.sellingWeight);
            if (i.sellingWeightInOunces == 0m)
                throw new Exception("Selling Weight In Ounces is 0; please check that your Selling Weight is an appopriate weight.");
            i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
            if (string.IsNullOrEmpty(i.classification))
                i.classification = " ";
            var commandText = @"Insert into densities (name, density, selling_weight, selling_weight_ounces, selling_price, price_per_ounce) 
                            values (@name, @density, @selling_weight, @selling_weight_ounces, @selling_price, @price_per_ounce);";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                return cmd;
            });
        }
        public void updateDensityTable(Ingredient i) {
            var db = new DatabaseAccess(); 
            var commandText = "update densities set name=@name, density=@density, selling_weight=@selling_weight, selling_weight_ounces=@selling_weight_ounces, selling_price=@selling_price, price_per_ounce=@price_per_ounce where ing_id=@ing_id";
            db.executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@density", i.density);
                cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
                cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
                cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
                cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
                return cmd;
            });
        }

        public void DeleteIngredientFromDensitiesTable(Ingredient i) {
            var db = new DatabaseAccess(); 
            var deleteCommand = "delete from densities where ing_id=@ing_id";
            db.executeVoidQuery(deleteCommand, cmd => {
                cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
                return cmd;
            });
        }
      //public Ingredient queryIngredientFromDensityTableByName(Ingredient i) {
      //      var db = new DatabaseAccess();
      //      var ingredientDensities = new Ingredient(); 
      //      var commandTextQueryDensityByName = string.Format(@"SELECT * FROM densities WHERE name='{0}';", i.name); 
      //      db.queryItems(commandTextQueryDensityByName, reader => {

      //      })
      //  }
    }
}