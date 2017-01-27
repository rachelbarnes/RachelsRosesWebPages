using RachelsRosesWebPages.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RachelsRosesWebPages.Models {
    public class DatabaseAccessDensityInformation{
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
        public List<Ingredient> queryDensityInfoTable() {
            var db = new DatabaseAccess(); 
            var DensityInfo = db.queryItems("select * from densityInfo", reader => {
                var densityIngredientInformation = new Ingredient(reader["ingredient"].ToString());
                densityIngredientInformation.density = (decimal)reader["density"];
                return densityIngredientInformation;
            });
            return DensityInfo;
        }
        public void insertIngredientIntoDensityInfoDatabase(Ingredient i) {
            var rest = new MakeRESTCalls();
            var db = new DatabaseAccess(); 
            var myDensityInfoTable = queryDensityInfoTable();
            if (myDensityInfoTable.Count() == 0)
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
        public List<Ingredient> assignIngredientDensityDictionaryValuesToListIngredients(Dictionary<string, decimal> myDensityIngredientDictionary) {
            var myIngredients = new List<Ingredient>();
            foreach (var pair in myDensityIngredientDictionary) {
                var currentIngredient = new Ingredient(pair.Key) {
                    density = pair.Value
                };
                myIngredients.Add(currentIngredient);
            }
            return myIngredients;
        }
        public void insertDensityTextFileIntoDensityInfoDatabase() {
            var read = new Reader(); //the filename below for the moment is hardcoded... 
            var db = new DatabaseAccess(); 
            var DensityTextDatabaseDictionary = read.ReadDensityTextFile(@"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
            var myDensityTable = queryDensityInfoTable();
            var myDensityTableNames = new List<string>();
            foreach (var ingredient in myDensityTable)
                myDensityTableNames.Add(ingredient.name);
            //this is going to need to allow for user error and grace in the name... need to have a similaries check, or make sure the name.tolower contains the ingredient's name, as opposed to == it
            //i may have fixed this with the type of ingredient.... but i'll have to do more tests around that to see if it's intuitive
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
        public decimal returnIngredientDensityFromDensityTable(Ingredient i) {
            var rest = new MakeRESTCalls();
            var dbIngredient = new DatabaseAccessIngredient(); 
            var myIngredients = dbIngredient.queryIngredients();
            var myDensityIngredients = queryDensityInfoTable();
            var myIngredientDensity = 0m;
            foreach (var ingredient in myDensityIngredients) {
                if (rest.SimilaritesInStrings(i.typeOfIngredient, ingredient.name)) {
                    myIngredientDensity = ingredient.density;
                    break;
                }
            }
            return myIngredientDensity;
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

    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it