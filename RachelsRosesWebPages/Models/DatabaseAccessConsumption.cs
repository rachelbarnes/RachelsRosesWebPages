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
            dropConsumptionTableIfExists("recipes");
            db.executeVoidQuery(@"create table recipes (
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
        ////densities table methods: 
        //public List<Ingredient> queryDensitiesTable() {
        //    var ingredientInformation = queryItems("select * from densities", reader => {
        //        var ingredient = new Ingredient(reader["name"].ToString());
        //        ingredient.ingredientId = (int)reader["ing_id"];
        //        ingredient.density = (decimal)reader["density"];
        //        ingredient.sellingWeight = (string)reader["selling_weight"];
        //        ingredient.sellingWeightInOunces = (decimal)reader["selling_weight_ounces"];
        //        ingredient.sellingPrice = (decimal)reader["selling_price"];
        //        ingredient.pricePerOunce = (decimal)reader["price_per_ounce"];
        //        return ingredient;
        //    });
        //    return ingredientInformation;
        //}
        //public void insertIngredientDensityData(Ingredient i) {
        //    var convert = new ConvertWeight();
        //    myItemResponse = returnItemResponse(i);
        //    i.density = returnIngredientDensityFromDensityTable(i);
        //    if (i.sellingPrice == 0m)
        //        i.sellingPrice = myItemResponse.salePrice;
        //    if (i.classification.ToLower() == "egg" || i.classification.ToLower() == "eggs") {
        //        i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
        //    } else i.sellingWeightInOunces = convert.ConvertWeightToOunces(i.sellingWeight);
        //    if (i.sellingWeightInOunces == 0m)
        //        throw new Exception("Selling Weight In Ounces is 0; please check that your Selling Weight is an appopriate weight.");
        //    i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
        //    if (string.IsNullOrEmpty(i.classification))
        //        i.classification = " ";
        //    var commandText = @"Insert into densities (name, density, selling_weight, selling_weight_ounces, selling_price, price_per_ounce) 
        //                    values (@name, @density, @selling_weight, @selling_weight_ounces, @selling_price, @price_per_ounce);";
        //    executeVoidQuery(commandText, cmd => {
        //        cmd.Parameters.AddWithValue("@name", i.name);
        //        cmd.Parameters.AddWithValue("@density", i.density);
        //        cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
        //        cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
        //        cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
        //        cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
        //        return cmd;
        //    });
        //}
        //public void updateDensityTable(Ingredient i) {
        //    var commandText = "update densities set name=@name, density=@density, selling_weight=@selling_weight, selling_weight_ounces=@selling_weight_ounces, selling_price=@selling_price, price_per_ounce=@price_per_ounce where ing_id=@ing_id";
        //    executeVoidQuery(commandText, cmd => {
        //        cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
        //        cmd.Parameters.AddWithValue("@name", i.name);
        //        cmd.Parameters.AddWithValue("@density", i.density);
        //        cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
        //        cmd.Parameters.AddWithValue("@selling_weight_ounces", i.sellingWeightInOunces);
        //        cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
        //        cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
        //        return cmd;
        //    });
        //}

        ////i'm going to have to say that if an ingredient is deleted from any of these, that I have to add the ounces consumed back
        ////and have a box select (if the recipe was made, then actually remove the ouncesConsumed from the ouncesRemaining)
        ////if it hasn't been made, show a potential of what the ouncesRemaining would be, vs what it actually is
        ////but before i get that, i want to make sure my view is doing what it needs to do
        ////make a check box that indicates if the recipe has been made... if it has been made, then enact this updateConsumptionTable method

        ////consumption table methods: 
        public List<Ingredient> queryConsumptionTable() {
            var ingredientInformation = queryItems("select * from consumption", reader => {
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
            var convertWeight = new ConvertWeight();
            var convert = new ConvertDensity();
            var myIngredient = queryAllTablesForIngredient(i);
            var myConsumptionTable = queryConsumptionTable();
            var temp = new Ingredient();
            bool alreadyContainsIngredient = new bool();
            if (myIngredient.classification.ToLower().Contains("egg")) {
                temp.name = "eggs";
                i.ouncesConsumed = convertWeight.EggsConsumedFromIngredientMeasurement(myIngredient.measurement);
            } else i.ouncesConsumed = CalculateOuncesConsumedFromMeasurement(i);
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
                executeVoidQuery(commandText, cmd => {
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
            var convert = new ConvertWeight();
            var myIngredient = queryAllTablesForIngredient(i);
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
                        ingredient.ouncesConsumed = CalculateOuncesConsumedFromMeasurement(i);
                        i.ouncesConsumed = ingredient.ouncesConsumed;
                        insertIngredientIntoConsumptionOuncesConsumed(myIngredient);
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
            executeVoidQuery(commandText, cmd => {
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
            executeVoidQuery(refillCommandText, cmd => {
                cmd.Parameters.AddWithValue("@refill", i.restock);
                cmd.Parameters.AddWithValue("@name", i.name);
                return cmd;
            });

            var myUpdatedIngredient = queryConsumptionTable();
            var myUpdatedConsumptionOuncesConsumedTable = queryConsumptionOuncesConsumed();
        }
        public void subtractOuncesRemainingIfExpirationDateIsPast(Ingredient i) {
            var convert = new ConvertWeight();
            var myIngredient = queryAllTablesForIngredient(i);
            if (i.expirationDate < DateTime.Today && (convertDateToStringMMDDYYYY(i.expirationDate) != "01/01/0001")) {
                //i.expirationDate != new DateTime()) {
                myIngredient.ouncesRemaining = myIngredient.ouncesRemaining - i.sellingWeightInOunces;
                if (myIngredient.ouncesRemaining < 0m)
                    myIngredient.ouncesRemaining = 0m;
                var commandText = @"update consumption set ounces_remaining=@ounces_remaining where name=@name";
                executeVoidQuery(commandText, cmd => {
                    cmd.Parameters.AddWithValue("@name", myIngredient.name);
                    cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                    return cmd;
                });
            }
            var myUpdatedIngredient = queryConsumptionTable();
        }
        public void refillIngredientInConsumptionDatabase(Ingredient i, string sellingWeightToRefill) {
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
            executeVoidQuery(commandText, cmd => {
                cmd.Parameters.AddWithValue("@name", i.name);
                cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                return cmd;
            });
        }
        public void refillIngredientInConsumptionDatabase(Ingredient i, string sellingWeightToRefill, string newExpirationDate) {
            var convert = new ConvertWeight();
            var myConsumptionTable = queryConsumptionTable();
            var myIngredientTable = queryIngredients();
            var sellingWeightToRefillOunces = convert.ConvertWeightToOunces(sellingWeightToRefill);
            foreach (var ingredient in myConsumptionTable) {
                if (ingredient.name.ToLower() == i.name.ToLower()) {
                    if (i.ouncesRemaining < 0m)
                        i.ouncesRemaining = 0m;
                    i.ouncesRemaining = ingredient.ouncesRemaining + sellingWeightToRefillOunces;
                    var commandText = "update consumption set ounces_remaining=@ounces_remaining where name=@name;";
                    executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@name", i.name);
                        cmd.Parameters.AddWithValue("@ounces_remaining", i.ouncesRemaining);
                        return cmd;
                    });
                    break;
                }
            }
            foreach (var ingredient in myIngredientTable) {
                if (ingredient.ingredientId == i.ingredientId && ingredient.name.ToLower() == i.name.ToLower()) {
                    ingredient.expirationDate = convertStringToDateYYYYMMDD(newExpirationDate);
                    var commandText = "update ingredients set expiration_date=@expiration_date where ing_id=@ing_id";
                    executeVoidQuery(commandText, cmd => {
                        cmd.Parameters.AddWithValue("@expiration_date", convertDateToStringMMDDYYYY(ingredient.expirationDate));
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
        //public List<Ingredient> queryConsumptionOuncesConsumed() {
        //    var ingredientConsumptionInformation = queryItems("select * from consumption_ounces_consumed", reader => {
        //        var ingredient = new Ingredient(reader["name"].ToString());
        //        ingredient.ouncesConsumed = (decimal)reader["ounces_consumed"];
        //        ingredient.measurement = (string)reader["measurement"];
        //        return ingredient;
        //    });
        //    return ingredientConsumptionInformation;
        //}
        //public void insertIngredientIntoConsumptionOuncesConsumed(Ingredient i) {
        //    var commandText = @"Insert into consumption_ounces_consumed (name, ounces_consumed, measurement) values (@name, @ounces_consumed, @measurement);";
        //    executeVoidQuery(commandText, cmd => {
        //        //cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
        //        cmd.Parameters.AddWithValue("@name", i.name);
        //        cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
        //        cmd.Parameters.AddWithValue("@measurement", i.measurement);
        //        return cmd;
        //    });
        //}
        //public void insertListOfIngredientsIntoConsumptionOuncesConsumed(List<Ingredient> myIngredients) {
        //    foreach (var ingredient in myIngredients)
        //        insertIngredientIntoConsumptionOuncesConsumed(ingredient);
        //}
        //public void updateIngredientInConsumptionouncesConsumed(Ingredient i) {
        //    var commandText = @"Update consumption_ounces_consumed set ounces_consumed=@ounces_consumed where name=@name and measurement=@measurement;";
        //    //var commandText = @"Update consumption_ounces_consumed set ounces_consumed=@ounces_consumed, name=@name where ing_id=@ing_id;";
        //    executeVoidQuery(commandText, cmd => {
        //        cmd.Parameters.AddWithValue("@measurement", i.measurement);
        //        cmd.Parameters.AddWithValue("@ounces_consumed", i.ouncesConsumed);
        //        cmd.Parameters.AddWithValue("@name", i.name);
        //        return cmd;
        //    });
        //}
        //public decimal CalculateOuncesConsumedFromMeasurement(Ingredient i) {
        //    var convertMeasurement = new ConvertMeasurement();
        //    var convertWeight = new ConvertWeight();
        //    var convert = new ConvertDensity();
        //    var myIngredientConsumptionData = queryConsumptionTable();
        //    var myIngredients = queryIngredients();
        //    var myConsumedOunces = 0m;
        //    var temp = new Ingredient();
        //    foreach (var ingredient in myIngredients) {
        //        if (ingredient.name == i.name) {
        //            var accumulatedOunces = convertMeasurement.AccumulatedTeaspoonMeasurement(i.measurement);
        //            if (i.classification.ToLower().Contains("egg")) {
        //                var splitEggMeasurement = convertWeight.SplitWeightMeasurement(i.sellingWeight);
        //                i.sellingWeightInOunces = decimal.Parse(splitEggMeasurement[0]);
        //            }
        //            myConsumedOunces = convert.CalculateOuncesUsed(i);
        //        }
        //    }
        //    return myConsumedOunces;
        //}
        ////date data type helper function: 
        //public DateTime convertIntToDate(int dateInInt) {
        //    var dateString = dateInInt.ToString();
        //    if (dateString.Length != 8) {
        //        return new DateTime();
        //    }
        //    var year = dateString.Substring(0, 4);
        //    var month = dateString.Substring(4, 2);
        //    var day = dateString.Substring(6, 2);
        //    return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        //}
        //public DateTime convertStringToDateYYYYMMDD(string dateString) {
        //    if (dateString.Length < 8)
        //        return new DateTime();
        //    var dateStringArray = new string[] { };
        //    if (dateString.Contains('.'))
        //        dateStringArray = dateString.Split('.');
        //    if (dateString.Contains('/'))
        //        dateStringArray = dateString.Split('/');
        //    if (dateString.Contains('-'))
        //        dateStringArray = dateString.Split('-');
        //    var year = int.Parse(dateStringArray[2]);
        //    var month = int.Parse(dateStringArray[0]);
        //    var day = int.Parse(dateStringArray[1]);
        //    return new DateTime(year, month, day);
        //}
        //public string convertDateToStringMMDDYYYY(DateTime date) {
        //    var dateTimeArray = date.ToString().Split(' ');
        //    var dateString = dateTimeArray[0];
        //    var dateStringArray = dateString.Split('/');
        //    var month = dateStringArray[0];
        //    if (int.Parse(month) < 10)
        //        month = "0" + month;
        //    var day = dateStringArray[1];
        //    if (int.Parse(day) < 10)
        //        day = "0" + day; 
        //    var year = dateStringArray[2];
        //    return string.Format("{0}/{1}/{2}", month, day, year);
        //}
        //public DateTime getExpirationDateFromIngredientsTable(Ingredient i) {
        //    var myIngredients = queryIngredients();
        //    var myIngredientExpirationDate = new DateTime();
        //    foreach (var ingredient in myIngredients) {
        //        if (ingredient.ingredientId == i.ingredientId) {
        //            (ingredient.expirationDate) = myIngredientExpirationDate;
        //            break;
        //        }
        //    }
        //    return myIngredientExpirationDate;
        //}
        ////cost table 
        //public List<Ingredient> queryCostTable() {
        //    var ingredientInformation = queryItems("select * from costs", reader => {
        //        var ingredient = new Ingredient(reader["name"].ToString());
        //        ingredient.ingredientId = (int)reader["ing_id"];
        //        ingredient.sellingWeight = (string)reader["selling_weight"];
        //        ingredient.sellingPrice = (decimal)reader["selling_price"];
        //        ingredient.pricePerOunce = (decimal)reader["price_per_ounce"];
        //        ingredient.itemId = (int)reader["item_id"];
        //        return ingredient;
        //    });
        //    return ingredientInformation;
        //}
        //public void insertIngredientCostDataCostTable(Ingredient i) {
        //    var convert = new ConvertWeight();
        //    var myCostTable = queryCostTable();
        //    var temp = new Ingredient();
        //    temp.sellingPrice = i.sellingPrice;
        //    if (i.classification.ToLower().Contains("egg")) {
        //        i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
        //        i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
        //    }
        //    var commandText = @"Insert into costs (name, selling_weight, selling_price, price_per_ounce, item_id) values (@name, @selling_weight, @selling_price, @price_per_ounce, @item_id);";
        //    executeVoidQuery(commandText, cmd => {
        //        cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
        //        cmd.Parameters.AddWithValue("@name", i.name);
        //        cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
        //        cmd.Parameters.AddWithValue("@selling_price", temp.sellingPrice);
        //        cmd.Parameters.AddWithValue("@price_per_ounce", i.pricePerOunce);
        //        cmd.Parameters.AddWithValue("@item_id", i.itemId);
        //        return cmd;
        //    });
        //}
        //public void updateCostDataTable(Ingredient i) {
        //    var myCostTable = queryCostTable();
        //    var commandText = @"Update costs set name=@name, selling_weight=@selling_weight, selling_price=@selling_price, price_per_ounce=@price_per_ounce where ing_id=@ing_id;";
        //    executeVoidQuery(commandText, cmd => {
        //        cmd.Parameters.AddWithValue("@ing_id", i.ingredientId);
        //        cmd.Parameters.AddWithValue("@name", i.name);
        //        cmd.Parameters.AddWithValue("@selling_weight", i.sellingWeight);
        //        cmd.Parameters.AddWithValue("@selling_price", i.sellingPrice);
        //        cmd.Parameters.AddWithValue("@price_per_ounce", getPricePerOunce(i));
        //        cmd.Parameters.AddWithValue("@item_id", i.itemId);
        //        return cmd;
        //    });
        //    var myUpdatedCostTable = queryCostTable();
        //}
        //public decimal getPricePerOunce(Ingredient i) {
        //    var convert = new ConvertWeight();
        //    var myCostTableIngredients = queryCostTable();
        //    var pricePerOunce = 0m;
        //    foreach (var ingredient in myCostTableIngredients) {
        //        if (ingredient.name == i.name) {
        //            i.sellingPrice = ingredient.sellingPrice;
        //            if (i.classification.ToLower().Contains("egg"))
        //                i.sellingWeightInOunces = convert.NumberOfEggsFromSellingQuantity(i.sellingWeight);
        //            else i.sellingWeightInOunces = convert.ConvertWeightToOunces(ingredient.sellingWeight);
        //            i.pricePerOunce = Math.Round((i.sellingPrice / i.sellingWeightInOunces), 4);
        //            pricePerOunce = i.pricePerOunce;
        //        }
        //    }
        //    return pricePerOunce;
        //}
        //public List<Ingredient> queryDensityInfoTable() {
        //    var DensityInfo = queryItems("select * from densityInfo", reader => {
        //        var densityIngredientInformation = new Ingredient(reader["ingredient"].ToString());
        //        densityIngredientInformation.density = (decimal)reader["density"];
        //        return densityIngredientInformation;
        //    });
        //    return DensityInfo;
        //}
        //public void insertIngredientIntoDensityInfoDatabase(Ingredient i) {
        //    var rest = new MakeRESTCalls();
        //    var myDensityInfoTable = queryDensityInfoTable();
        //    if (myDensityInfoTable.Count() == 0)
        //        //insertDensityTextFileIntoDensityInfoDatabase(@"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
        //        insertDensityTextFileIntoDensityInfoDatabase();
        //    var myUpdatedDensityInfoTable = queryDensityInfoTable();
        //    var myMilkAndEggDensityInfoIngredients = new List<Ingredient>();
        //    foreach (var ingredient in myUpdatedDensityInfoTable) {
        //        if (ingredient.name.ToLower().Contains("milk") || ingredient.name.ToLower().Contains("egg"))
        //            myMilkAndEggDensityInfoIngredients.Add(ingredient);
        //    }
        //    var countSimilarIngredients = 0;
        //    foreach (var ingredient in myUpdatedDensityInfoTable) {
        //        if (i.typeOfIngredient.ToLower().Contains("milk") || i.typeOfIngredient.ToLower().Contains("egg")) {
        //            foreach (var dairyOrEggIngredient in myMilkAndEggDensityInfoIngredients) {
        //                if (i.typeOfIngredient == dairyOrEggIngredient.name) {
        //                    countSimilarIngredients++;
        //                    break;
        //                }
        //            }
        //            break;
        //        } else {
        //            if (rest.SimilaritesInStrings(i.typeOfIngredient, ingredient.name)) {
        //                countSimilarIngredients++;
        //                break;
        //            }
        //        }
        //    }
        //    if (countSimilarIngredients == 0) {
        //        var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
        //        executeVoidQuery(commandText, cmd => {
        //            cmd.Parameters.AddWithValue("@ingredient", i.typeOfIngredient);
        //            cmd.Parameters.AddWithValue("@density", i.density);
        //            return cmd;
        //        });
        //    }
        //    //all this is doing is determining if the density table already has an ingredient with said name, if so, then it won't add it, if the table doesn't have that name, it will insert it with the density
        //    var myDensityInfoDatabase = queryDensityInfoTable();
        //}
        //public List<Ingredient> assignIngredientDensityDictionaryValuesToListIngredients(Dictionary<string, decimal> myDensityIngredientDictionary) {
        //    var myIngredients = new List<Ingredient>();
        //    foreach (var pair in myDensityIngredientDictionary) {
        //        var currentIngredient = new Ingredient(pair.Key) {
        //            density = pair.Value
        //        };
        //        myIngredients.Add(currentIngredient);
        //    }
        //    return myIngredients;
        //}
        //public void insertDensityTextFileIntoDensityInfoDatabase() {
        //    //filename = @"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt";
        //    var read = new Reader(); //the filename below for the moment is hardcoded... 
        //    var DensityTextDatabaseDictionary = read.ReadDensityTextFile(@"C: \Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
        //    var myDensityTable = queryDensityInfoTable();
        //    var myDensityTableNames = new List<string>();
        //    foreach (var ingredient in myDensityTable)
        //        myDensityTableNames.Add(ingredient.name);
        //    //this is going to need to allow for user error and grace in the name... need to have a similaries check, or make sure the name.tolower contains the ingredient's name, as opposed to == it
        //    //i may have fixed this with the type of ingredient.... but i'll have to do more tests around that to see if it's intuitive
        //    foreach (var ingredient in DensityTextDatabaseDictionary) {
        //        if (!myDensityTableNames.Contains(ingredient.Key)) {
        //            var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
        //            executeVoidQuery(commandText, cmd => {
        //                cmd.Parameters.AddWithValue("@ingredient", ingredient.Key);
        //                cmd.Parameters.AddWithValue("@density", ingredient.Value);
        //                return cmd;
        //            });
        //        }
        //    }
        //    var myDensityTableAfter = queryDensityInfoTable();
        //}
        //public void insertListIntoDensityInfoDatabase(List<Ingredient> MyIngredients) {
        //    var read = new Reader(); //the filename below for the moment is hardcoded... but i would prefer to not keep it that way... bad business
        //    var myDensityTable = queryDensityInfoTable();
        //    var myDensityInfoTableIngredients = new List<string>();
        //    foreach (var ingredient in myDensityTable)
        //        myDensityInfoTableIngredients.Add(ingredient.typeOfIngredient);
        //    for (int i = 0; i < MyIngredients.Count(); i++) {
        //        if (!myDensityInfoTableIngredients.Contains(MyIngredients[i].typeOfIngredient)) {
        //            var commandText = @"Insert into densityInfo (ingredient, density) values (@ingredient, @density);";
        //            executeVoidQuery(commandText, cmd => {
        //                cmd.Parameters.AddWithValue("@ingredient", MyIngredients[i].name);
        //                cmd.Parameters.AddWithValue("@density", MyIngredients[i].density);
        //                return cmd;
        //            });
        //        }
        //    }
        //    var myDensityInfoTable = queryDensityInfoTable();
        //}
        //public void updateDensityInfoTable(Ingredient myIngredient) {
        //    var myDensityTableInfo = queryDensityInfoTable();
        //    var myDensityTableInfoNames = new List<string>();
        //    foreach (var ingredient in myDensityTableInfo)
        //        myDensityTableInfoNames.Add(ingredient.name);
        //    if (!myDensityTableInfoNames.Contains(myIngredient.typeOfIngredient))
        //        insertIngredientIntoDensityInfoDatabase(myIngredient);
        //    else {
        //        var commandText = @"Update densityInfo set density=@density where ingredient=@ingredient;";
        //        executeVoidQuery(commandText, cmd => {
        //            cmd.Parameters.AddWithValue("@ingredient", myIngredient.typeOfIngredient);
        //            cmd.Parameters.AddWithValue("@density", myIngredient.density);
        //            return cmd;
        //        });
        //    }
        //}
        //public decimal returnIngredientDensityFromDensityTable(Ingredient i) {
        //    var rest = new MakeRESTCalls();
        //    var myIngredients = queryIngredients();
        //    var myDensityIngredients = queryDensityInfoTable();
        //    var myIngredientDensity = 0m;
        //    foreach (var ingredient in myDensityIngredients) {
        //        if (rest.SimilaritesInStrings(i.typeOfIngredient, ingredient.name)) {
        //            myIngredientDensity = ingredient.density;
        //            break;
        //        }
        //    }
        //    return myIngredientDensity;
        //}
        //public void updateListOfIngredientsInDensityInfoTable(List<Ingredient> MyIngredients) {
        //    var myDensityTableInfo = queryDensityInfoTable();
        //    var myDensityTableInfoNames = new List<string>();
        //    foreach (var ingredient in myDensityTableInfo)
        //        myDensityTableInfoNames.Add(ingredient.name);
        //    for (int i = 0; i < MyIngredients.Count(); i++) {
        //        if (!myDensityTableInfoNames.Contains(MyIngredients[i].name))
        //            insertIngredientIntoDensityInfoDatabase(MyIngredients[i]);
        //        else
        //            updateDensityInfoTable(MyIngredients[i]);
        //    }
        //}

        ////initalize database tables
        //public void dropTableIfExists(string table) {
        //    var drop = @"IF OBJECT_ID('dbo." + table + " ', 'U') IS NOT NULL DROP TABLE dbo." + table + ";";
        //    executeVoidQuery(drop, a => a);
        //}
        //public void createDensityDatabase() {
        //    executeVoidQuery(@"create table densityInfo (
        //                ingredient nvarchar(max),
        //                density decimal(4,2)
        //                );", a => a);
        //}
        //public void initializeDatabase() {
        //    dropTableIfExists("recipes");
        //    executeVoidQuery(@"create table recipes (
        //                recipe_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
        //                name nvarchar(max), 
        //                yield int,
        //                aggregated_price decimal(5, 2)
        //             );", a => a);

        //    dropTableIfExists("ingredients");
        //    executeVoidQuery(@"create table ingredients (
        //                ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
        //                recipe_id Int,
        //                name nvarchar(max), 
        //                item_id int, 
        //                measurement nvarchar(max),
        //                ingredient_classification nvarchar(max),
        //                ingredient_type nvarchar(max),
        //                price_measured_ingredient decimal(6,2),
        //                item_response_name varchar(max),
        //                expiration_date nvarchar(25)
        //             );", a => a);
        //    dropTableIfExists("densities");
        //    executeVoidQuery(@"create table densities (
        //                ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
        //                name nvarchar(max), 
        //                density decimal (4,2),
        //                selling_weight varchar(250),
        //                selling_weight_ounces decimal(6,2),
        //                selling_price decimal(6,2),
        //                price_per_ounce decimal(8,4)
        //             );", a => a);
        //    dropTableIfExists("consumption");
        //    executeVoidQuery(@"create table consumption (
        //                id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        //                name varchar(max),
        //                density decimal (4,2),
        //                ounces_consumed decimal (5,2),
        //                ounces_remaining decimal(6,2),
        //                refill int default 0
        //             );", a => a);
        //    dropTableIfExists("costs");
        //    executeVoidQuery(@"create table costs (
        //                ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        //                name varchar (max),
        //                selling_weight varchar(max),
        //                selling_price decimal(6,2),
        //                price_per_ounce decimal (6,4),
        //                item_id int
        //            );", a => a);
        //    dropTableIfExists("densityInfo");
        //    executeVoidQuery(@"create table densityInfo (
        //                ing_id int,
        //                ingredient nvarchar(max),
        //                density decimal(4,2)
        //                );", a => a);
        //    dropTableIfExists("consumption_ounces_consumed");
        //    executeVoidQuery(@"create table consumption_ounces_consumed (
        //                ing_id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        //                name nvarchar(max), 
        //                measurement nvarchar(max),
        //                ounces_consumed decimal(5,2)
        //                );", a => a);
        //    //this ingredient name is to represent the ingredient.typeOfIngredient
        //    executeVoidQuery("SET IDENTITY_INSERT densities ON", cmd => cmd);
        //    insertDensityTextFileIntoDensityInfoDatabase();
        //}
    }
}
// read up on the Normal Forms of a relational database: e.g what is the 1st normal form and how do you do it