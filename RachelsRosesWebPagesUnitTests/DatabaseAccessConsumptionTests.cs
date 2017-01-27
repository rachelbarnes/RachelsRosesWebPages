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
    class DatabaseAccessConsumptionTests {
        [Test]
        public void testInsertionIntoConsumptionDatabase() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var i = new Ingredient("Butter") {
                ingredientId = 1,
                density = 8m,
                ouncesConsumed = 6m,
                ouncesRemaining = 2m
            };
            t.initializeDatabase();
            dbC.insertIngredientConsumtionData(i);
            var myIngInfo = dbC.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.name, myIngInfo[0].name);
            Assert.AreEqual(i.ouncesConsumed, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(i.ouncesRemaining, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void testInsertionIntoConsumptionDatabase2() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var r = new Recipe("Wholesome Whole Wheat Bread") {
                id = 1
            };
            var i = new Ingredient("Butter") {
                recipeId = 1,
                ingredientId = 1,
                density = 8m,
                ouncesConsumed = 6m,
                ouncesRemaining = (6m - 8m),
                sellingWeight = "1 lb",
                typeOfIngredient = "butter"
            };
            var i2 = new Ingredient("Whole Wheat Flour") {
                recipeId = 1,
                ingredientId = 2,
                density = 4.5m,
                ouncesConsumed = 13.5m,
                ouncesRemaining = (13.5m - 80m),
                //this is to test negative numbers... making sure they're doing what I expect them to
                sellingWeight = "5 lb",
                typeOfIngredient = "whole wheat flour"
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbI.insertIngredient(i2, r);
            dbC.insertIngredientConsumtionData(i);
            dbC.insertIngredientConsumtionData(i2);
            var myIngInfo = dbC.queryConsumptionTable();
            Assert.AreEqual(2, myIngInfo.Count());
            //Assert.AreEqual(i.ingredientId, myIngInfo[0].ingredientId);
            Assert.AreEqual(i.name, myIngInfo[0].name);
            Assert.AreEqual(i.ouncesConsumed, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(i.ouncesRemaining, myIngInfo[0].ouncesRemaining);
            Assert.AreEqual(i2.name, myIngInfo[1].name);
            Assert.AreEqual(i2.ouncesConsumed, myIngInfo[1].ouncesConsumed);
            Assert.AreEqual(i2.ouncesRemaining, myIngInfo[1].ouncesRemaining);
        }
        [Test]
        public void TestInsertIntoConsumtionTable() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                measurement = "2 cups",
                recipeId = 1,
                density = 5.4m,
                ouncesRemaining = 80m,
                sellingWeight = "5 lb",
                typeOfIngredient = "bread flour"
            };
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbC.insertIngredientConsumtionData(i);
            var myIngInfo = dbC.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(10.8m, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(69.2m, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestUpdateConsumptionTable2() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                density = 4.5m,
                ouncesConsumed = 27m
            };
            t.initializeDatabase();
            dbC.insertIngredientConsumtionData(i);
            var remainingOunces = i.ouncesConsumed - 80m;
            i.ouncesRemaining = remainingOunces;
            dbC.updateConsumptionTable(i);
            var myIngInfo = dbC.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.density, myIngInfo[0].density);
            Assert.AreEqual(remainingOunces, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestCalculatedOuncesUsedFromGivenMeasurments() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var i = new Ingredient("Grandulated Sugar") {
                recipeId = 1,
                ingredientId = 1,
                measurement = "1/3 cup",
                density = 7.1m,
                ouncesRemaining = 80m,
                sellingWeight = "5 lb"
            };
            var r = new Recipe("Sugar Cookies") {
                id = 1
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbC.insertIngredientConsumtionData(i);
            dbC.updateConsumptionTable(i);
            //after this refactoring, i want to have all of this updating ingredients for the initial calculated ounces consumed and such to be in the insert ingredient to the consumption table!!
            var myIngInfo = dbC.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(2.37m, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(77.63m, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestCalculatedOuncesConsumedFromMeasurmeent() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                measurement = "6 cups",
                density = 5.4m,
                ouncesRemaining = 80m
            };
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1
            };
            t.initializeDatabase();
            dbI.insertIngredient(i, r);
            dbC.insertIngredientConsumtionData(i);
            var myIngInfo = dbC.queryConsumptionTable();
            Assert.AreEqual(32.4m, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(47.6m, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestDeleteIngredeintFromConsumptionTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbR = new DatabaseAccessRecipe();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = dbR.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            dbC.DeleteIngredientFromConsumptionTable(honey);
            var myConsumptionTable = dbC.queryConsumptionTable();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myConsumptionTable.Count());
        }
        [Test]
        public void TestItemResponseInformationInConsumptionTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbR = new DatabaseAccessRecipe();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, sellingWeight = "32 oz", measurement = "2 tablespoons", typeOfIngredient = "honey" };
            //i would eventually like for this to be a drop down menu, to show all options for the typeOfIngredient in the density table}
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = dbR.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            var myConsumptionTable = dbC.queryConsumptionTable();
            Assert.AreEqual(.37m, myIngredient.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestOuncesRemainingAndConsumedInConsumptionTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, sellingWeight = "32 oz", measurement = "1/3 cup", typeOfIngredient = "honey" };
            var breadFlour = new Ingredient("bread flour") { ingredientId = 2, recipeId = 1, sellingWeight = "5 lb", measurement = "6 cups", typeOfIngredient = "bread flour" };
            var breadIngredients = new List<Ingredient> { honey, breadFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(breadIngredients, bread);
            var myConsumptionTable = dbC.queryConsumptionTable();
            var allTablesForHoney = t.queryAllTablesForIngredient(honey);
            var allTablesForBreadFlour = t.queryAllTablesForIngredient(breadFlour);
            Assert.AreEqual("honey", myConsumptionTable[0].name);
            Assert.AreEqual(32m, allTablesForHoney.sellingWeightInOunces);
            Assert.AreEqual(28m, myConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(4m, myConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual("bread flour", myConsumptionTable[1].name);
            Assert.AreEqual(80m, allTablesForBreadFlour.sellingWeightInOunces);
            Assert.AreEqual(47.6m, myConsumptionTable[1].ouncesRemaining);
            Assert.AreEqual(32.4m, myConsumptionTable[1].ouncesConsumed);
        }
        [Test]
        public void Test2IngredientsInConsumptionTableWithSameName() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbR = new DatabaseAccessRecipe();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1 };
            var cinnamonSwirlBread = new Recipe("Cinnamon Swirl Bread") { id = 2 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var honey2 = new Ingredient("honey") { ingredientId = 2, recipeId = 2, measurement = "1 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var honeyListOfIngredients = new List<Ingredient> { honey, honey2 };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, honeyButtermilkBread);
            var honeyIngInfo = dbC.queryConsumptionTable();
            t.insertIngredientIntoAllTables(honey2, cinnamonSwirlBread);
            var honey2IngInfo = dbC.queryConsumptionTable();
            var myRecipes = dbR.MyRecipeBox();
            var honeyAllTablesInfo = t.queryAllTablesForIngredient(honey);
            var honey2AllTablesInfo = t.queryAllTablesForIngredient(honey2);
            var myIngredientBox = t.queryAllTablesForAllIngredients(honeyListOfIngredients);
            var myConsumptionTable = dbC.queryConsumptionTable();
            Assert.AreEqual(32m, myIngredientBox[0].sellingWeightInOunces);
            Assert.AreEqual(32m, myIngredientBox[1].sellingWeightInOunces);
            Assert.AreEqual(28m, honeyIngInfo[0].ouncesRemaining);
            Assert.AreEqual(4m, honeyIngInfo[0].ouncesConsumed);
            Assert.AreEqual(16m, honey2IngInfo[0].ouncesRemaining);
            Assert.AreEqual(12m, honey2IngInfo[0].ouncesConsumed);
            Assert.AreEqual(16m, myConsumptionTable[0].ouncesRemaining);
        }
        [Test]
        public void TestMultipleIngredientsRepeatedInConsumptionTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbR = new DatabaseAccessRecipe();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1 };
            var cinnamonSwirlBread = new Recipe("Cinnamon Swirl Bread") { id = 2 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 2, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var salt = new Ingredient("Salt") { ingredientId = 3, recipeId = 1, measurement = "1 teaspoon", sellingWeight = "48 oz", typeOfIngredient = "salt" };
            var honey2 = new Ingredient("Honey") { ingredientId = 4, recipeId = 2, measurement = "1 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var breadFlour2 = new Ingredient("bread flour") { ingredientId = 5, recipeId = 2, measurement = "2 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var salt2 = new Ingredient("salt") { ingredientId = 6, recipeId = 6, measurement = "1 tablespoon", sellingWeight = "48 oz", typeOfIngredient = "salt" };
            //very interesting... with the way I have this set up with the SQL Database and having hte ing_id being the key column and not having it be able to write in... I've assigned this differently and
            //long story short, make sure to put the ingredients either in the correct order that you're entering them in the database above when you're defining them, or enter them in the particular manner that they're defined.
            //this will reset your ingredient ids... 
            var honeyBreadIngredients = new List<Ingredient> { honey, breadFlour, salt };
            var cinnBreadIngredients = new List<Ingredient> { honey2, breadFlour2, salt2 };
            t.initializeDatabase();
            var myHoneyButtermilkBreadIngredients = t.queryAllTablesForAllIngredients(honeyBreadIngredients);
            t.insertListOfIngredientsIntoAllTables(honeyBreadIngredients, honeyButtermilkBread);
            var myConsumptionTable = dbC.queryConsumptionTable();
            t.insertListOfIngredientsIntoAllTables(cinnBreadIngredients, cinnamonSwirlBread);
            var myUpdatedConsumptionTable = dbC.queryConsumptionTable();
            var myCinnamonSwirlBreadIngredients = t.queryAllTablesForAllIngredients(cinnBreadIngredients);
            var myRecipes = dbR.MyRecipeBox();
            var honeyBreadRecipeIngredients = t.queryAllTablesForAllIngredients(honeyBreadIngredients);
            var cinnBreadRecipeIngredients = t.queryAllTablesForAllIngredients(cinnBreadIngredients);
            Assert.AreEqual(32m, honeyBreadRecipeIngredients[0].sellingWeightInOunces);
            Assert.AreEqual(80m, honeyBreadRecipeIngredients[1].sellingWeightInOunces);
            Assert.AreEqual(48m, honeyBreadRecipeIngredients[2].sellingWeightInOunces);
            Assert.AreEqual(32m, cinnBreadRecipeIngredients[0].sellingWeightInOunces);
            Assert.AreEqual(80m, cinnBreadRecipeIngredients[1].sellingWeightInOunces);
            Assert.AreEqual(48m, cinnBreadRecipeIngredients[2].sellingWeightInOunces);
            //----
            Assert.AreEqual(4m, myConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual(28m, myConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(47.6m, myConsumptionTable[1].ouncesRemaining);
            Assert.AreEqual(32.4m, myConsumptionTable[1].ouncesConsumed);
            Assert.AreEqual(.22m, myConsumptionTable[2].ouncesConsumed);
            Assert.AreEqual(47.78m, myConsumptionTable[2].ouncesRemaining);
            Assert.AreEqual(12m, myUpdatedConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual(16m, myUpdatedConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(10.8m, myUpdatedConsumptionTable[1].ouncesConsumed);
            Assert.AreEqual(36.8m, myUpdatedConsumptionTable[1].ouncesRemaining);
            Assert.AreEqual(.67m, myUpdatedConsumptionTable[2].ouncesConsumed);
            Assert.AreEqual(47.11m, myUpdatedConsumptionTable[2].ouncesRemaining);
        }
        [Test]
        public void TestMultipleUsesOfOneIngredientInConsumptionDatabase() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbR = new DatabaseAccessRecipe();
            var bread = new Recipe("Bread") { id = 1 };
            var bread2 = new Recipe("Bread2") { id = 2 };
            var bread3 = new Recipe("Bread3") { id = 3 };
            var bread4 = new Recipe("Bread4") { id = 4 };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 1, recipeId = 1, measurement = "4 cups 2 tablespoons", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var breadFlour2 = new Ingredient("Bread Flour") { ingredientId = 2, recipeId = 2, measurement = "1/4 cup", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var breadFlour3 = new Ingredient("Bread Flour") { ingredientId = 3, recipeId = 3, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var breadFlour4 = new Ingredient("bread flour") { ingredientId = 4, recipeId = 4, measurement = "2 tablespoons", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(breadFlour, bread);
            var consumptionTable1 = dbC.queryConsumptionTable();
            t.insertIngredientIntoAllTables(breadFlour2, bread2);
            var consumptionTable2 = dbC.queryConsumptionTable();
            t.insertIngredientIntoAllTables(breadFlour3, bread3);
            var consumptionTable3 = dbC.queryConsumptionTable();
            t.insertIngredientIntoAllTables(breadFlour4, bread4);
            var consumptionTable4 = dbC.queryConsumptionTable();
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(1, consumptionTable1.Count());
            Assert.AreEqual(1, consumptionTable2.Count());
            Assert.AreEqual(1, consumptionTable3.Count());
            Assert.AreEqual(1, consumptionTable4.Count());
            Assert.AreEqual(80m, myRecipeBox[0].ingredients[0].sellingWeightInOunces);
            Assert.AreEqual(80m, myRecipeBox[1].ingredients[0].sellingWeightInOunces);
            Assert.AreEqual(80m, myRecipeBox[2].ingredients[0].sellingWeightInOunces);
            Assert.AreEqual(80m, myRecipeBox[3].ingredients[0].sellingWeightInOunces);
            Assert.AreEqual(22.28m, consumptionTable1[0].ouncesConsumed);
            Assert.AreEqual(1.35m, consumptionTable2[0].ouncesConsumed);
            Assert.AreEqual(32.4m, consumptionTable3[0].ouncesConsumed);
            Assert.AreEqual(.68m, consumptionTable4[0].ouncesConsumed);
            Assert.AreEqual(57.72m, consumptionTable1[0].ouncesRemaining);
            Assert.AreEqual(56.37m, consumptionTable2[0].ouncesRemaining);
            Assert.AreEqual(23.97m, consumptionTable3[0].ouncesRemaining);
            Assert.AreEqual(23.29m, consumptionTable4[0].ouncesRemaining);
            Assert.AreEqual(23.29m, myRecipeBox[0].ingredients[0].ouncesRemaining);
        }
        [Test]
        public void TestConsumptionRefill() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var chocolateChipCookies = new Recipe("Chocolate Chip Cookies") { id = 1 };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "1 3/4 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(chocolateChips, chocolateChipCookies);
            var myConsumptionTable = dbC.queryConsumptionTable();
            var myIngredientData = t.queryAllTablesForIngredient(chocolateChips);
            dbC.refillIngredientInConsumptionDatabase(chocolateChips, "24 oz");
            var myUpdatedConsumptionTable = dbC.queryConsumptionTable();
            var myUpdatedIngredientData = t.queryAllTablesForIngredient(chocolateChips);
            Assert.AreEqual(12m, myIngredientData.sellingWeightInOunces);
            Assert.AreEqual(9.36m, myIngredientData.ouncesConsumed);
            Assert.AreEqual(9.36m, myConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual(2.64m, myConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(9.36m, myUpdatedIngredientData.ouncesConsumed);
            Assert.AreEqual(9.36m, myUpdatedConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual(26.64m, myUpdatedConsumptionTable[0].ouncesRemaining);
        }
        [Test]
        public void TestDairyConsumptionDairy() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1 };
            var buttermilk = new Ingredient("Buttermilk") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "1 quart", typeOfIngredient = "buttermilk", sellingPrice = 1.79m, classification = "dairy" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(buttermilk, honeyButtermilkBread);
            var myConsumptionTable = dbC.queryConsumptionTable();
            var myIngredientData = t.queryAllTablesForIngredient(buttermilk);
            Assert.AreEqual(8.2m, myIngredientData.density);
            Assert.AreEqual(1.79m, myIngredientData.sellingPrice);
            Assert.AreEqual(32m, myIngredientData.sellingWeightInOunces);
            Assert.AreEqual(.0559m, myIngredientData.pricePerOunce);
            Assert.AreEqual(16.4m, myConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual(15.6m, myConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(.92m, myIngredientData.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestInsertEggIntoAllTables() {
            var t = new DatabaseAccess();
            var chocolateChipCookies = new Recipe("Chocolate Chip Cookies") { id = 1 };
            var eggs = new Ingredient("Eggs") { ingredientId = 1, recipeId = 1, classification = "eggs", measurement = "2 eggs", sellingPrice = 2.99m, sellingWeight = "1 dozen", typeOfIngredient = "egg" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(eggs, chocolateChipCookies);
            var myEggIngredientData = t.queryAllTablesForIngredient(eggs);
            Assert.AreEqual(1, myEggIngredientData.ingredientId);
            Assert.AreEqual(1, myEggIngredientData.recipeId);
            Assert.AreEqual("eggs", myEggIngredientData.classification);
            Assert.AreEqual("2 eggs", myEggIngredientData.measurement);
            Assert.AreEqual(2.99m, myEggIngredientData.sellingPrice);
            Assert.AreEqual(12m, myEggIngredientData.sellingWeightInOunces);
            Assert.AreEqual("1 dozen", myEggIngredientData.sellingWeight);
            Assert.AreEqual(.5m, myEggIngredientData.priceOfMeasuredConsumption);
            Assert.AreEqual(.2492m, myEggIngredientData.pricePerOunce);
        }
        [Test]
        public void TestEggs2() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe(); 
            var fluffyWhiteCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var eggWhites = new Ingredient("egg whites, stiffly beaten") { ingredientId = 1, recipeId = 1, measurement = "3 egg whites", sellingWeight = "1 dozen", sellingPrice = 2.99m, typeOfIngredient = "egg", classification = "egg" }; //for the record, one egg white merginued does not equal 1.70 oz to my knowledge.. should be decently lighter
            var eggs = new Ingredient("Eggs") { ingredientId = 2, recipeId = 1, measurement = "2 eggs", sellingWeight = "1 dozen", sellingPrice = 2.99m, typeOfIngredient = "egg", classification = "egg" };
            var fluffyWhiteCakeEggIngredients = new List<Ingredient> { eggWhites, eggs };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeEggIngredients, fluffyWhiteCake);
            var myIngredients = t.queryAllTablesForAllIngredients(fluffyWhiteCakeEggIngredients);
            var myRecipes = dbR.MyRecipeBox();
            Assert.AreEqual(.2492m, myIngredients[0].pricePerOunce);
            Assert.AreEqual(.75m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.2492m, myIngredients[1].pricePerOunce);
            Assert.AreEqual(.5m, myIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(2, myIngredients[1].ouncesConsumed);
            Assert.AreEqual(7, myIngredients[1].ouncesRemaining);
            Assert.AreEqual(1.25m, myRecipes[0].aggregatedPrice);
            Assert.AreEqual(.10m, myRecipes[0].pricePerServing);
        }
        [Test]
        public void TestEggs3() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe(); 
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield = 16 };
            var yellowCake = new Recipe("Yellow Cake") { id = 2, yield = 24 };
            var chocolateBananaBread = new Recipe("Chocolate Banana Bread") { id = 3, yield = 12 };
            var eggWhites = new Ingredient("Egg whites, meringued") { ingredientId = 1, recipeId = 1, classification = "egg", typeOfIngredient = "egg", measurement = "4 egg, stiffy beaten", sellingWeight = "2 dozen", sellingPrice = 3.5m };
            var softasilk = new Ingredient("Softasilk Cake Flour") { ingredientId = 2, recipeId = 1, typeOfIngredient = "cake flour", measurement = "2 cups 2 tablespoons", sellingWeight = "32 oz" };
            var eggs = new Ingredient("Eggs") { ingredientId = 3, recipeId = 2, classification = "egg", typeOfIngredient = "egg", measurement = "2 eggs", sellingWeight = "2 dozen", sellingPrice = 3.5m };
            var vanillaExtract = new Ingredient("Vanilla Extract") { ingredientId = 4, recipeId = 2, typeOfIngredient = "vanilla extract", measurement = "1 tablespoon", sellingWeight = "16 oz" };
            var eggs2 = new Ingredient("Eggs, slightly beaten") { ingredientId = 5, recipeId = 3, classification = "egg", typeOfIngredient = "egg", measurement = "2 eggs", sellingWeight = "2 dozen", sellingPrice = 3.5m };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Morsels") { ingredientId = 6, recipeId = 3, typeOfIngredient = "chocolate chips", measurement = "2 cups", sellingWeight = "12 oz" };
            var fluffyWhiteCakeIngredients = new List<Ingredient> { eggWhites, softasilk };
            var yellowCakeIngredients = new List<Ingredient> { eggs, vanillaExtract };
            var chocolateBananaBreadIngredients = new List<Ingredient> { eggs2, chocolateChips };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeIngredients, fluffyWhiteCake);
            t.insertListOfIngredientsIntoAllTables(yellowCakeIngredients, yellowCake);
            t.insertListOfIngredientsIntoAllTables(chocolateBananaBreadIngredients, chocolateBananaBread);
            var myFWCIngredientBox = t.queryAllTablesForAllIngredients(fluffyWhiteCakeIngredients);
            var myYCIngredients = t.queryAllTablesForAllIngredients(yellowCakeIngredients);
            var myCBBIngredients = t.queryAllTablesForAllIngredients(chocolateBananaBreadIngredients);
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(.58m, myFWCIngredientBox[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.89m, myFWCIngredientBox[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.29m, myYCIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.74m, myYCIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.29m, myCBBIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(3.17m, myCBBIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.47m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(.09m, myRecipeBox[0].pricePerServing);
            Assert.AreEqual(1.03m, myRecipeBox[1].aggregatedPrice);
            Assert.AreEqual(.04m, myRecipeBox[1].pricePerServing);
            Assert.AreEqual(3.46m, myRecipeBox[2].aggregatedPrice);
            Assert.AreEqual(.29m, myRecipeBox[2].pricePerServing);
            Assert.AreEqual(2m, myCBBIngredients[0].ouncesConsumed);
            Assert.AreEqual(16m, myCBBIngredients[0].ouncesRemaining);
        }
        [Test]
        public void TestgetDoubleAverageOuncesConsumedForIngredient() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var bananaNutMuffins = new Recipe("Banana Nut Muffins") { id = 1, yield = 18 };
            var APFlour = new Ingredient("All Purpose Flour") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "5 lb", typeOfIngredient = "all purpose flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(APFlour, bananaNutMuffins);
            var doubleAverage = dbC.doubleAverageOuncesConsumed(APFlour);
            Assert.AreEqual(15m, doubleAverage);
        }
        [Test]
        public void TestgetDoubleAverageOuncesConsumedForIngredient2() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed(); 
            var chocolateChipCheesecake = new Recipe("Chocolate Chip Cheesecake") { id = 1, yield = 18 };
            var chocolateChipCookies = new Recipe("Chocolate Chip Cookies") { id = 2, yield = 24 };
            var bananaChocolateChipMuffins = new Recipe("Banana Chocolate Chip Cookies") { id = 3, yield = 12 };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "1 3/4 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            var chocolateChips2 = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 2, recipeId = 2, measurement = "1/2 cup", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            var chocolateChips3 = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 3, recipeId = 3, measurement = "1 1/2 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 4, recipeId = 3, measurement = "2 teapsoons", sellingWeight = "10 oz", typeOfIngredient = "baking powder" };
            var bananaChocolateChipMuffinsIngredients = new List<Ingredient> { chocolateChips3, bakingPowder };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(chocolateChips, chocolateChipCheesecake);
            t.insertIngredientIntoAllTables(chocolateChips2, chocolateChipCookies);
            t.insertListOfIngredientsIntoAllTables(bananaChocolateChipMuffinsIngredients, bananaChocolateChipMuffins);
            var myIngredientConsumptionInformation = dbCOC.queryConsumptionOuncesConsumed();
            var doubleAverage = dbC.doubleAverageOuncesConsumed(chocolateChips);
            Assert.AreEqual(9.36m, myIngredientConsumptionInformation[0].ouncesConsumed);
            Assert.AreEqual(2.68m, myIngredientConsumptionInformation[1].ouncesConsumed);
            Assert.AreEqual(8.02m, myIngredientConsumptionInformation[2].ouncesConsumed);
            Assert.AreEqual(13.37m, doubleAverage);
            //something is off with getting the ounces consumed... i may have to match more than the name and measurement? 
            //the name and measurement should be the only thigns i need... debug and see where it's picking up only the first one again
        }
        [Test]
        public void TestgetDoubleAverageOuncesConsumedForIngredient3() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed(); 
            var cakeFlour = new Ingredient("Cake Flour") { ingredientId = 1, ouncesConsumed = 10m };
            var cakeFlour2 = new Ingredient("Cake Flour") { ingredientId = 2, ouncesConsumed = 20m };
            var cakeFlour3 = new Ingredient("Cake Flour") { ingredientId = 3, ouncesConsumed = 30m };
            var cakeFlour4 = new Ingredient("Cake Flour") { ingredientId = 4, ouncesConsumed = 40m };
            var cakeFlour5 = new Ingredient("Cake Flour") { ingredientId = 5, ouncesConsumed = 50m };
            var myIngredientBox = new List<Ingredient> { cakeFlour, cakeFlour2, cakeFlour3, cakeFlour4, cakeFlour5 };
            t.initializeDatabase();
            dbCOC.insertListOfIngredientsIntoConsumptionOuncesConsumed(myIngredientBox);
            var myIngredients = dbCOC.queryConsumptionOuncesConsumed();
            var doubleAverage = dbC.doubleAverageOuncesConsumed(cakeFlour);
            Assert.AreEqual(10m, myIngredientBox[0].ouncesConsumed);
            Assert.AreEqual(20m, myIngredientBox[1].ouncesConsumed);
            Assert.AreEqual(30m, myIngredientBox[2].ouncesConsumed);
            Assert.AreEqual(40m, myIngredientBox[3].ouncesConsumed);
            Assert.AreEqual(50m, myIngredientBox[4].ouncesConsumed);
            Assert.AreEqual(60m, doubleAverage);
        }
        [Test]
        public void TestgetDoubleAverageOuncesConsumedForIngredient4() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 18 };
            var yellowCake = new Recipe("Yellow Cake") { id = 2, yield = 12 };
            var whiteCake = new Recipe("White Cake") { id = 3, yield = 12 };
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 4, yield = 24 };
            var eggs = new Ingredient("Eggs") { ingredientId = 1, recipeId = 1, measurement = "2 eggs", sellingWeight = "1 dozen", typeOfIngredient = "eggs", classification = "eggs" };
            var wholeMilk = new Ingredient("Whole Milk") { ingredientId = 2, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "1 quart", typeOfIngredient = "milk", classification = "dairy", sellingPrice = 1.99m };
            var softasilkFlour = new Ingredient("Softasilk") { ingredientId = 3, recipeId = 1, measurement = "3 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var eggs2 = new Ingredient("Eggs") { ingredientId = 4, recipeId = 2, measurement = "4 eggs", sellingWeight = "1 dozen", typeOfIngredient = "eggs", classification = "eggs" };
            var softasilkFlour2 = new Ingredient("Softasilk") { ingredientId = 5, recipeId = 2, measurement = "2 1/4 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 6, recipeId = 2, measurement = "2 teaspoons", sellingWeight = "10 oz", typeOfIngredient = "baking powder" };
            var softasilkFlour3 = new Ingredient("Softasilk") { ingredientId = 7, recipeId = 3, measurement = "2 1/2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var bakingPowder2 = new Ingredient("Baking Powder") { ingredientId = 8, recipeId = 3, measurement = "1 tablespoon 1 teaspoon", sellingWeight = "10 oz", typeOfIngredient = "baking powder" };
            var eggs3 = new Ingredient("Egg whites, merginued") { ingredientId = 9, recipeId = 3, measurement = "3 eggs", sellingWeight = "1 dozen", typeOfIngredient = "eggs", classification = "eggs" };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 10, recipeId = 4, measurement = "6 cups 2 tablespoons", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var chocolateCakeIngredients = new List<Ingredient> { eggs, wholeMilk, softasilkFlour };
            var yellowCakeIngredients = new List<Ingredient> { eggs2, softasilkFlour2, bakingPowder };
            var whiteCakeIngredients = new List<Ingredient> { softasilkFlour3, bakingPowder2, eggs3 };
            var honeyButtermilkBreadIngredients = new List<Ingredient> { breadFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            t.insertListOfIngredientsIntoAllTables(yellowCakeIngredients, yellowCake);
            t.insertListOfIngredientsIntoAllTables(whiteCakeIngredients, whiteCake);
            t.insertListOfIngredientsIntoAllTables(honeyButtermilkBreadIngredients, honeyButtermilkBread);
            var myConsumptionTable = dbC.queryConsumptionTable();
            var myIngredients = dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(2m, myIngredients[0].ouncesConsumed);
            Assert.AreEqual(12.3m, myIngredients[1].ouncesConsumed);
            Assert.AreEqual(13.5m, myIngredients[2].ouncesConsumed);
            Assert.AreEqual(4m, myIngredients[3].ouncesConsumed);
            Assert.AreEqual(10.12m, myIngredients[4].ouncesConsumed);
            Assert.AreEqual(.35m, myIngredients[5].ouncesConsumed);
            Assert.AreEqual(11.25m, myIngredients[6].ouncesConsumed);
            Assert.AreEqual(.7m, myIngredients[7].ouncesConsumed);
            Assert.AreEqual(3m, myIngredients[8].ouncesConsumed);
            Assert.AreEqual(33.08m, myIngredients[9].ouncesConsumed);
            Assert.AreEqual(5, myConsumptionTable.Count());
        }
        [Test]
        public void TestIngredientRestockCondition() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var chocolateChipCookies = new Recipe("Chocolate Chip Cookies") { id = 1, yield = 18 };
            var chocolateChipCheesecake = new Recipe("Chocolate Chip Cheesecake") { id = 2, yield = 16 };
            var cheeseCake = new Recipe("Cheesecake") { id = 3, yield = 16 };
            var chocolateCake = new Recipe("Chocolate Cake") { id = 4, yield = 20 };
            var yellowCake = new Recipe("Yellow Cake") { id = 5, yield = 16 };
            var chocolateChips = new Ingredient("Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            var brownSugar = new Ingredient("Brown Sugar") { ingredientId = 2, recipeId = 1, measurement = "1/2 cup", sellingWeight = "2 lb", typeOfIngredient = "brown sugar" };
            var granSugar = new Ingredient("Granulated Sugar") { ingredientId = 3, recipeId = 1, measurement = "1/2 cup", sellingWeight = "4 lb", typeOfIngredient = "granulated sugar" };
            var granSugar2 = new Ingredient("Granulated Sugar") { ingredientId = 4, recipeId = 2, measurement = "1 1/2 cups", sellingWeight = "4 lb", typeOfIngredient = "granulated sugar" };
            var granSugar3 = new Ingredient("Granulated Sugar") { ingredientId = 5, recipeId = 3, measurement = "2 cups", sellingWeight = "4 lb", typeOfIngredient = "granulated sugar" };
            var granSugar4 = new Ingredient("Granulated Sugar") { ingredientId = 6, recipeId = 4, measurement = "2 3/4 cups", sellingWeight = "4 lb", typeOfIngredient = "granlated sugar" };
            var chocolateChipCookesIngredients = new List<Ingredient> { chocolateChips, brownSugar, granSugar };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(chocolateChipCookesIngredients, chocolateChipCookies);
            t.insertIngredientIntoAllTables(granSugar2, chocolateChipCheesecake);
            t.insertIngredientIntoAllTables(granSugar3, cheeseCake);//.5 1.5 2 2.75 average: 12.07     17.85
            t.insertIngredientIntoAllTables(granSugar4, chocolateCake);
            var myIngredients =dbC.queryConsumptionTable();
            var granSugarRestock =dbC.doesIngredientNeedRestocking(granSugar4);
            var brownSugarRestock =dbC.doesIngredientNeedRestocking(brownSugar);
            var chocolateChipsRestock =dbC.doesIngredientNeedRestocking(chocolateChips);
            Assert.AreEqual(true, granSugarRestock);
            Assert.AreEqual(false, brownSugarRestock);
            Assert.AreEqual(true, chocolateChipsRestock);
        }
        [Test]
        public void TestIngredientRestockConditionWithRestock() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();  
            var chocolateChipCheesecake = new Recipe("Chocolate Chip Cheesecake") { id = 1, yield = 16 };
            var chocolateChips = new Ingredient("Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "2 cups", typeOfIngredient = "chocolate chips", sellingWeight = "12 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(chocolateChips, chocolateChipCheesecake);
            var sugarRestock =dbC.doesIngredientNeedRestocking(chocolateChips);
            dbC.refillIngredientInConsumptionDatabase(chocolateChips, "12 oz");
            var myUpdatedIngredient = t.queryAllTablesForIngredient(chocolateChips);
            var updatedSugarRestock =dbC.doesIngredientNeedRestocking(myUpdatedIngredient);
            Assert.AreEqual(true, sugarRestock);
            Assert.AreEqual(true, updatedSugarRestock);
        }
        [Test]
        public void TestIngredientRestockConditionWithRestock2() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var chocolateChipCheesecake = new Recipe("Chocolate Chip Cheesecake") { id = 1, yield = 16 };
            var chocolateChips = new Ingredient("Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "2 cups", typeOfIngredient = "chocolate chips", sellingWeight = "12 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(chocolateChips, chocolateChipCheesecake);
            var sugarRestock = dbC.doesIngredientNeedRestocking(chocolateChips);
             dbC.refillIngredientInConsumptionDatabase(chocolateChips, "36 oz");
            var myUpdatedIngredient = t.queryAllTablesForIngredient(chocolateChips);
            var updatedSugarRestock = dbC.doesIngredientNeedRestocking(myUpdatedIngredient);
            Assert.AreEqual(true, sugarRestock);
            Assert.AreEqual(false, updatedSugarRestock);
        }
        [Test]
        public void TestIngredientExpirationDateAndDeletionFromConsumptionTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 24 };
            //can i do 8 oz for this for the measurement
            var sourCream = new Ingredient("Sour Cream") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "16 oz", sellingPrice = 2.79m, typeOfIngredient = "sour cream", classification = "dairy", expirationDate = new DateTime(1988, 1, 25) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(sourCream, chocolateCake);
            var myIngredientTable = t.queryAllTablesForIngredient(sourCream);
            var myConsumptionTable =  dbC.queryConsumptionTable();
            var exDate = new DateTime(1988, 1, 25);
            Assert.AreEqual(1, myConsumptionTable.Count());
            Assert.AreEqual(-8.6m, myConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(exDate, myIngredientTable.expirationDate);
        }
        [Test]
        public void TestIngredientExpirationDateDeleteFromConsumptionTable2() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 16 };
            var sourCream = new Ingredient("Sour Cream") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "16 oz", sellingPrice = 2.79m, typeOfIngredient = "sour cream", classification = "dairy", expirationDate = new DateTime(2017, 1, 25) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(sourCream, chocolateCake);
            var myIngredient = t.queryAllTablesForIngredient(sourCream);
            var myIngredientConsumptionTable = dbC.queryConsumptionTable();
             dbC.subtractOuncesRemainingIfExpirationDateIsPast(sourCream);
            //var myUpdatedIngredient = t.queryAllTablesForIngredient(sourCream);
            //var myUpdatedIngredients = t.queryConsumptionTable();
            var exDate = new DateTime(2017, 1, 25);
            Assert.AreEqual(1, myIngredientConsumptionTable.Count());
            Assert.AreEqual(7.4m, myIngredient.ouncesRemaining);
            Assert.AreEqual(exDate, myIngredient.expirationDate);
            //something with the date is not transfering... i'm getting a data type incorrect somewhere with the consumption table
        }
        [Test]
        public void TestIngredientExpirationDateFromConsumptionTable3() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbI = new DatabaseAccessIngredient(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 16 };
            var sourCream = new Ingredient("Sour Cream") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "16 oz", sellingPrice = 2.79m, typeOfIngredient = "sour cream", classification = "dairy", expirationDate = new DateTime(2017, 1, 16) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(sourCream, chocolateCake);
            var myIngredient = t.queryAllTablesForIngredient(sourCream);
            var myIngredients = dbI.queryIngredients();
            var myUpdatedIngredient = t.queryAllTablesForIngredient(sourCream);
            var myUpdatedIngredients = dbC.queryConsumptionTable();
            var exDate = new DateTime(2017, 1, 16);
            Assert.AreEqual(1, myIngredients.Count());
            Assert.AreEqual(0m, myUpdatedIngredient.ouncesRemaining);
            Assert.AreEqual(0m, myUpdatedIngredients[0].ouncesRemaining);
            Assert.AreEqual(exDate, myIngredients[0].expirationDate);
            //something with the date is not transfering... i'm getting a data type incorrect somewhere with the consumption table
        }
        [Test]
        public void TestIngredientExpirationDateConsumptionTable4() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption();
            var dbI = new DatabaseAccessIngredient(); 
            var yellowCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var wholeMilk = new Ingredient("Whole Milk") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "1 gallon", sellingPrice = 3.99m, typeOfIngredient = "milk", classification = "dairy", expirationDate = new DateTime(2017, 1, 18) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(wholeMilk, yellowCake);
            var myIngredient = t.queryAllTablesForIngredient(wholeMilk);
            var myIngredientBox = dbI.queryIngredients();
            var myConsumptionTable = dbC.queryConsumptionTable();
            var exDate = new DateTime(2017, 1, 18);
            Assert.AreEqual(12.3m, myIngredient.ouncesConsumed);
            Assert.AreEqual(12.3m, myConsumptionTable[0].ouncesConsumed);
            Assert.AreEqual(-12.3m, myConsumptionTable[0].ouncesRemaining);
            Assert.AreEqual(exDate, myIngredientBox[0].expirationDate);
            Assert.AreEqual(exDate, myIngredient.expirationDate);
        }
        [Test]
        public void TestIngredientExpirationDateAndRefill() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var yellowCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var wholeMilk = new Ingredient("Whole Milk") { ingredientId = 1, recipeId = 1, measurement = "1 3/4 cups", sellingWeight = "1 gallon", sellingPrice = 3.99m, typeOfIngredient = "milk", classification = "dairy", expirationDate = new DateTime(2017, 1, 18) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(wholeMilk, yellowCake);
            var myIngredient = t.queryAllTablesForIngredient(wholeMilk);
            var exDate = new DateTime(2017, 1, 18);
             dbC.refillIngredientInConsumptionDatabase(wholeMilk, "1 quart", "1.27.2017");
            var newExDate = new DateTime(2017, 1, 27);
            var myUpdatedIngredient = t.queryAllTablesForIngredient(wholeMilk);
            Assert.AreEqual(14.35m, myIngredient.ouncesConsumed);
            Assert.AreEqual(32m, myUpdatedIngredient.ouncesRemaining);
            Assert.AreEqual(newExDate, myUpdatedIngredient.expirationDate);
        }
        [Test]
        public void TestingMultipleRecipesWIthExpiredIngredients() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessConsumption(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 16 };
            var yellowCake = new Recipe("Yellow Cake") { id = 2, yield = 20 };
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 3, yield = 24 };
            var sourCream = new Ingredient("Sour Cream") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "16 oz", sellingPrice = 2.79m, classification = "dairy", typeOfIngredient = "sour cream", expirationDate = new DateTime(2017, 1, 18) };
            var wholeMilk = new Ingredient("Whole Milk") { ingredientId = 2, recipeId = 2, measurement = "1 1/2 cups", sellingWeight = "1 quart", sellingPrice = 1.29m, classification = "dairy", typeOfIngredient = "milk", expirationDate = new DateTime(2017, 1, 4) };
            var buttermilk = new Ingredient("Buttermilk") { ingredientId = 3, recipeId = 3, measurement = "2 cups", sellingWeight = "1 quart", sellingPrice = 1.79m, classification = "dairy", typeOfIngredient = "buttermilk", expirationDate = new DateTime(2016, 12, 28) };
            var myIngredientBox = new List<Ingredient> { sourCream, wholeMilk, buttermilk };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(sourCream, chocolateCake);
            t.insertIngredientIntoAllTables(wholeMilk, yellowCake);
            t.insertIngredientIntoAllTables(buttermilk, honeyButtermilkBread);
            var myIngredients = t.queryAllTablesForAllIngredients(myIngredientBox);
             dbC.refillIngredientInConsumptionDatabase(sourCream, "8 oz", "1.25.17");
             dbC.refillIngredientInConsumptionDatabase(buttermilk, "1 quart", "2.14.17");
            Assert.AreEqual(8.6m, myIngredients[0].ouncesConsumed);
            Assert.AreEqual(8m, myIngredientBox[0].ouncesRemaining);
            Assert.AreEqual(12.3, myIngredientBox[1].ouncesConsumed);
            Assert.AreEqual(0m, myIngredientBox[1].ouncesRemaining);
            Assert.AreEqual(16.4m, myIngredientBox[2].ouncesConsumed);
            Assert.AreEqual(32m, myIngredientBox[2].ouncesRemaining);
        }
        [Test]
        public void TestEggsExpirationDate() {
            var t = new DatabaseAccess();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 24 };
            var eggs = new Ingredient("Eggs") { ingredientId = 1, recipeId = 1, measurement = "3 eggs", sellingWeight = "12 eggs", sellingPrice = 2.99m, classification = "eggs", typeOfIngredient = "eggs", expirationDate = new DateTime(2017, 1, 13) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(eggs, chocolateCake);
            var myIngredient = t.queryAllTablesForIngredient(eggs);
            Assert.AreEqual(3m, myIngredient.ouncesConsumed);
            Assert.AreEqual(0m, myIngredient.ouncesRemaining);
        }
        [Test]
        public void TestEggsExpirationDate2() {
            var t = new DatabaseAccess();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 24 };
            var eggs = new Ingredient("Eggs") { ingredientId = 1, recipeId = 1, measurement = "3 eggs", sellingWeight = "12 eggs", sellingPrice = 2.99m, classification = "eggs", typeOfIngredient = "eggs", expirationDate = new DateTime(2017, 3, 13) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(eggs, chocolateCake);
            var myIngredient = t.queryAllTablesForIngredient(eggs);
            Assert.AreEqual(3m, myIngredient.ouncesConsumed);
            Assert.AreEqual(9m, myIngredient.ouncesRemaining);
        }
    }
}
