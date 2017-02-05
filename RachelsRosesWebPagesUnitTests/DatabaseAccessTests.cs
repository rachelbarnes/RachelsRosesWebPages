using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework;
using RachelsRosesWebPages.Models;
using RachelsRosesWebPages.Controllers;

namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class DatabaseAccessTests {
        [Test]
        public void TestInsertionIntoAllTables() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCosts = new DatabaseAccessCosts();
            var dbD = new DatabaseAccessDensities();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1
            };
            var i = new Ingredient("King Arthur Bread Flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "6 cups",
                density = 5.4m
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngInfo = dbI.queryAllIngredientsFromIngredientTable();
            var myIngCons = dbC.queryConsumptionTable();
            var myIngDens = dbD.queryDensitiesTableAllRows();
            var myIngCost = dbCosts.queryCostTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1, myIngCons.Count());
            Assert.AreEqual(1, myIngDens.Count());
            Assert.AreEqual(1, myIngCost.Count());
        }
        [Test]
        public void TestInsertionIntoAllTables2() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1
            };
            var i = new Ingredient("King Arthur Bread Flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "6 cups",
                density = 5.4m
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            dbI.getIngredientMeasuredPrice(i, r);
            var myIngInfo = dbI.queryAllIngredientsFromIngredientTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1.7m, myIngInfo[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestInsertionIntoTables3() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbCosts = new DatabaseAccessCosts();
            var r = new Recipe("Sour Cream Chocolate Cake") {
                id = 1
            };
            var i = new Ingredient("Hershey's Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "8 ounces",
                measurement = "1 cup",
                density = 4.16m
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            dbI.getIngredientMeasuredPrice(i, r);
            var myIngInfo = dbI.queryAllIngredientsFromIngredientTable();
            var myInfoCostInfo = dbCosts.queryCostTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(3.34m, myInfoCostInfo[0].sellingPrice);
            Assert.AreEqual(1.74m, myIngInfo[0].priceOfMeasuredConsumption);
        }

        [Test]
        public void TestQueryAllTables() {
            var t = new DatabaseAccess();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1
            };
            var i = new Ingredient("King Arthur Bread Flour") {
                recipeId = 1,
                ingredientId = 1,
                density = 5.4m,
                measurement = "6 cups",
                sellingWeight = "5 lb"
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredient = t.queryAllRelevantTablesSQLByIngredientName(i);
            Assert.AreEqual(80m, myIngredient.sellingWeightInOunces);
            Assert.AreEqual(4.20m, myIngredient.sellingPrice);
            Assert.AreEqual(.0525m, myIngredient.pricePerOunce);
            Assert.AreEqual(1.70m, myIngredient.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestGettingAllIngredientFromAllTables() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("Sour Cream Chocolate Cake") {
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
            t.insertIngredientIntoAllTables(i, r);
            dbI.getIngredientMeasuredPrice(i, r);
            var myIngredient = t.queryAllRelevantTablesSQLByIngredientName(i);
            Assert.AreEqual(3.34m, myIngredient.sellingPrice);
            Assert.AreEqual(.87m, myIngredient.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestQueryingListOfIngredients() {
            var t = new DatabaseAccess();
            var r = new Recipe("Chocolate Chip Cookies") {
                id = 1
            };
            var chococateChips = new Ingredient("Nestle Tollhouse Semi Sweet Chocolate Chips") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "12 ounces",
                measurement = "1 3/4 cups",
                density = 5.35m
            };
            var flour = new Ingredient("King Arthur All Purpose Flour") {
                recipeId = 1,
                ingredientId = 2,
                sellingWeight = "5 lb",
                measurement = "2 1/4 cups",
                density = 5m
            };
            var vanillaExtract = new Ingredient("McCormick Vanilla Extract") {
                recipeId = 1,
                ingredientId = 3,
                sellingWeight = "8 fl oz",
                measurement = "1 teaspoon",
                density = 6.86m
            };
            var salt = new Ingredient("Morton Kosher Salt") {
                recipeId = 1,
                ingredientId = 4,
                sellingWeight = "48 ounces",
                measurement = "1 teaspoon",
                density = 10.72m
            };
            var granulatedSugar = new Ingredient("Domino Granulated Sugar") {
                recipeId = 1,
                ingredientId = 5,
                sellingWeight = "4 lb",
                measurement = "3/4 cup",
                density = 7.1m
            };
            var brownSugar = new Ingredient("Domino Brown Sugar") {
                recipeId = 1,
                ingredientId = 6,
                sellingWeight = "32 oz",
                measurement = "3/4 cup",
                density = 7.75m
            };
            var bakingSoda = new Ingredient("Baking Soda") {
                recipeId = 1,
                ingredientId = 7,
                sellingWeight = "4 lb",
                measurement = "1 teaspoon",
                density = 8.57m
            };
            var butter = new Ingredient("Unsalted Butter") {
                recipeId = 1,
                ingredientId = 8,
                sellingWeight = "1 lb",
                measurement = "1 cup",
                density = 8,
                sellingPrice = 3.99m
            };
            var ListOfIngredients = new List<Ingredient> { chococateChips, flour, vanillaExtract, salt, granulatedSugar, brownSugar, bakingSoda, butter };
            var firstListOfIngredients = new List<string>();
            var secondListOfIngredients = new List<string>();
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(ListOfIngredients, r);
            var myIngredients = t.queryAllTablesForAllIngredients(ListOfIngredients);
            foreach (var ing in ListOfIngredients)
                firstListOfIngredients.Add(ing.name);
            foreach (var ing in myIngredients)
                secondListOfIngredients.Add(ing.name);
            Assert.AreEqual(firstListOfIngredients[0], secondListOfIngredients[0]);
            Assert.AreEqual(firstListOfIngredients[1], secondListOfIngredients[1]);
            Assert.AreEqual(firstListOfIngredients[2], secondListOfIngredients[2]);
            Assert.AreEqual(firstListOfIngredients[3], secondListOfIngredients[3]);
            Assert.AreEqual(firstListOfIngredients[4], secondListOfIngredients[4]);
            Assert.AreEqual(firstListOfIngredients[5], secondListOfIngredients[5]);
            Assert.AreEqual(firstListOfIngredients[6], secondListOfIngredients[6]);
            Assert.AreEqual(firstListOfIngredients[7], secondListOfIngredients[7]);
        }
        [Test]
        public void TestQueryAllIngredientsWithDensityCostTableAdded() {
            var t = new DatabaseAccess();
            var r = new Recipe("something that takes all purpose flour") { id = 1 };
            var i = new Ingredient("all purpose flour") {
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "6 cups",
                recipeId = 1,
                density = 5m
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = t.queryAllRelevantTablesSQLByIngredientName(i);
            Assert.AreEqual(5m, myIngredients.density);
        }
        [Test]
        public void TestQueryListOfIngredientsDensityCostTableAdded() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var cocnutMacaroons = new Recipe("Coconut Macaroons") { id = 1 };
            var i = new Ingredient("Baker's Coconut Flakes") {
                recipeId = 1,
                measurement = "1 cup",
                sellingWeight = "14 oz",
                density = 2.5m,
                ingredientId = 1 //2.6, .46
            };
            var i2 = new Ingredient("All Purpose Flour") {
                recipeId = 1,
                measurement = "1/2 cup",
                sellingWeight = "5 lb",
                density = 5m,
                ingredientId = 2 //3.65
            };
            var cocnutMcaroonsIngredients = new List<Ingredient> { i, i2 };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(cocnutMcaroonsIngredients, cocnutMacaroons);
            var myIngredients = t.queryAllTablesForAllIngredients(cocnutMcaroonsIngredients);
            var myRecipes = dbR.queryRecipes();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(2, myIngredients.Count());
            Assert.AreEqual(2.60m, myIngredients[0].sellingPrice);
            Assert.AreEqual(.46m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.1857m, myIngredients[0].pricePerOunce);
            Assert.AreEqual(2.5m, myIngredients[0].density);
            Assert.AreEqual(3.65m, myIngredients[1].sellingPrice);
            Assert.AreEqual(.11m, myIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.0456m, myIngredients[1].pricePerOunce);
            Assert.AreEqual(5m, myIngredients[1].density);
        }
        [Test]
        public void TestSQLQueryAllTalesForIngredients() {
            var t = new DatabaseAccess();
            var cake = new Recipe("Cake") { id = 1, yield = 14 };
            var softasilk = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour", classification = "flour", expirationDate = new DateTime(2017, 6, 5) };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(softasilk, cake);
            var myIngredient = t.queryAllRelevantTablesSQLByIngredientName(softasilk);
            Assert.AreEqual("Softasilk Cake Flour", myIngredient.name);
            Assert.AreEqual(1, myIngredient.recipeId);
            Assert.AreEqual(1, myIngredient.ingredientId);
            Assert.AreEqual("2 cups", myIngredient.measurement);
            Assert.AreEqual(9m, myIngredient.ouncesConsumed);
            Assert.AreEqual(23m, myIngredient.ouncesRemaining);
            Assert.AreEqual("flour", myIngredient.classification);
            Assert.AreEqual("cake flour", myIngredient.typeOfIngredient);
            Assert.AreEqual(.84m, myIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(2.98m, myIngredient.sellingPrice);
            Assert.AreEqual("32 oz", myIngredient.sellingWeight);
            Assert.AreEqual(32m, myIngredient.sellingWeightInOunces);
            Assert.AreEqual(.0931m, myIngredient.pricePerOunce);
        }
        [Test]
        public void TestSQLQueryAllTablesForIngredients() {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var yellowCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var chocolateCake = new Recipe("Chocolate Cake") { id = 2, yield = 18 };
            var marbleCake = new Recipe("Marble Cake") { id = 3, yield = 24 };
            var softasilkCakeFlour = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", typeOfIngredient = "cake flour", classification = "flour", sellingWeight = "32 oz" };
            var bakingSoda = new Ingredient("Baking Soda") { ingredientId = 2, recipeId = 1, measurement = "2 1/2 teaspoons", typeOfIngredient = "baking soda", classification = "rising agent", sellingWeight = "4 lb" };
            var eggs = new Ingredient("Eggs") { ingredientId = 3, recipeId = 1, measurement = "3 eggs", sellingWeight = "1 dozen", typeOfIngredient = "egg", classification = "egg", sellingPrice = 1.99m, expirationDate = new DateTime(2017, 4, 8) };
            var vanillaExtract = new Ingredient("Vanilla Extract") { ingredientId = 4, recipeId = 1, measurement = "2 teaspoons", sellingWeight = "2 oz", classification = "flavoring", typeOfIngredient = "vanilla extract" };
            var granulatedSugar = new Ingredient("Granulated Sugar") { ingredientId = 5, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "4 lb", classification = "sugar", typeOfIngredient = "white sugar" };
            var softasilkCakeFlour2 = new Ingredient("Softasilk Cake Flour") { ingredientId = 6, recipeId = 2, measurement = "2 cups", sellingWeight = "32 ounces", typeOfIngredient = "cake flour", classification = "flour" };
            var bakingCocoa = new Ingredient("Unsweetened Cocoa") { ingredientId = 7, recipeId = 2, measurement = "1 cup", sellingWeight = "8 oz", classification = "baking chocolate", typeOfIngredient = "baking cocoa" };
            var eggs2 = new Ingredient("Eggs") { ingredientId = 8, recipeId = 2, measurement = "3 eggs", sellingWeight = "1 dozen", sellingPrice = 1.99m, typeOfIngredient = "egg", classification = "egg", expirationDate = new DateTime(2017, 4, 8) };
            var granulatedSugar2 = new Ingredient("Granulated Sugar") { ingredientId = 9, recipeId = 2, measurement = "1 1/2 cups", sellingWeight = "4 lb", classification = "sugar", typeOfIngredient = "white sugar" };
            var softasilkCakeFlour3 = new Ingredient("Softasilk Cake Flour") { ingredientId = 10, recipeId = 3, measurement = "2 cups", sellingWeight = "32 ounces", typeOfIngredient = "cake flour", classification = "flour" };
            var yellowCakeIngredients = new List<Ingredient> { softasilkCakeFlour, bakingSoda, eggs, vanillaExtract, granulatedSugar };
            var chocolateCakeIngredients = new List<Ingredient> { softasilkCakeFlour2, bakingCocoa, eggs2, granulatedSugar2 };
            var myIngredients = new List<Ingredient> { softasilkCakeFlour, bakingSoda, eggs, vanillaExtract, granulatedSugar, softasilkCakeFlour2, bakingCocoa, eggs2, granulatedSugar2, softasilkCakeFlour3};
            db.initializeDatabase();
            db.insertListOfIngredientsIntoAllTables(yellowCakeIngredients, yellowCake);
            db.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            //db.insertIngredientIntoAllTables(softasilkCakeFlour2, chocolateCake); 
            db.insertIngredientIntoAllTables(softasilkCakeFlour3, marbleCake);
            var myIngredientBox = db.queryAllRelevantTablesSQLForListOfIngredients(myIngredients);
            Assert.AreEqual(10, myIngredientBox.Count());
            Assert.AreEqual(1, myIngredientBox[0].ingredientId);
            Assert.AreEqual("Softasilk Cake Flour", myIngredientBox[0].name);
            Assert.AreEqual("1 1/2 cups", myIngredientBox[0].measurement);
            Assert.AreEqual(6.75m, myIngredientBox[0].ouncesConsumed);
            Assert.AreEqual(25.25m, myIngredientBox[0].ouncesRemaining);
            Assert.AreEqual(.63m, myIngredientBox[0].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Baking Soda", myIngredientBox[1].name);
            Assert.AreEqual("2 1/2 teaspoons", myIngredientBox[1].measurement);
            Assert.AreEqual(.45m, myIngredientBox[1].ouncesConsumed);
            Assert.AreEqual(63.55m, myIngredientBox[1].ouncesRemaining);
            Assert.AreEqual(.02m, myIngredientBox[1].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Eggs", myIngredientBox[2].name);
            Assert.AreEqual("3 eggs", myIngredientBox[2].measurement);
            Assert.AreEqual(3m, myIngredientBox[2].ouncesConsumed);
            Assert.AreEqual(9m, myIngredientBox[2].ouncesRemaining);
            Assert.AreEqual(.5m, myIngredientBox[2].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Vanilla Extract", myIngredientBox[3].name);
            Assert.AreEqual("2 teaspoons", myIngredientBox[3].measurement);
            Assert.AreEqual(.29m, myIngredientBox[3].ouncesConsumed);
            Assert.AreEqual(1.71m, myIngredientBox[3].ouncesRemaining);
            Assert.AreEqual(.53m, myIngredientBox[3].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Granulated Sugar", myIngredientBox[4].name);
            Assert.AreEqual("1 1/2 cups", myIngredientBox[4].measurement);
            Assert.AreEqual(10.65m, myIngredientBox[4].ouncesConsumed);
            Assert.AreEqual(53.35m, myIngredientBox[4].ouncesRemaining);
            Assert.AreEqual(.58m, myIngredientBox[4].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Softasilk Cake Flour", myIngredientBox[5].name);
            Assert.AreEqual("2 cups", myIngredientBox[5].measurement);
            Assert.AreEqual(9m, myIngredientBox[5].ouncesConsumed);
            Assert.AreEqual(16.25m, myIngredientBox[5].ouncesRemaining);
            Assert.AreEqual(.84m, myIngredientBox[5].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Unsweetened Cocoa", myIngredientBox[6].name);
            Assert.AreEqual("1 cup", myIngredientBox[6].measurement);
            Assert.AreEqual(4.16m, myIngredientBox[6].ouncesConsumed);
            Assert.AreEqual(3.84m, myIngredientBox[6].ouncesRemaining);
            Assert.AreEqual(1.65m, myIngredientBox[6].priceOfMeasuredConsumption);
            //Assert.AreEqual(1, myIngredientBox[6].restock);
            //
            Assert.AreEqual("Eggs", myIngredientBox[7].name);
            Assert.AreEqual("3 eggs", myIngredientBox[7].measurement);
            Assert.AreEqual(3m, myIngredientBox[7].ouncesConsumed);
            Assert.AreEqual(6m, myIngredientBox[7].ouncesRemaining);
            Assert.AreEqual(.5m, myIngredientBox[7].priceOfMeasuredConsumption);
            //Assert.AreEqual(1, myIngredientBox[7].restock);
            //
            Assert.AreEqual("Granulated Sugar", myIngredientBox[8].name);
            Assert.AreEqual("1 1/2 cups", myIngredientBox[8].measurement);
            Assert.AreEqual(10.65m, myIngredientBox[8].ouncesConsumed);
            Assert.AreEqual(42.7, myIngredientBox[8].ouncesRemaining);
            Assert.AreEqual(.58m, myIngredientBox[8].priceOfMeasuredConsumption);
            //
            Assert.AreEqual("Softasilk Cake Flour", myIngredientBox[9].name);
            Assert.AreEqual("2 cups", myIngredientBox[9].measurement);
            Assert.AreEqual(9m, myIngredientBox[9].ouncesConsumed);
            Assert.AreEqual(7.25m, myIngredientBox[9].ouncesRemaining);
            Assert.AreEqual(.84m, myIngredientBox[9].priceOfMeasuredConsumption);
            ////Assert.AreEqual(1, myIngredientBox[9].restock); 
        }
    }
}

