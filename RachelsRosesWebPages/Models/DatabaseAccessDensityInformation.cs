using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessDensityInformation {
        const string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;User Id=RACHELSLAPTOP\\Rachel;Initial Catalog=RachelsRosesWebPagesDB;Integrated Security=True; MultipleActiveResultSets=True";
        public void dropDensityInformationTableIfExists(string table) {
            var db = new DatabaseAccess();
            var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
            db.executeVoidQuery(drop, a => a);
        }
        public void InitializeDensityInformationTable() {
            var db = new DatabaseAccess();
            dropDensityInformationTableIfExists("densityInfo");
            db.executeVoidQuery(@"create table densityInfo (
                        ingredient nvarchar(max),
                        density decimal(4,2)
                        );", a => a);
        }
        public ItemResponse returnItemResponse(Ingredient i) {
            var rest = new MakeRESTCalls();
            return rest.GetItemResponse(i);
        }
        public List<Ingredient> queryDensityInfoTable() {
            var db = new DatabaseAccess();
            var DensityInfo = db.queryItems("select * from densityInfo order by ingredient desc", reader => {
                var densityIngredientInformation = new Ingredient(reader["ingredient"].ToString());
                densityIngredientInformation.density = (decimal)reader["density"];
                return densityIngredientInformation;
            });
            return DensityInfo;
        }
        public void insertIngredientIntoDensityInfoDatabase(Ingredient i) {
            var rest = new MakeRESTCalls();
            var db = new DatabaseAccess();
            insertDensityTextFileIntoDensityInfoDatabase();
            var myUpdatedDensityInfoTable = queryDensityInfoTable();
            var myMilkAndEggDensityInfoIngredients = new List<Ingredient>();
            foreach (var ingredient in myUpdatedDensityInfoTable) {
                if (ingredient.name.ToLower().Contains("milk") || ingredient.name.ToLower().Contains("egg"))
                    myMilkAndEggDensityInfoIngredients.Add(ingredient);
            }
            var countSimilarIngredients = 0;
            foreach (var ingredient in myUpdatedDensityInfoTable) {
                if (i.typeOfIngredient.ToLower().Contains("milk") || i.typeOfIngredient.ToLower().Contains("egg")) {
                    foreach (var dairyOrEggIngredient in myMilkAndEggDensityInfoIngredients) {
                        if (i.typeOfIngredient == dairyOrEggIngredient.name) {
                            countSimilarIngredients++;
                            break;
                        }
                    }
                    break;
                } else {
                    if (rest.SimilaritesInStrings(i.typeOfIngredient, ingredient.name)) {
                        countSimilarIngredients++;
                        break;
                    }
                }
            }
            if (countSimilarIngredients == 0) {
                var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
                db.executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@ingredient", i.typeOfIngredient);
                    cmd.Parameters.AddWithValue("@density", i.density);
                    return cmd;
                });
            }
            //all this is doing is determining if the density table already has an ingredient with said name, if so, then it won't add it, if the table doesn't have that name, it will insert it with the density
            var myDensityInfoDatabase = queryDensityInfoTable();
        }
        public void insertDensityTextFileIntoDensityInfoDatabase() {
            var read = new Reader(); //the filename below for the moment is hardcoded... 
            var db = new DatabaseAccess();
            var DensityTextDatabaseDictionary = read.ReadDensityTextFile(@"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
            var myDensityTable = queryDensityInfoTable();
            var myDensityTableNames = new List<string>();
            foreach (var ingredient in myDensityTable)
                myDensityTableNames.Add(ingredient.name);
            foreach (var ingredient in DensityTextDatabaseDictionary) {
                if (!myDensityTableNames.Contains(ingredient.Key)) {
                    var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
                    db.executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@ingredient", ingredient.Key);
                        cmd.Parameters.AddWithValue("@density", ingredient.Value);
                        return cmd;
                    });
                }
            }
            var myDensityTableAfter = queryDensityInfoTable();
        }
        public void insertListIntoDensityInfoDatabase(List<Ingredient> MyIngredients) {
            var read = new Reader(); //the filename below for the moment is hardcoded... but i would prefer to not keep it that way... bad business
            var db = new DatabaseAccess();
            var myDensityTable = queryDensityInfoTable();
            var myDensityInfoTableIngredients = new List<string>();
            foreach (var ingredient in myDensityTable)
                myDensityInfoTableIngredients.Add(ingredient.typeOfIngredient);
            for (int i = 0; i < MyIngredients.Count(); i++) {
                if (!myDensityInfoTableIngredients.Contains(MyIngredients[i].typeOfIngredient)) {
                    var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
                    db.executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@ingredient", MyIngredients[i].name);
                        cmd.Parameters.AddWithValue("@density", MyIngredients[i].density);
                        return cmd;
                    });
                }
            }
            var myDensityInfoTable = queryDensityInfoTable();
        }
        public void updateDensityInfoTable(Ingredient myIngredient) {
            var db = new DatabaseAccess();
            var myDensityTableInfo = queryDensityInfoTable();
            var myDensityTableInfoNames = new List<string>();
            foreach (var ingredient in myDensityTableInfo)
                myDensityTableInfoNames.Add(ingredient.name);
            if (!myDensityTableInfoNames.Contains(myIngredient.typeOfIngredient))
                insertIngredientIntoDensityInfoDatabase(myIngredient);
            else {
                var commandText = @"Update densityInfo set density=@density where ingredient=@ingredient;";
                db.executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@ingredient", myIngredient.typeOfIngredient);
                    cmd.Parameters.AddWithValue("@density", myIngredient.density);
                    return cmd;
                });
            }
        }
        public decimal queryDensityTableRowDensityValueByName(Ingredient i) {
            var rest = new MakeRESTCalls();
            var db = new DatabaseAccess(); 
            var myIngredient = new Ingredient(); 
            var commandTextQueryTableRowByName = string.Format(@"SELECT * FROM densityInfo WHERE ingredient='{0}';", i.typeOfIngredient);
            db.queryItems(commandTextQueryTableRowByName, reader => {
                myIngredient.name = (string)reader["ingredient"];
                myIngredient.density = (decimal)reader["density"];
                return myIngredient; 
            });
            return myIngredient.density;
        }
        public void updateListOfIngredientsInDensityInfoTable(List<Ingredient> MyIngredients) {
            var myDensityTableInfo = queryDensityInfoTable();
            var myDensityTableInfoNames = new List<string>();
            foreach (var ingredient in myDensityTableInfo)
                myDensityTableInfoNames.Add(ingredient.name);
            for (int i = 0; i < MyIngredients.Count(); i++) {
                if (!myDensityTableInfoNames.Contains(MyIngredients[i].name))
                    insertIngredientIntoDensityInfoDatabase(MyIngredients[i]);
                else
                    updateDensityInfoTable(MyIngredients[i]);
            }
        }

        public List<string> getListOfIngredientTypesFromDensityTable() {
            var myDensityTable = queryDensityInfoTable();
            var myIngredientTypes = new List<string>();
            foreach (var ingredient in myDensityTable) {
                if (!myIngredientTypes.Contains(ingredient.name))
                    myIngredientTypes.Add(ingredient.name);
            }
            myIngredientTypes.Sort();
            return myIngredientTypes;
        }

    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it