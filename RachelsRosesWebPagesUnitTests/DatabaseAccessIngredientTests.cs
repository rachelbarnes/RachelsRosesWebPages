using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RachelsRosesWebPages.Models;
using RachelsRosesWebPages;
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class DatbaseAccessIngredientTests {
        [Test]
        public void TestIngredientTable() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var i = new Ingredient("all-purpose flour", "2 1/2 cups") {
                recipeId = 1
            };
            var i2 = new Ingredient("butter", "1/2 cup") {
                recipeId = 1
            };
            var r = new Recipe("White Cake") {
                id = 1
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            dbI.insertIngredient(i, r);
            dbI.insertIngredient(i2, r);
            var recipes = dbR.queryRecipes();
            var ingredients = dbI.queryIngredients();
            var myIngredientBox = dbI.queryIngredients();
            Assert.AreEqual("all-purpose flour", ingredients[0].name);
            Assert.AreEqual("butter", ingredients[1].name);
        }
        [Test]
        public void TestInsertIngredientToIngredientDatabase() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("Cranberry Swirl Bread") {
                id = 1
            };
            var i = new Ingredient("Cranberries", "2 cups") {
                recipeId = r.id
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            dbI.insertIngredient(i, r);
            var myRecipeBox = dbR.queryRecipes();
            var myIngredientBox = dbI.queryIngredients();
            var myRecipe = dbR.GetFullRecipe(r);
            Assert.AreEqual(r.name, myRecipeBox[0].name);
            Assert.AreEqual(i.name, myIngredientBox[0].name);
            Assert.AreEqual(i.measurement, myIngredientBox[0].measurement);
            Assert.AreEqual(i.name, myRecipe.ingredients[0].name);
            Assert.AreEqual(i.measurement, myRecipe.ingredients[0].measurement);
        }
        [Test]
        public void TestUpdatingIngredients() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("Honey Buttermilk Bread");
            var i = new Ingredient("Flour", "6 cups") {
                recipeId = 1,
                ingredientId = 1
            };
            var i2 = new Ingredient("Bread Flour", "6 1/3 cups") {
                recipeId = 1,
                ingredientId = 1
            };
            i = i2;
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            dbI.insertIngredient(i, r);
            dbI.UpdateIngredient(i);
            var myIngredientBox = dbI.queryIngredients();
            Assert.AreEqual(i2.name, myIngredientBox[0].name);
            Assert.AreEqual(i2.measurement, myIngredientBox[0].measurement);
        }
        [Test]
        public void TestMeasuredIngredientPriceIngredientsTable() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCosts = new DatabaseAccessCosts();
            var dbD = new DatabaseAccessDensities();
            var rest = new MakeRESTCalls();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1
            };
            var i = new Ingredient("king arthur bread flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "6 cups",
                density = 5.4m,
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbC.insertIngredientConsumtionData(i);
            dbD.insertIngredientDensityData(i);
            dbCosts.insertIngredientCostDataCostTable(i);
            var IngredientMeasuredPrice = dbI.MeasuredIngredientPrice(i);
            var myIngInfo = dbCosts.queryCostTable();
            Assert.AreEqual(1.70m, IngredientMeasuredPrice);
        }
        [Test]
        public void TestMeasuredIngredientPriceIngredientsTablew() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCosts = new DatabaseAccessCosts();
            var dbD = new DatabaseAccessDensities();
            var rest = new MakeRESTCalls();
            var r = new Recipe("Wholesome Whole Wheat Bread") {
                id = 1
            };
            var i = new Ingredient("king arthur whole wheat flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "1 1/2 cups",
                density = 5.4m
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbC.insertIngredientConsumtionData(i);
            dbD.insertIngredientDensityData(i);
            dbCosts.insertIngredientCostDataCostTable(i);
            var IngredientMeasuredPrice = dbI.MeasuredIngredientPrice(i);
            var myIngInfo = dbCosts.queryCostTable();
            Assert.AreEqual(.43m, IngredientMeasuredPrice);
        }
        [Test]
        public void TestMeasuredIngredientPriceIngredientsTable2() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCosts = new DatabaseAccessCosts();
            var dbD = new DatabaseAccessDensities();
            var rest = new MakeRESTCalls();
            var r = new Recipe("Wholesome Whole Wheat Bread") {
                id = 1
            };
            var i = new Ingredient("King Arthur Whole Wheat Flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "3 cups",
                density = 5
            };
            var i2 = new Ingredient("Rumford Baking Powder") {
                recipeId = 1,
                ingredientId = 2,
                sellingWeight = "10 oz",
                measurement = "1 teaspoon",
                density = 8.4m
            };
            var i3 = new Ingredient("King Arthur All Purpose Flour") {
                recipeId = 1,
                ingredientId = 3,
                sellingWeight = "5 lb",
                measurement = "2 cups",
                density = 5
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbI.insertIngredient(i2, r);
            dbI.insertIngredient(i3, r);
            dbC.insertIngredientConsumtionData(i);
            dbC.insertIngredientConsumtionData(i2);
            dbC.insertIngredientConsumtionData(i3);
            dbD.insertIngredientDensityData(i);
            dbD.insertIngredientDensityData(i2);
            dbD.insertIngredientDensityData(i3);
            dbCosts.insertIngredientCostDataCostTable(i);
            dbCosts.insertIngredientCostDataCostTable(i2);
            dbCosts.insertIngredientCostDataCostTable(i3);
            var ingredientMeasuredPrice1 = dbI.MeasuredIngredientPrice(i);
            var ingredient2MeasuredPrice = dbI.MeasuredIngredientPrice(i2);
            var ingredient3MeasuredPrice = dbI.MeasuredIngredientPrice(i3);
            Assert.AreEqual(.79m, ingredientMeasuredPrice1);
            Assert.AreEqual(.04m, ingredient2MeasuredPrice);
            Assert.AreEqual(.46m, ingredient3MeasuredPrice);
        }
        [Test]
        public void TestHersheysUnsweetenedCocoa() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("Chocolate Something") {
                id = 1
            };
            var i = new Ingredient("Hershey's Special Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "8 oz",
                measurement = "1/2 cup",
                density = 4.16m
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            t.insertIngredientIntoAllTables(i, r);
            dbI.getIngredientMeasuredPrice(i, r);
            var myIngInfo = dbI.queryIngredients();
            var myRecipesInfo = dbR.queryRecipes();
            Assert.AreEqual(1, myRecipesInfo.Count());
            Assert.AreEqual(.87m, myIngInfo[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestMarshallowsPriceConsumed() {
            var t = new DatabaseAccess();
            var dbCosts = new DatabaseAccessCosts();
            var r = new Recipe("Rocky Road Brownies") {
                id = 1
            };
            var i = new Ingredient("Marshmallows") {
                recipeId = 1,
                ingredientId = 1,
                measurement = "2 cups",
                density = 4.67m,
                sellingWeight = "24 ounces"
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredient = t.queryAllTablesForIngredient(i);
            var myIngredientsCost = dbCosts.queryCostTable();
            Assert.AreEqual(2.98m, myIngredient.sellingPrice);
            Assert.AreEqual(1.16m, myIngredient.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestChocolateChips() {
            var dbD = new DatabaseAccessDensityInformation();
            var dbR = new DatabaseAccessRecipe();
            var t = new DatabaseAccess();
            var r = new Recipe("Chooalte Chip Cookies") { id = 1 };
            var i = new Ingredient("Semi Sweet Morsels") { ingredientId = 1, recipeId = 1, sellingWeight = "36 oz", density = 5.35m, measurement = "1 cup" };
            //6.98    1.04
            t.initializeDatabase();
            //var filename = @"C:\Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt";
            dbD.insertDensityTextFileIntoDensityInfoDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var mydensityDataInformation = dbD.queryDensityInfoTable();
            var semiSweetMorsels = t.queryAllTablesForIngredient(i);
            var myRecipes = dbR.MyRecipeBox();
            Assert.AreEqual("all purpose flour", mydensityDataInformation[0].name);
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(6.98m, semiSweetMorsels.sellingPrice);
            Assert.AreEqual(6.98m, myRecipes[0].ingredients[0].sellingPrice);
            Assert.AreEqual(1.04m, semiSweetMorsels.priceOfMeasuredConsumption);
            Assert.AreEqual(1.04m, myRecipes[0].ingredients[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestReturnPriceOfMeasuredIngredient() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("Bread") { id = 1 };
            var i = new Ingredient("King Arthur Bread Flour") { recipeId = 1, ingredientId = 1, measurement = "6 cups", sellingWeight = "5 lb", density = 5.4m };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myPriceOfMeasuredConsumption = dbI.returnIngredientMeasuredPrice(i);
            Assert.AreEqual(1.70m, myPriceOfMeasuredConsumption);
        }
        [Test]
        public void TestItemId() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("bread flour") { recipeId = 1, ingredientId = 1, measurement = "3 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = dbI.queryIngredients();
            var expected = 10308169;
            var actual = myIngredients[0].itemId;
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestItemId2() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("baking soda") { recipeId = 1, ingredientId = 1, sellingWeight = "4 lb", measurement = "1/2 teaspoon" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = dbI.queryIngredients();
            var expected = 11027507;
            var actual = myIngredients[0].itemId;
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGetListOfItemResponses() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var rest = new MakeRESTCalls();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("bread flour") { ingredientId = 1, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = dbI.queryIngredients();
            var listOfItemResponses = rest.GetListItemResponses(i);
            Assert.AreEqual(4, listOfItemResponses.Count());
            Assert.AreEqual(10308169, myIngredients[0].itemId);
            Assert.AreEqual(10308169, listOfItemResponses[0].itemId);
            Assert.AreEqual(true, listOfItemResponses[0].name.Contains(i.sellingWeight));
        }
        [Test]
        public void TestGettingUniqueEntriesINIngredientTable() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield = 12 };
            var chocolateCake = new Recipe("My Favorite Chocolat Cake") { id = 2, yield = 18 };
            var yellowCake = new Recipe("Yellow Cake") { id = 3, yield = 16 };
            var softasilk = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "2 cups 2 tablespoons", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilk2 = new Ingredient("Softasilk Cake Flour") { ingredientId = 2, recipeId = 2, measurement = "3 cups", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilk3 = new Ingredient("Softasilk Cake Flour") { ingredientId = 3, recipeId = 3, measurement = "1 1/2 cups", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 4, recipeId = 3, measurement = "2 teaspoons", typeOfIngredient = "baking powder", sellingWeight = "10 oz" };
            var yellowCakeIngredients = new List<Ingredient> { softasilk3, bakingPowder };
            var myIngredientBox = new List<Ingredient> { softasilk, softasilk2, softasilk3, bakingPowder };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(softasilk, fluffyWhiteCake);
            t.insertIngredientIntoAllTables(softasilk2, chocolateCake);
            t.insertListOfIngredientsIntoAllTables(yellowCakeIngredients, yellowCake);
            var myIngredientsTable = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            var myDistictIngredientTable = dbI.getListOfDistintIngredientsSorted();
            var myIngredientBoxFilled = t.queryAllTablesForAllIngredients(myIngredientBox);
            Assert.AreEqual(4, myIngredientsTable.Count());
            Assert.AreEqual(2, myDistictIngredientTable.Count());
            Assert.AreEqual("Softasilk Cake Flour", myDistictIngredientTable[0]);
            Assert.AreEqual("Baking Powder", myDistictIngredientTable[1]);
            Assert.AreEqual(.89m, softasilk.priceOfMeasuredConsumption);
            Assert.AreEqual(1.26m, softasilk2.priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, softasilk3.priceOfMeasuredConsumption);
            Assert.AreEqual(.10m, bakingPowder.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestConvertIntToDate() {
            var t = new DatabaseAccessIngredient();
            var expected = new DateTime(2017, 01, 17);
            var actual = t.convertIntToDate(20170117);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertStringToDate() {
            var t = new DatabaseAccessIngredient();
            var expected = new DateTime(2017, 04, 16);
            var actual = t.convertStringToDateYYYYMMDD("2017.04.16");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertStringToDate2() {
            var t = new DatabaseAccessIngredient();
            var expected = new DateTime(2017, 04, 16);
            var actual = t.convertStringToDateYYYYMMDD("2017-04-16");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertStringToDate3() {
            var t = new DatabaseAccessIngredient();
            var expected = new DateTime(2017, 04, 16);
            var actual = t.convertStringToDateYYYYMMDD("2017/04/16");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestCovnertStringtoDate4() {
            var t = new DatabaseAccessIngredient();
            var expected = new DateTime(2017, 04, 16);
            var actual = t.convertStringToDateYYYYMMDD("20170416");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertDateToString() {
            var expected = "4/16/2017";
            var actual = new DateTime(2017, 04, 16).Date.ToString();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertDateToString2() {
            var t = new DatabaseAccessIngredient();
            var expected = "04/16/2017";
            var actual = t.convertDateToStringMMDDYYYY(new DateTime(2017, 04, 16));
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertDateToString3() {
            var t = new DatabaseAccessIngredient();
            var expected = "12/12/2015";
            var actual = t.convertDateToStringMMDDYYYY(new DateTime(2015, 12, 12));
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertDateToString4() {
            var t = new DatabaseAccessIngredient();
            var expected = "01/01/0001";
            var actual = t.convertDateToStringMMDDYYYY(new DateTime());
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestingDateTimeParameters() {
            var expected = "";//this is just a test to look at the output from converting the date to a string
            var actual = new DateTime(2017, 1, 2).ToString();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSortingListOfIngredients() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 12 };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips", classification = "baking chocolate" };
            var bakingCocoa = new Ingredient("Unsweetened Cocoa") { ingredientId = 2, recipeId = 1, measurement = "1 cup", sellingWeight = "16 oz", typeOfIngredient = "baking cocoa", classification = "baking chocolate" };
            var milk = new Ingredient("Whole Milk") { ingredientId = 3, recipeId = 1, measurement = "2 cups", sellingWeight = "1/2 gallon", sellingPrice = 1.79m, typeOfIngredient = "milk", classification = "dairy" };
            var chocolateCakeIngredients = new List<Ingredient>() { chocolateChips, bakingCocoa, milk };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            var myIngredientBox = dbI.myIngredientBox();
            Assert.AreEqual("Semi Sweet Chocolate Chips", myIngredientBox[0].name);
            Assert.AreEqual("Unsweetened Cocoa", myIngredientBox[1].name);
            Assert.AreEqual("Whole Milk", myIngredientBox[2].name);
        }
        [Test]
        public void TestSortingListOfStrings() {
            var expected = new List<string> { "b", "g", "c", "f", "a", "r" };
            var actual = new List<string> { "a", "b", "c", "f", "g", "r" };
            expected.Sort();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSortingListOfIngredients3() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 12 };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Morsels") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips", classification = "baking chocolate" };
            var bakingCocoa = new Ingredient("Unsweetened Cocoa") { ingredientId = 2, recipeId = 1, measurement = "1 cup", sellingWeight = "8 oz", typeOfIngredient = "baking cocoa", classification = "baking chocolate" };
            var milk = new Ingredient("Whole Milk") { ingredientId = 3, recipeId = 1, measurement = "2 cups", sellingWeight = "1/2 gallon", sellingPrice = 1.79m, typeOfIngredient = "milk", classification = "dairy", expirationDate = new DateTime(2017, 2, 15)};
            var eggs = new Ingredient("Eggs") { ingredientId = 4, recipeId = 1, measurement = "2 eggs", sellingWeight = "1 dozen", sellingPrice = 2.50m, typeOfIngredient = "egg", classification = "eggs", expirationDate = new DateTime(2017, 4, 4)};
            var salt = new Ingredient("Salt") { ingredientId = 5, recipeId = 1, measurement = "1 teapsoon", sellingWeight = "48 oz", typeOfIngredient = "salt", classification = "salt" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 6, recipeId = 1, measurement = "2 teaspoons", sellingWeight = "10 oz", typeOfIngredient = "baking powder", classification = "rising agent" };
            //var chocolateCakeIngredients = new List<Ingredient>() { eggs, salt, bakingPowder, chocolateChips, bakingCocoa, milk };
            var chocolateCakeIngredients = new List<Ingredient>() { chocolateChips, bakingCocoa, milk, eggs, salt, bakingPowder }; 
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            var uniqueIngredientNames = dbI.getListOfDistintIngredientsSorted(); 
            Assert.AreEqual("Baking Powder", uniqueIngredientNames[0]);
            Assert.AreEqual("Eggs", uniqueIngredientNames[1]);
            Assert.AreEqual("Salt", uniqueIngredientNames[2]);
            Assert.AreEqual("Semi Sweet Chocolate Morsels", uniqueIngredientNames[3]);
            Assert.AreEqual("Unsweetened Cocoa", uniqueIngredientNames[4]);
            Assert.AreEqual("Whole Milk", uniqueIngredientNames[5]);
        }
        [Test]
        public void TestSimilarNamesInDatabase() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var HoneyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var CinnamonSwirlBread = new Recipe("Cinnamon Swirl Buttermilk Bread") { id = 2, yield = 24 };
            var breadFlour1 = new Ingredient("King Arthur Bread Flour") { ingredientId = 1, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour", classification = "flour" };
            var breadFlour2 = new Ingredient("King Arthur Bread Flour") { ingredientId = 2, recipeId = 2, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour", classification = "flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(breadFlour1, HoneyButtermilkBread);
            t.insertIngredientIntoAllTables(breadFlour2, CinnamonSwirlBread);
            var myIngredientBox = dbI.myIngredientBox();
            Assert.AreEqual(2, myIngredientBox.Count());
        }
        [Test]
        public void TestDeleteIngredientFromIngredientsTableDeleteOnlyFromIngredientTable() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCosts = new DatabaseAccessCosts();
            var dbDI = new DatabaseAccessDensityInformation();
            var dbD = new DatabaseAccessDensities();
            var HoneyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var breadFlour = new Ingredient("King Arthur Bread Flour") { ingredientId = 1, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour", classification = "flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(breadFlour, HoneyButtermilkBread);
            var myIngredientBox = dbI.myIngredientBox();
            dbI.DeleteIngredientFromIngredientTable(breadFlour);
            var myIngredientTableCount = dbI.queryIngredients().Count();
            var myIngredientBoxCount = dbI.myIngredientBox().Count();
            var myCostTableCount = dbCosts.queryCostTable().Count();
            var myDensitiesTableCount = dbD.queryDensitiesTable().Count();
            var myDensityInformationTableCount = dbDI.queryDensityInfoTable().Count();
            var myConsumptionTable = dbC.queryConsumptionTable().Count();
            Assert.AreEqual(0, myIngredientTableCount);
            Assert.AreEqual(1, myCostTableCount);
            Assert.AreEqual(1, myDensitiesTableCount);
            Assert.AreEqual(1, myConsumptionTable);
        }
        [Test]
        public void TestDeleteIngredientFromIngredientsTableAndAccessOtherTablesForDeletedIngredient() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCosts = new DatabaseAccessCosts();
            var dbD = new DatabaseAccessDensities();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var cinnamonButtermilkBread = new Recipe("Cinnamon Buttermilk Bread") { id = 2, yield = 24 };
            var breadFlour1 = new Ingredient("King Arthur Bread Flour") { ingredientId = 1, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour", classification = "flour" };
            var breadFlour2 = new Ingredient("King Arthur Bread Flour") { ingredientId = 2, recipeId = 2, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour", classification = "flour" };
            var salt = new Ingredient("Salt") { ingredientId = 3, recipeId = 2, measurement = "1 tablespoon", sellingWeight = "48 oz", typeOfIngredient = "salt", classification = "salt" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(breadFlour1, honeyButtermilkBread);
            t.insertIngredientIntoAllTables(breadFlour2, cinnamonButtermilkBread);
            t.insertIngredientIntoAllTables(salt, cinnamonButtermilkBread);
            var myIngredientBox = dbI.myIngredientBox();
            dbI.DeleteIngredientFromIngredientTable(breadFlour1);
            dbI.DeleteIngredientFromIngredientTable(breadFlour2);
            var myIngredientTable = dbI.queryIngredients();
            var myCostTable = dbCosts.queryCostTable();
            var myDensitiesTable = dbD.queryDensitiesTable();
            var myConsumptionTable = dbC.queryConsumptionTable();
            Assert.AreEqual(1, myIngredientTable.Count());
            Assert.AreEqual("Salt", myIngredientTable[0].name);
            Assert.AreEqual(3, myCostTable.Count());
            Assert.AreEqual(3, myDensitiesTable.Count());
            Assert.AreEqual(2, myConsumptionTable.Count());
        }
        [Test]
        public void TestDeleteIngredientBasedOnNameAndMeasurement() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 1, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour", classification = "flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(breadFlour, honeyButtermilkBread);
            dbI.DeleteIngredientFromIngredientTable(breadFlour);
            var myIngredientTable = dbI.queryIngredients();
            Assert.AreEqual(0, myIngredientTable.Count());
        }
        //write a test for the order by method for my ingredient names... 
            //multipe ingredients of the same name and make sure they're in the right order

        //need to figure out what's going on with the priceOfMeasuredConsumption for the other recipe tests... i was having trouble with this on my browser too... 
            //it's not getting transfered somewhere... if I can get the query all tables for ingredients done with joins, either left or inner joins or even using the outter joins, then that
                //would be benefical for tomorrow, plus muscle and brain memory

        //also do the order bys for selling weights, densities, types, and ingredients by both name and by object with LINQ and SQL
    }
}
