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
    class DatabaseAccessDensitiesTests {
        [Test]
        public void TestDensitiesDatabase() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                density = 4.5m,
                pricePerOunce = .0525m,
                sellingWeight = "5 lbs",
                sellingPrice = 4.20m
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngredientInformation = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngredientInformation.Count());
            Assert.AreEqual(i.name, myIngredientInformation[0].name);
            Assert.AreEqual(i.density, (decimal)myIngredientInformation[0].density);
            Assert.AreEqual(i.pricePerOunce, (decimal)myIngredientInformation[0].pricePerOunce);
            Assert.AreEqual(i.sellingWeight, (string)myIngredientInformation[0].sellingWeight);
            Assert.AreEqual(i.sellingPrice, (decimal)myIngredientInformation[0].sellingPrice);
        }
        [Test]
        public void TestDensitiesDatabase2() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Honey, raw") {
                density = 12m,
                sellingWeight = "32 ounces",
                sellingPrice = 5.59m,
                sellingWeightInOunces = 32m
            };
            i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngredientInformation = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngredientInformation.Count());
            Assert.AreEqual(i.density, (decimal)myIngredientInformation[0].density);
            Assert.AreEqual(i.sellingWeight, (string)myIngredientInformation[0].sellingWeight);
        }
        [Test]
        public void TestUdateDensityDatabase() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Honey, raw") {
                ingredientId = 1,
                density = 8m,
                sellingWeight = "32 ounces",
                sellingPrice = 5.59m,
                sellingWeightInOunces = 32m
            };
            i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            i.density = 12m;
            dbD.updateDensityTable(i);
            var myIngredientInformation = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngredientInformation.Count());
            Assert.AreEqual(1, myIngredientInformation[0].ingredientId);
            Assert.AreEqual(i.density, myIngredientInformation[0].density);
        }
        [Test]
        public void TestUpdateDensityDatabase2() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Butter") {
                ingredientId = 1,
                density = 8m,
                sellingWeight = "1 lb",
                sellingPrice = 3.99m
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1, myIngInfo[0].ingredientId);
            Assert.AreEqual(i.density, myIngInfo[0].density);
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(i.sellingPrice, myIngInfo[0].sellingPrice);
        }
        [Test]
        public void TestSellingWeightDataType() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeightInOunces = 80m,
                sellingWeight = "5 lbs"
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(i.sellingWeightInOunces, myIngInfo[0].sellingWeightInOunces);
            Assert.AreEqual(80m, myIngInfo[0].sellingWeightInOunces);
        } [Test]
        public void TestUpdateDensityDatabase() {
            var t = new DatabaseAccess(); 
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var iSWO = 80;
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(iSWO, myIngInfo[0].sellingWeightInOunces);
        }   [Test]
        public void TestUpdatingSellingWeightInOunces() {
            var t = new DatabaseAccess(); 
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lbs"
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(80, myIngInfo[0].sellingWeightInOunces);
        }  [Test]
        public void TestUpdatingSellingWeightInOunces2() {
            var t = new DatabaseAccess(); 
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Cake Flour") {
                ingredientId = 1,
                sellingWeight = "32 ounces"
            };
            var i2 = new Ingredient("All-Purpose Flour") {
                ingredientId = 2,
                sellingWeight = "2 lbs"
            };
            var i3 = new Ingredient("Baking Powder") {
                ingredientId = 3,
                sellingWeight = "10 oz"
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            dbD.insertIngredientDensityData(i2);
            dbD.insertIngredientDensityData(i3);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(3, myIngInfo.Count());
            Assert.AreEqual(32, myIngInfo[0].sellingWeightInOunces);
            Assert.AreEqual(32, myIngInfo[1].sellingWeightInOunces);
            Assert.AreEqual(10, myIngInfo[2].sellingWeightInOunces);
        }  [Test]
        public void TestUpdatingSellingPrice() {
            var t = new DatabaseAccess(); 
            var dbD = new DatabaseAccessDensities();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(2.62m, myIngInfo[0].sellingPrice);
        }  [Test]
        public void TestPricePerOunce() {
            var t = new DatabaseAccess(); 
            var dbD = new DatabaseAccessDensities();
            var rest = new MakeRESTCalls();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var response = new ItemResponse() {
                name = "King Arthur Flour Unbleached Bread Flour, 5.0 LB"
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(.0525m, myIngInfo[0].pricePerOunce);
        }  [Test]
        public void TestUpdatingSellingPrice2() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 pound"
            };
            var i2 = new Ingredient("Softasilk Cake Flour") {
                ingredientId = 2,
                sellingWeight = "32 ounces"
            };
            var i3 = new Ingredient("Pillsbury All-Purpose Flour") {
                ingredientId = 3,
                sellingWeight = "2 pounds"
            };
            var i4 = new Ingredient("Baking Powder") {
                ingredientId = 4,
                sellingWeight = "10 ounces"
            };
            var response = new ItemResponse() {
                name = "King Arthur Flour Unbleached Bread Flour, 5.0 LB"
            };
            var response2 = new ItemResponse() {
                name = "Pillsbury Softasilk: Enriched Bleached Cake Flour, 32 Oz"
            };
            var response3 = new ItemResponse() {
                name = "Pillsbury Best All Purpose Bleached Enriched Pre-Sifted Flour, 2 lb"
            };
            var respone4 = new ItemResponse() {
                name = "Rumford Premium Aluminum-Free Baking Powder, 10 oz"
            };
            t.initializeDatabase();
            dbD.insertIngredientDensityData(i);
            dbD.insertIngredientDensityData(i2);
            dbD.insertIngredientDensityData(i3);
            dbD.insertIngredientDensityData(i4);
            var myIngInfo = dbD.queryDensitiesTableAllRows();
            Assert.AreEqual(4, myIngInfo.Count());
            Assert.AreEqual(rest.GetItemResponse(i), myIngInfo[0].sellingPrice);
            Assert.AreEqual(rest.GetItemResponse(i2), myIngInfo[1].sellingPrice);
            Assert.AreEqual(rest.GetItemResponse(i3), myIngInfo[2].sellingPrice);
            Assert.AreEqual(rest.GetItemResponse(i4), myIngInfo[3].sellingPrice);
        }   [Test]
        public void TestReturnDensityFromDensityTable() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensityInformation(); 
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("all purpose flour") {
                recipeId = 1,
                ingredientId = 1,
                density = 5m,
                measurement = "6 cups",
                sellingWeight = "5 lb"
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var expected = 5m;
            var actual = dbD.queryDensityTableRowDensityValueByName(i);
            Assert.AreEqual(expected, actual);
        }


        //[Test] // some of this needs to be rewritten- i lost a ()
        //public void TestDeleteIngredientFromDensitiesTable() {
        //    var t = new DatabaseAccess();
        //    var dbD = new DatabaseAccessDensities();
        //    var dbR = new DatabaseAccessRecipe(); 
        //    var bread = new Recipe("Bread") { id = 1 };
        //    var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
        //    t.initializeDatabase();
        //    t.insertIngredientIntoAllTables(honey, bread);
        //    var myRecipes = dbR.MyRecipeBox();
        //    var myIngredient = t.queryAllTablesForIngredient(honey);
        //    dbD.DeleteIngredientFromDensitiesTable(honey);
        //    var myDensitiesIngredients = dbD.queryDensitiesTable();
        //    Assert.AreEqual(1, myRecipes.Count());
        //    Assert.AreEqual(1, myRecipes[0].ingredients.Count());
        //    Assert.AreEqual(0, myDensitiesIngredients.Count());
        //}    
       [Test]
        public void TestGetListOfDistinctSellingWeights() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 20 };
            var eggs = new Ingredient("Eggs") { ingredientId = 1, recipeId = 1, sellingWeight = "1 dozen", sellingPrice = 1.50m, classification = "eggs", typeOfIngredient = "eggs", measurement = "3 eggs", expirationDate = new DateTime(2017, 3, 14) };
            var cakeFlour = new Ingredient("Softasilk Cake Flour") { ingredientId = 2, recipeId = 1, sellingWeight = "32 oz", classification = "flour", typeOfIngredient = "cake flour", measurement = "2 cups 2 tablespoons" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 3, recipeId = 1, sellingWeight = "10 oz", typeOfIngredient = "baking powder", measurement = "2 1/2 teaspoons", classification = "rising agent" };
            var salt = new Ingredient("Salt") { ingredientId = 4, recipeId = 1, sellingWeight = "48 oz", typeOfIngredient = "salt", measurement = "1 teaspoon", classification = "salt" };
            var cocoaPowder = new Ingredient("Semi Sweet Cocoa") { ingredientId = 5, recipeId = 1, sellingWeight = "16 oz", typeOfIngredient = "baking cocoa", measurement = "1 cup", classification = "cocoa powder" };
            var chocolateCakeIngredients = new List<Ingredient> { eggs, cakeFlour, bakingPowder, salt, cocoaPowder };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            var myIngredientBox = t.queryAllTablesForAllIngredients(chocolateCakeIngredients);
            var myListOfSellingWeights = dbD.getListOfDistinctSellingWeights();
            Assert.AreEqual(5, myListOfSellingWeights.Count());
            Assert.AreEqual(eggs.sellingWeight, myListOfSellingWeights[0]);
            Assert.AreEqual(cakeFlour.sellingWeight, myListOfSellingWeights[1]);
            Assert.AreEqual(bakingPowder.sellingWeight, myListOfSellingWeights[2]);
            Assert.AreEqual(salt.sellingWeight, myListOfSellingWeights[3]);
            Assert.AreEqual(cocoaPowder.sellingWeight, myListOfSellingWeights[4]);
        } 
        [Test]
        public void TestGetDensityTableInformationFromDensityTable() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensities();
            var dbDI = new DatabaseAccessDensityInformation(); 
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 20 };
            var cakeFlour = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, sellingWeight = "32 oz", classification = "flour", typeOfIngredient = "cake flour", measurement = "2 cups 2 tablespoons", density = 4.5m, pricePerOunce = .0931m};
            t.initializeDatabase();
            dbDI.insertDensityTextFileIntoDensityInfoDatabase(); 
            dbD.insertIngredientDensityData(cakeFlour);
            var myIngrededientTableInformation = dbD.queryIngredientFromDensityTableByName(cakeFlour);
            Assert.AreEqual(1, myIngrededientTableInformation.ingredientId);
            Assert.AreEqual("32 oz", myIngrededientTableInformation.sellingWeight);
            Assert.AreEqual(32m, myIngrededientTableInformation.sellingWeightInOunces);
            Assert.AreEqual(2.98m, myIngrededientTableInformation.sellingPrice);
            Assert.AreEqual(4.5m, myIngrededientTableInformation.density);
            Assert.AreEqual(.0931m, myIngrededientTableInformation.pricePerOunce); 
        }
    }
}
