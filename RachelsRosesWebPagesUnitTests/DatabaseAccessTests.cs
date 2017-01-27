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
            var myIngInfo = dbI.queryIngredients();
            var myIngCons = dbC.queryConsumptionTable();
            var myIngDens = dbD.queryDensitiesTable();
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
            var myIngInfo = dbI.queryIngredients();
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
            var myIngInfo = dbI.queryIngredients();
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
            var myIngredient = t.queryAllTablesForIngredient(i);
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
            var myIngredient = t.queryAllTablesForIngredient(i);
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
            var myIngredients = t.queryAllTablesForIngredient(i);
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
    }
}

