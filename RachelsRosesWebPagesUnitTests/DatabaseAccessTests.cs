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
        //some of these tests will not be passing based on rest calls... some of the prices will be different, even if they were passing before... 
        [Test]
        public void TestSeveralRecipes() {
            var t = new DatabaseAccess();
            t.initializeDatabase();
            var r = new Recipe("test") {
                yield = 4
            };
            t.InsertRecipe(r);
            r = new Recipe("other") {
                yield = 1
            };
            t.InsertRecipe(r);
            var returns = t.queryRecipes();
            Assert.AreEqual(2, returns.Count());
            Assert.AreEqual(1, returns[0].id);
        }
        [Test]
        public void TestIngredientTable() {
            var t = new DatabaseAccess();
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
            t.InsertRecipe(r);
            t.insertIngredient(i, r);
            t.insertIngredient(i2, r);
            var recipes = t.queryRecipes();
            var ingredients = t.queryIngredients();
            var myIngredientBox = t.queryIngredients();
            Assert.AreEqual("all-purpose flour", ingredients[0].name);
            Assert.AreEqual("butter", ingredients[1].name);
            //this method ins't enacting the functionality of adding the ingredients to the recipe (as the recipe in my database doesn't have the ingredients listed with it
            //this method wasn't passing because I was asking it to do functionality that I didn't call (assigning recipes an ingredient, with GetFullRecipe wasn't called)
        }
        [Test]
        public void TestDeleteRecipe() {
            var t = new DatabaseAccess();
            var r = new Recipe("Pecan Pie") {
                yield = 8
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.DeleteRecipeAndRecipeIngredients(r);
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(0, myRecipes.Count());
            Assert.AreEqual(false, myRecipes.Contains(r));
        }
        [Test]
        public void CompileRecipeAndProperIngredients() {
            var t = new DatabaseAccess();
            var r = new Recipe("Pecan Pie") {
                yield = 8
            };
            var r2 = new Recipe("White Cake") {
                yield = 16
            };
            var i = new Ingredient("flour", "2 1/2 cups") {
                recipeId = 1
            };
            var i2 = new Ingredient("sugar", "1/3 cup") {
                recipeId = 2
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.InsertRecipe(r2);
            var myRecipes = t.queryRecipes();
            r = myRecipes.First(x => x.yield == 8);
            r2 = myRecipes.First(y => y.yield == 16);
            t.insertIngredient(i, r);
            t.insertIngredient(i2, r2);
            Recipe returnedRecipe = t.GetFullRecipe(r);
            Assert.AreEqual("flour", returnedRecipe.ingredients.First().name);
            Assert.AreEqual(1, returnedRecipe.ingredients.Count());
        }
        [Test]
        public void TestProperRecipeIds() {
            var t = new DatabaseAccess();
            var r = new Recipe("White Cake");
            var r2 = new Recipe("Pecan Pie");
            var r3 = new Recipe("Cranberry Swirl Loaf");
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.InsertRecipe(r2);
            t.InsertRecipe(r3);
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(1, myRecipes[0].id);
            Assert.AreEqual(2, myRecipes[1].id);
            Assert.AreEqual(3, myRecipes[2].id);
        }
        [Test]
        public void TestEditRecipeName() {
            var t = new DatabaseAccess();
            var r = new Recipe("White Cake") {
                id = 1
            };
            //this id here, this "for what" is a very important part of this that I forgot
            var newRecipeName = "Fluffy White Cake";
            t.initializeDatabase();
            t.InsertRecipe(r);
            r.name = newRecipeName;
            t.UpdateRecipe(r);
            var myRecipeBox = t.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
        }
        [Test]
        public void TestEditRecipeName2() {
            var t = new DatabaseAccess();
            var r = new Recipe("Buttermilk Bread") {
                id = 1
            };
            var newRecipeName = "Honey Buttermilk Bread";
            r.name = newRecipeName;
            var r2 = new Recipe("Cranberry Swirl Bread") {
                id = 2
            };
            var newRecipe2Name = "Cranberry Nut Cinnamon Swirl Bread";
            r2.name = newRecipe2Name;
            var r3 = new Recipe("Fluffy White Cake") {
                id = 3
            };
            var newRecipe3Name = "My Favorite White Cake";
            r3.name = newRecipe3Name;
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.InsertRecipe(r2);
            t.InsertRecipe(r3);
            t.UpdateRecipe(r);
            t.UpdateRecipe(r2);
            t.UpdateRecipe(r3);
            var myRecipeBox = t.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
            Assert.AreEqual(newRecipe2Name, myRecipeBox[1].name);
            Assert.AreEqual(newRecipe3Name, myRecipeBox[2].name);
        }
        [Test]
        public void TestEditRecipeName3() {
            var t = new DatabaseAccess();
            var r = new Recipe("Cranberry Swirl Bread") {
                //as a note to self, assign this id with the identity insert (so make sure it's the next chronological id for the recipe in the initialized table
                id = 1
            };
            var newRecipeName = "Cranberry Apple Bread";
            t.initializeDatabase();
            t.InsertRecipe(r);
            r.name = newRecipeName;
            t.UpdateRecipe(r);
            var myRecipeBox = t.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
        }
        [Test]
        public void TestUpdateRecipe() {
            var t = new DatabaseAccess();
            var r = new Recipe("My Favorite White Cake") {
                id = 1
            };
            var updatedRecipeName = "My Fluffy White Cake";
            t.initializeDatabase();
            t.InsertRecipe(r);
            var myRecipeBoxBefore = t.queryRecipes();
            r.name = updatedRecipeName;
            t.UpdateRecipe(r);
            var myRecipeBoxAfterwards = t.queryRecipes();
            Assert.AreEqual("My Favorite White Cake", myRecipeBoxBefore[0].name);
            Assert.AreEqual(r.name, myRecipeBoxAfterwards[0].name);
        }
        [Test]
        public void TestInsertIngredientToIngredientDatabase() {
            var t = new DatabaseAccess();
            var r = new Recipe("Cranberry Swirl Bread") {
                id = 1
            };
            var i = new Ingredient("Cranberries", "2 cups") {
                recipeId = r.id
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.insertIngredient(i, r);
            var myRecipeBox = t.queryRecipes();
            var myIngredientBox = t.queryIngredients();
            var myRecipe = t.GetFullRecipe(r);
            Assert.AreEqual(r.name, myRecipeBox[0].name);
            Assert.AreEqual(i.name, myIngredientBox[0].name);
            Assert.AreEqual(i.measurement, myIngredientBox[0].measurement);
            Assert.AreEqual(i.name, myRecipe.ingredients[0].name);
            Assert.AreEqual(i.measurement, myRecipe.ingredients[0].measurement);
        }
        //[Test]
        //public void TestSortingofQueriedRecipesAlphabetically() {
        //    var t = new DatabaseAccess();
        //    var r = new Recipe("Cranberry Swirl Bread");
        //    var r2 = new Recipe("Honey Buttermilk Bread");
        //    var r3 = new Recipe("Honey Buttermilk Cinnamon Bread");
        //    var r4 = new Recipe("Fluffy White Bread");
        //    var r5 = new Recipe("Abrosia");
        //    t.initializeDatabase();
        //    t.InsertRecipe(r); 
        //    t.InsertRecipe(r2); 
        //    t.InsertRecipe(r3); 
        //    t.InsertRecipe(r4); 
        //    t.InsertRecipe(r5);
        //    var MyRecipeBox = t.queryRecipes();
        //    Assert.AreEqual(r5.name, MyRecipeBox[0].name);
        //    Assert.AreEqual(r.name, MyRecipeBox[1].name); 
        //    Assert.AreEqual(r4.name, MyRecipeBox[0].name);
        //    Assert.AreEqual(r2.name, MyRecipeBox[0].name);
        //    Assert.AreEqual(r3.name, MyRecipeBox[0].name);
        //}
        [Test]
        public void TestUpdatingIngredients() {
            var t = new DatabaseAccess();
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
            t.InsertRecipe(r);
            t.insertIngredient(i, r);
            t.UpdateIngredient(i);
            var myIngredientBox = t.queryIngredients();
            Assert.AreEqual(i2.name, myIngredientBox[0].name);
            Assert.AreEqual(i2.measurement, myIngredientBox[0].measurement);
        }
        [Test]
        public void TestInsertingYield() {
            var t = new DatabaseAccess();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1,
                yield = 0
            };
            var r2 = new Recipe("Fluffy White Cake") {
                id = 2,
                yield = 0
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.InsertRecipe(r2);
            r.yield = 24;
            r2.yield = 16;
            t.UpdateRecipe(r);
            t.UpdateRecipe(r2);
            var myRecipeBox = t.queryRecipes();
            Assert.AreEqual(24, myRecipeBox[0].yield);
            Assert.AreEqual(16, myRecipeBox[1].yield);
        }
        //[Test]
        //public void TestItemId() {
        //    var t = new DatabaseAccess();
        //    var r = new Recipe("Honey Buttermilk Bread") {
        //        id = 1
        //    };
        //    var i = new Ingredient("King Arthur Bread Flour") {
        //        recipeId = 1,
        //        ingredientId = 1,
        //        measurement = "6 cups",
        //        sellingWeight = "5 lb",
        //        density = 5.4m
        //    };
        //    t.initializeDatabase();
        //    t.insertIngredientIntoAllTables(i, r);
        //    var BreadFlour = t.queryAllTablesForIngredient(i);
        //    var myCostTable = t.queryCostTable();
        //    var myIng = t.queryIngredients(); 
        //    Assert.AreEqual(0, BreadFlour.itemId);
        //    Assert.AreEqual(myCostTable[0].itemId, myIng[0].itemId); 
        //}
        [Test]
        public void TestGetFullRecipe() {
            var t = new DatabaseAccess();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1,
                yield = 24
            };
            var i = new Ingredient("Honey, raw") {
                ingredientId = 1,
                recipeId = 1,
                measurement = "1/3 cup"
            };
            var i2 = new Ingredient("Bread Flour") {
                ingredientId = 2,
                recipeId = 1,
                measurement = "6 cups"
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.insertIngredient(i, r);
            t.insertIngredient(i2, r);
            var myRecipeBox = t.queryRecipes();
            var myIngredientBox = t.queryIngredients();
            var myRecipe = t.GetFullRecipe(r);
            Assert.AreEqual(2, myIngredientBox.Count());
            Assert.AreEqual(2, myRecipe.ingredients.Count());
            Assert.AreEqual(i.name, myRecipe.ingredients[0].name);
            Assert.AreEqual(i.measurement, myRecipe.ingredients[0].measurement);
            Assert.AreEqual(i2.name, myRecipe.ingredients[1].name);
            Assert.AreEqual(i2.measurement, myRecipe.ingredients[1].measurement);
        }
        [Test]
        public void TestDensitiesDatabase() {
            var t = new DatabaseAccess();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                density = 4.5m,
                pricePerOunce = .0525m,
                sellingWeight = "5 lbs",
                sellingPrice = 4.20m
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngredientInformation = t.queryDensitiesTable();
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
            var i = new Ingredient("Honey, raw") {
                density = 12m,
                sellingWeight = "32 ounces",
                sellingPrice = 5.59m,
                sellingWeightInOunces = 32m
            };
            i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngredientInformation = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngredientInformation.Count());
            Assert.AreEqual(i.density, (decimal)myIngredientInformation[0].density);
            Assert.AreEqual(i.sellingWeight, (string)myIngredientInformation[0].sellingWeight);
        }
        [Test]
        public void TestUdateDensityDatabase() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Honey, raw") {
                ingredientId = 1,
                density = 8m,
                sellingWeight = "32 ounces",
                sellingPrice = 5.59m,
                sellingWeightInOunces = 32m
            };
            i.pricePerOunce = i.sellingPrice / i.sellingWeightInOunces;
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            i.density = 12m;
            t.updateDensityTable(i);
            var myIngredientInformation = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngredientInformation.Count());
            Assert.AreEqual(1, myIngredientInformation[0].ingredientId);
            Assert.AreEqual(i.density, myIngredientInformation[0].density);
        }
        [Test]
        public void TestUpdateDensityDatabase2() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Butter") {
                ingredientId = 1,
                density = 8m,
                sellingWeight = "1 lb",
                sellingPrice = 3.99m
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1, myIngInfo[0].ingredientId);
            Assert.AreEqual(i.density, myIngInfo[0].density);
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(i.sellingPrice, myIngInfo[0].sellingPrice);
        }
        [Test]
        public void TestSellingWeightDataType() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeightInOunces = 80m,
                sellingWeight = "5 lbs"
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(i.sellingWeightInOunces, myIngInfo[0].sellingWeightInOunces);
            Assert.AreEqual(80m, myIngInfo[0].sellingWeightInOunces);
        }
        [Test]
        public void TestUpdateDensityDatabase() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var iSWO = 80;
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(iSWO, myIngInfo[0].sellingWeightInOunces);
        }
        [Test]
        public void TestUpdatingSellingWeightInOunces() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lbs"
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(80, myIngInfo[0].sellingWeightInOunces);
        }
        [Test]
        public void TestUpdatingSellingWeightInOunces2() {
            var t = new DatabaseAccess();
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
            t.insertIngredientDensityData(i);
            t.insertIngredientDensityData(i2);
            t.insertIngredientDensityData(i3);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(3, myIngInfo.Count());
            Assert.AreEqual(32, myIngInfo[0].sellingWeightInOunces);
            Assert.AreEqual(32, myIngInfo[1].sellingWeightInOunces);
            Assert.AreEqual(10, myIngInfo[2].sellingWeightInOunces);
        }
        [Test]
        public void TestUpdatingSellingPrice() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(2.62m, myIngInfo[0].sellingPrice);
        }
        [Test]
        public void TestPricePerOunce() {
            var t = new DatabaseAccess();
            var rest = new MakeRESTCalls();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var response = new ItemResponse() {
                name = "King Arthur Flour Unbleached Bread Flour, 5.0 LB"
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(.0525m, myIngInfo[0].pricePerOunce);
        }
        [Test]
        public void TestUpdatingSellingPrice2() {
            var t = new DatabaseAccess();
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
            t.insertIngredientDensityData(i);
            t.insertIngredientDensityData(i2);
            t.insertIngredientDensityData(i3);
            t.insertIngredientDensityData(i4);
            var myIngInfo = t.queryDensitiesTable();
            Assert.AreEqual(4, myIngInfo.Count());
            Assert.AreEqual(rest.GetItemResponse(i), myIngInfo[0].sellingPrice);
            Assert.AreEqual(rest.GetItemResponse(i2), myIngInfo[1].sellingPrice);
            Assert.AreEqual(rest.GetItemResponse(i3), myIngInfo[2].sellingPrice);
            Assert.AreEqual(rest.GetItemResponse(i4), myIngInfo[3].sellingPrice);
        }
        [Test]
        public void testInsertionIntoConsumptionDatabase() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Butter") {
                ingredientId = 1,
                density = 8m,
                ouncesConsumed = 6m,
                ouncesRemaining = 2m
            };
            t.initializeDatabase();
            t.insertIngredientConsumtionData(i);
            var myIngInfo = t.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            //Assert.AreEqual(i.ingredientId, myIngInfo[0].ingredientId);
            Assert.AreEqual(i.name, myIngInfo[0].name);
            Assert.AreEqual(i.ouncesConsumed, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(i.ouncesRemaining, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void testInsertionIntoConsumptionDatabase2() {
            var t = new DatabaseAccess();
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
            t.insertIngredient(i, r);
            t.insertIngredient(i2, r);
            t.insertIngredientConsumtionData(i);
            t.insertIngredientConsumtionData(i2);
            var myIngInfo = t.queryConsumptionTable();
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
            t.insertIngredient(i, r);
            t.insertIngredientConsumtionData(i);
            var myIngInfo = t.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(10.8m, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(69.2m, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestUpdateConsumptionTable2() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                density = 4.5m,
                ouncesConsumed = 27m
            };
            t.initializeDatabase();
            t.insertIngredientConsumtionData(i);
            var remainingOunces = i.ouncesConsumed - 80m;
            i.ouncesRemaining = remainingOunces;
            t.updateConsumptionTable(i);
            var myIngInfo = t.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(i.density, myIngInfo[0].density);
            Assert.AreEqual(remainingOunces, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestCalculatedOuncesUsedFromGivenMeasurments() {
            var t = new DatabaseAccess();
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
            t.insertIngredient(i, r);
            t.insertIngredientConsumtionData(i);
            t.updateConsumptionTable(i);
            //after this refactoring, i want to have all of this updating ingredients for the initial calculated ounces consumed and such to be in the insert ingredient to the consumption table!!
            var myIngInfo = t.queryConsumptionTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(2.37m, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(77.63m, myIngInfo[0].ouncesRemaining);
        }
        [Test]
        public void TestCalculatedOuncesConsumedFromMeasurmeent() {
            var t = new DatabaseAccess();
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
            t.insertIngredient(i, r);
            t.insertIngredientConsumtionData(i);
            var myIngInfo = t.queryConsumptionTable();
            Assert.AreEqual(32.4m, myIngInfo[0].ouncesConsumed);
            Assert.AreEqual(47.6m, myIngInfo[0].ouncesRemaining);
        }
        //[Test]
        //public void TestCostDatabaseInsertionAndUpdates() {
        //    var t = new DatabaseAccess();
        //    var rest = new MakeRESTCalls();
        //    var i = new Ingredient("King Arthur Bread Flour") {
        //        ingredientId = 1,
        //        density = 5.4m,
        //        measurement = "6 cups",
        //        ouncesRemaining = 80m,
        //        sellingWeight = "5 lb"
        //    };
        //    t.initializeDatabase();
        //    t.insertIngredientDensityData(i);
        //    t.insertIngredientCostDataCostTable(i);
        //    i.sellingPrice = rest.GetItemResponsePrice(i);
        //    t.updateDensityTable(i);
        //    t.updateConsumptionTable(i);
        //    t.updateConsumptionTableOuncesRemaining(i);
        //    var myIngInfo = t.queryConsumptionTable();
        //    Assert.AreEqual(1, myIngInfo.Count());
        //    Assert.AreEqual(80m, myIngInfo[0].sellingWeightInOunces);
        //    Assert.AreEqual(4.34m, myIngInfo[0].sellingPrice);
        //    Assert.AreEqual(.0543m, myIngInfo[0].pricePerOunce);
        //    Assert.AreEqual(.55m, myIngInfo[0].priceOfMeasuredConsumption); 
        //}
        //i need to test the price of the measured ingredient in the ingredient database (i figured it would be best there)
        [Test]
        public void TestMeasuredIngredientPriceIngredientsTable() {
            var t = new DatabaseAccess();
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
            t.insertIngredient(i, r);
            t.insertIngredientConsumtionData(i);
            t.insertIngredientDensityData(i);
            t.insertIngredientCostDataCostTable(i);
            var IngredientMeasuredPrice = t.MeasuredIngredientPrice(i);
            var myIngInfo = t.queryCostTable();
            Assert.AreEqual(1.70m, IngredientMeasuredPrice);
        }
        [Test]
        public void TestMeasuredIngredientPriceIngredientsTablew() {
            var t = new DatabaseAccess();
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
            t.insertIngredient(i, r);
            t.insertIngredientConsumtionData(i);
            t.insertIngredientDensityData(i);
            t.insertIngredientCostDataCostTable(i);
            var IngredientMeasuredPrice = t.MeasuredIngredientPrice(i);
            var myIngInfo = t.queryCostTable();
            Assert.AreEqual(.43m, IngredientMeasuredPrice);
        }
        [Test]
        public void TestMeasuredIngredientPriceIngredientsTable2() {
            var t = new DatabaseAccess();
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
            t.insertIngredient(i, r);
            t.insertIngredient(i2, r);
            t.insertIngredient(i3, r);
            t.insertIngredientConsumtionData(i);
            t.insertIngredientConsumtionData(i2);
            t.insertIngredientConsumtionData(i3);
            t.insertIngredientDensityData(i);
            t.insertIngredientDensityData(i2);
            t.insertIngredientDensityData(i3);
            t.insertIngredientCostDataCostTable(i);
            t.insertIngredientCostDataCostTable(i2);
            t.insertIngredientCostDataCostTable(i3);
            var ingredientMeasuredPrice1 = t.MeasuredIngredientPrice(i);
            var ingredient2MeasuredPrice = t.MeasuredIngredientPrice(i2);
            var ingredient3MeasuredPrice = t.MeasuredIngredientPrice(i3);
            Assert.AreEqual(.79m, ingredientMeasuredPrice1);
            Assert.AreEqual(.04m, ingredient2MeasuredPrice);
            Assert.AreEqual(.46m, ingredient3MeasuredPrice);
        }
        //ok, so my thought process from here is to be able to insert a recipe and all it's information into the proper tables only from the insertIngredient method... add the other ingredients to the other tables in a catch all kind of method
        //something else that needs to happen is i need to be able to filter our itemrepsonse names that are causing out of range exceptions for my parsers. 
        [Test]
        public void TestInsertionIntoAllTables() {
            var t = new DatabaseAccess();
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
            var myIngInfo = t.queryIngredients();
            var myIngCons = t.queryConsumptionTable();
            var myIngDens = t.queryDensitiesTable();
            var myIngCost = t.queryCostTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1, myIngCons.Count());
            Assert.AreEqual(1, myIngDens.Count());
            Assert.AreEqual(1, myIngCost.Count());
        }
        [Test]
        public void TestInsertionIntoAllTables2() {
            var t = new DatabaseAccess();
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
            t.getIngredientMeasuredPrice(i, r);
            var myIngInfo = t.queryIngredients();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1.7m, myIngInfo[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestInsertionIntoTables3() {
            var t = new DatabaseAccess();
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
            t.getIngredientMeasuredPrice(i, r);
            var myIngInfo = t.queryIngredients();
            var myInfoCostInfo = t.queryCostTable();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(3.34m, myInfoCostInfo[0].sellingPrice);
            Assert.AreEqual(1.74m, myIngInfo[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestHersheysUnsweetenedCocoa() {
            var t = new DatabaseAccess();
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
            t.InsertRecipe(r);
            t.insertIngredientIntoAllTables(i, r);
            t.getIngredientMeasuredPrice(i, r);
            var myIngInfo = t.queryIngredients();
            var myRecipesInfo = t.queryRecipes();
            Assert.AreEqual(1, myRecipesInfo.Count());
            Assert.AreEqual(.87m, myIngInfo[0].priceOfMeasuredConsumption);
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
        public void TestMarshallowsPriceConsumed() {
            var t = new DatabaseAccess();
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
            var myIngredientsCost = t.queryCostTable();
            Assert.AreEqual(2.98m, myIngredient.sellingPrice);
            Assert.AreEqual(1.16m, myIngredient.priceOfMeasuredConsumption);
            //Assert.AreEqual(1.22m, myIngredientsCost[0].priceOfMeasuredConsumption); 
            //that's really interesting... when i query all the tables, i'm able to get the prices... now i just need to assign those ingredients to the proper recipe, and get the recipe aggregated price
            //my biggest problem with this is if i only have 1 route that works, and i have other methods that don't work, that i'm building something that can be dangerously inflexibel
        }
        [Test]
        public void TestGettingAllIngredientFromAllTables() {
            var t = new DatabaseAccess();
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
            t.getIngredientMeasuredPrice(i, r);
            var myIngredient = t.queryAllTablesForIngredient(i);
            Assert.AreEqual(3.34m, myIngredient.sellingPrice);
            Assert.AreEqual(.87m, myIngredient.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestAggregatedPriceInRecipesTable() {
            var t = new DatabaseAccess();
            var r = new Recipe("Sour Cream Chocolate Cake") {
                id = 1,
                aggregatedPrice = 4.53m
            };
            var i = new Ingredient("Hershey's Special Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(4.53m, myRecipes[0].aggregatedPrice);
        }
        [Test]
        public void TestAggregatedPriceInPrecipesTableUpdate() {
            var t = new DatabaseAccess();
            var r = new Recipe("Sour Cream Chocolate Cake") {
                id = 1
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            r.aggregatedPrice = 4.53m;
            t.UpdateRecipe(r);
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(r.aggregatedPrice, myRecipes[0].aggregatedPrice);
            Assert.AreEqual(4.53m, myRecipes[0].aggregatedPrice);
        }
        [Test]
        public void TestAggregatedRecipeMeasurement() {
            var t = new DatabaseAccess();
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
            var i2 = new Ingredient("Softasilk Cake Flour") {
                recipeId = 1,
                ingredientId = 2,
                sellingWeight = "32 oz",
                measurement = "1 1/2 cups",
                density = 4.5m
            };
            var myIngredients = new List<Ingredient> { i, i2 };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            t.insertIngredientIntoAllTables(i2, r);
            t.GetFullRecipePrice(r);
            var myIngInfo = t.queryIngredients();
            var myRecipesInfo = t.queryRecipes();
            Assert.AreEqual(2, myIngInfo.Count());
            Assert.AreEqual(1, myRecipesInfo.Count());
            Assert.AreEqual(.87m, myIngInfo[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngInfo[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.50m, myRecipesInfo[0].aggregatedPrice);
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
            //i have to do something with eggs here... always the black sheep of my plans...
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
        public void TestChocolateChips() {
            var t = new DatabaseAccess();
            var r = new Recipe("Chooalte Chip Cookies") { id = 1 };
            var i = new Ingredient("Semi Sweet Morsels") { ingredientId = 1, recipeId = 1, sellingWeight = "36 oz", density = 5.35m, measurement = "1 cup" };
            //6.98    1.04
            t.initializeDatabase();
            var filename = @"C:\Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt";
            t.insertDensityTextFileIntoDensityInfoDatabase(filename);
            t.insertIngredientIntoAllTables(i, r);
            var mydensityDataInformation = t.queryDensityInfoTable();
            var semiSweetMorsels = t.queryAllTablesForIngredient(i);
            var myRecipes = t.MyRecipeBox();
            Assert.AreEqual("all purpose flour", mydensityDataInformation[0].name);
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(6.98m, semiSweetMorsels.sellingPrice);
            Assert.AreEqual(6.98m, myRecipes[0].ingredients[0].sellingPrice);
            Assert.AreEqual(1.04m, semiSweetMorsels.priceOfMeasuredConsumption);
            Assert.AreEqual(1.04m, myRecipes[0].ingredients[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestAggregatedChocolateChipRecipeConsumedPrice() {
            var t = new DatabaseAccess();
            var r = new Recipe("Chocolate Chip Cookies") {
                id = 1
            };
            var chococateChips = new Ingredient("Semi Sweet Chocolate Morsels") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "36 ounces",
                measurement = "1 3/4 cups",
                density = 5.35m //6.98 1.84
            };
            var flour = new Ingredient("King Arthur All Purpose Flour") {
                recipeId = 1,
                ingredientId = 2,
                sellingWeight = "5 lb",
                measurement = "2 1/4 cups",
                density = 5m //.51
            };
            var vanillaExtract = new Ingredient("McCormick Vanilla Extract") {
                recipeId = 1,
                ingredientId = 3,
                sellingWeight = "8 fl oz",
                measurement = "1 teaspoon",
                density = 6.86m //.63
            };
            var salt = new Ingredient("Morton Kosher Salt") {
                recipeId = 1,
                ingredientId = 4,
                sellingWeight = "48 ounces",
                measurement = "1 teaspoon",
                density = 10.72m //.56
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
            //i have to do something with eggs here... always the black sheep of my plans...
            var ListOfIngredients = new List<Ingredient> { chococateChips, flour, vanillaExtract, salt, granulatedSugar, brownSugar, bakingSoda, butter };
            var NewListOfIngredients = new List<Ingredient>();
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(ListOfIngredients, r);
            var myIngredients = t.queryAllTablesForAllIngredients(ListOfIngredients);
            var myRecieBox = t.MyRecipeBox();
            Assert.AreEqual(1, myRecieBox.Count());
            Assert.AreEqual(1.84m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.29m, myRecieBox[0].aggregatedPrice);
        }
        [Test]
        public void TestWinterCookiesInAJarPriceCostEvaluation() {
            var t = new DatabaseAccess();
            var winterCookies = new Recipe("Winter Cranberry Cookies") {
                id = 1
            };
            var flour = new Ingredient("All Purpose Flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "1 cup 2 tablespoons",
                density = 5m
            };
            var bakingSoda = new Ingredient("Baking Soda") {
                recipeId = 1,
                ingredientId = 2,
                sellingWeight = "4 lb",
                measurement = "1/2 teaspoon",
                density = 8.57m
            };
            var salt = new Ingredient("Morton Kosher Salt") {
                recipeId = 1,
                ingredientId = 3,
                sellingWeight = "48 ounces",
                measurement = "1/2 teaspoon",
                density = 10.72m
            };
            var rolledOats = new Ingredient("Old Fashioned Oats") {
                recipeId = 1,
                ingredientId = 4,
                sellingWeight = "42 ounces",
                measurement = "1/2 cup",
                density = 3m
            };
            var brownSugar = new Ingredient("Domino Brown Sugar") {
                recipeId = 1,
                ingredientId = 5,
                sellingWeight = "32 oz",
                measurement = "1/3 cup",
                density = 7.75m
            };
            var granulatedSugar = new Ingredient("Domino Granulated Sugar") {
                recipeId = 1,
                ingredientId = 6,
                sellingWeight = "4 lb",
                measurement = "1/3 cup",
                density = 7.1m
            };
            var driedCranberries = new Ingredient("Dried Cranberries") {
                recipeId = 1,
                ingredientId = 7,
                sellingWeight = "24 ounces",
                measurement = "1/2 cup",
                density = 4.2m
            };
            var whiteChocolate = new Ingredient("White Chocolate Chips") {
                recipeId = 1,
                ingredientId = 8,
                sellingWeight = "11 ounces",
                measurement = "1/2 cup",
                density = 5.35m
            };
            var choppedPecans = new Ingredient("Chopped Pecans") {
                recipeId = 1,
                ingredientId = 9,
                sellingWeight = "8 ounces",
                measurement = "1/2 cup",
                density = 3.84m
                //this is not getting the selling price for this, but i think i'm getting everything else i need
            };
            t.initializeDatabase();
            var winterCookieIngredients = new List<Ingredient> { flour, bakingSoda, salt, rolledOats, brownSugar, granulatedSugar, driedCranberries, whiteChocolate, choppedPecans };
            t.insertListOfIngredientsIntoAllTables(winterCookieIngredients, winterCookies);
            var myIngredients = t.queryAllTablesForAllIngredients(winterCookieIngredients);
            var myRecipes = t.GetFullRecipe(winterCookies);
            Assert.AreEqual(9, myIngredients.Count());
            Assert.AreEqual(.26m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.00m, myIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.01m, myIngredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual(.15m, myIngredients[3].priceOfMeasuredConsumption);
            Assert.AreEqual(.17m, myIngredients[4].priceOfMeasuredConsumption);
            Assert.AreEqual(.13m, myIngredients[5].priceOfMeasuredConsumption);
            Assert.AreEqual(.43m, myIngredients[6].priceOfMeasuredConsumption);
            Assert.AreEqual(.76m, myIngredients[7].priceOfMeasuredConsumption);
            Assert.AreEqual(1.56m, myIngredients[8].priceOfMeasuredConsumption);
            Assert.AreEqual(3.47m, myRecipes.aggregatedPrice);
        }

        [Test]
        public void TestWinterCookiesInAJarPriceCostEvaluationWithOtherRecipeIngredient() {
            var t = new DatabaseAccess();
            var winterCookies = new Recipe("Winter Cookies") {
                id = 1
            };
            var chocolateChipCookies = new Recipe("Chocolate Chip Cookies In a Jar") {
                id = 2
            };
            var flour = new Ingredient("All Purpose Flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "5 lb",
                measurement = "1 cup 2 tablespoons",
                density = 5m
            };
            var bakingSoda = new Ingredient("Baking Soda") {
                recipeId = 1,
                ingredientId = 2,
                sellingWeight = "4 lb",
                measurement = "1/2 teaspoon",
                density = 8.57m
            };
            var salt = new Ingredient("Morton Kosher Salt") {
                recipeId = 1,
                ingredientId = 3,
                sellingWeight = "48 ounces",
                measurement = "1/2 teaspoon",
                density = 10.72m
            };
            var rolledOats = new Ingredient("Old Fashioned Oats") {
                recipeId = 1,
                ingredientId = 4,
                sellingWeight = "42 ounces",
                measurement = "1/2 cup",
                density = 3m
            };
            var brownSugar = new Ingredient("Domino Brown Sugar") {
                recipeId = 1,
                ingredientId = 5,
                sellingWeight = "32 oz",
                measurement = "1/3 cup",
                density = 7.75m
            };
            var granulatedSugar = new Ingredient("Domino Granulated Sugar") {
                recipeId = 1,
                ingredientId = 6,
                sellingWeight = "4 lb",
                measurement = "1/3 cup",
                density = 7.1m
            };
            var driedCranberries = new Ingredient("Dried Cranberries") {
                recipeId = 1,
                ingredientId = 7,
                sellingWeight = "24 ounces",
                measurement = "1/2 cup",
                density = 4.2m
            };
            var whiteChocolate = new Ingredient("White Chocolate Chips") {
                recipeId = 1,
                ingredientId = 8,
                sellingWeight = "11 ounces",
                measurement = "1/2 cup",
                density = 5.35m
            };
            var choppedPecans = new Ingredient("Chopped Pecans") {
                recipeId = 1,
                ingredientId = 9,
                sellingWeight = "8 ounces",
                measurement = "1/2 cup",
                density = 3.84m
                //this is not getting the selling price for this, but i think i'm getting everything else i need
            };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Chips") {
                recipeId = 2,
                ingredientId = 10,
                sellingWeight = "12 oz",
                measurement = "1 cup",
                density = 5.35m
            };
            t.initializeDatabase();
            var winterCookieIngredients = new List<Ingredient> { flour, bakingSoda, salt, rolledOats, brownSugar, granulatedSugar, driedCranberries, whiteChocolate, choppedPecans };
            var chocolateChipCookieIngredients = new List<Ingredient> { chocolateChips };
            t.insertListOfIngredientsIntoAllTables(winterCookieIngredients, winterCookies);
            t.insertListOfIngredientsIntoAllTables(chocolateChipCookieIngredients, chocolateChipCookies);
            t.GetFullRecipePrice(winterCookies);
            t.GetFullRecipePrice(chocolateChipCookies);
            var myWinterCookieIngredients = t.queryAllTablesForAllIngredients(winterCookieIngredients);
            var myChocolateChipCookieIngredients = t.queryAllTablesForAllIngredients(chocolateChipCookieIngredients);
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(2, myRecipes.Count());
            Assert.AreEqual(9, myWinterCookieIngredients.Count());
            Assert.AreEqual(.26m, myWinterCookieIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.00m, myWinterCookieIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.00m, myWinterCookieIngredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual(.15m, myWinterCookieIngredients[3].priceOfMeasuredConsumption);
            Assert.AreEqual(.17m, myWinterCookieIngredients[4].priceOfMeasuredConsumption);
            Assert.AreEqual(.09m, myWinterCookieIngredients[5].priceOfMeasuredConsumption);
            Assert.AreEqual(.43m, myWinterCookieIngredients[6].priceOfMeasuredConsumption);
            Assert.AreEqual(.76m, myWinterCookieIngredients[7].priceOfMeasuredConsumption);
            Assert.AreEqual(1.56m, myWinterCookieIngredients[8].priceOfMeasuredConsumption);
            Assert.AreEqual(.89m, myChocolateChipCookieIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(3.43m, myRecipes[0].aggregatedPrice);
            Assert.AreEqual(.89m, myRecipes[1].aggregatedPrice);
        }
        //read the density database, have to put that in a database, have to read the database and put it in for all these...
        //i also have to return a list of the items received from walmart's database so you know what you're getting and you can choose which one you would rather have, and be able to select it
        //i would also like to get other stores's databases too, like costco, target maybe, etc. 
        //in the list of itemresponses, look at which one is the best deal for it's price, and bold that one in the view
        //still have to refactor and do tech debt on QueryAllTablesFroAllIngredients
        [Test]
        public void Test2Recipse() {
            var t = new DatabaseAccess();
            var brownies = new Recipe("Brownies") {
                id = 1
            };
            var cocoa = new Ingredient("Hershey's Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1,
                measurement = "1 cup",
                density = 4.16m,
                sellingWeight = "8 oz"
            };
            var cake = new Recipe("Cake") {
                id = 2
            };
            var breadFlour = new Ingredient("Softasilk Cake Flour") {
                recipeId = 2,
                ingredientId = 2,
                measurement = "1 1/2 cup",
                density = 4.5m,
                sellingWeight = "32 oz"
            };
            t.initializeDatabase();
            var browniesIngredients = new List<Ingredient> { cocoa };
            var cakeIngredients = new List<Ingredient> { breadFlour };
            t.insertListOfIngredientsIntoAllTables(browniesIngredients, brownies);
            t.insertListOfIngredientsIntoAllTables(cakeIngredients, cake);
            t.GetFullRecipePrice(brownies);
            t.GetFullRecipePrice(cake);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(1.74m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.74m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngredients[1].priceOfMeasuredConsumption);
        }
        [Test]
        public void Test3Recipes() {
            var t = new DatabaseAccess();
            var cinnamonSwirlBread = new Recipe("Cinnamon Swirl Bread") {
                id = 1
            };
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") {
                id = 2
            };
            var wholeWheatBread = new Recipe("Whole Wheat Bread") {
                id = 3
            };
            var breadFlour = new Ingredient("King Arthur Bread Flour") {
                recipeId = 1,
                measurement = "6 cups",
                ingredientId = 1,
                density = 5.4m,
                sellingWeight = "5 lb"
            };
            var granSugar = new Ingredient("Domino Granulated Sugar") {
                recipeId = 1,
                measurement = "1/3 cup",
                ingredientId = 2,
                density = 7.1m,
                sellingWeight = "4 lb"
            };
            var cinnamon = new Ingredient("Ground Cinnamon") {
                recipeId = 1,
                measurement = "3 tablespoons",
                ingredientId = 3,
                density = 4.4m,
                sellingWeight = "8.75 oz"
            };
            //var butter = new Ingredient("Unsalted Butter") {
            //    recipeId = 1,
            //    measurement = "1/4 cup",
            //    ingredientId = 4,
            //    density = 8m,
            //    sellingWeight = "1 lb",
            //    sellingPrice = 2.99m
            //};
            var breadFlour2 = new Ingredient("King Arthur Bread Flour") {
                recipeId = 2,
                measurement = "6 cups",
                ingredientId = 4,
                density = 5.4m,
                sellingWeight = "5 lb"
            };
            var ginger = new Ingredient("Ground Ginger") {
                recipeId = 2,
                measurement = "1 pinch",
                ingredientId = 5,
                density = 4.4m,
                sellingWeight = "8 oz"
            };
            var granSugar2 = new Ingredient("Granulated Sugar") {
                recipeId = 2,
                measurement = "1 teaspoon",
                ingredientId = 6,
                density = 7.1m,
                sellingWeight = "4 lb"
            };
            var bakingSoda = new Ingredient("Baking Soda") {
                recipeId = 2,
                measurement = "3/4 teaspoon",
                ingredientId = 7,
                density = 8.57m,
                sellingWeight = "4 lb"
            };
            var activeDryYeast = new Ingredient("Active Dry Yeast") {
                recipeId = 2,
                measurement = "2 1/4 teaspoons",
                ingredientId = 8,
                density = 5.49m,
                sellingWeight = "4 oz"
            };
            var honey = new Ingredient("Honey") {
                recipeId = 2,
                measurement = "1/3 cup",
                ingredientId = 9,
                density = 12m,
                sellingWeight = "32 oz"
            };
            var wholeWheatFlour = new Ingredient("Whole Wheat Flour") {
                recipeId = 3,
                measurement = "4 1/2 cups",
                ingredientId = 10,
                density = 5m,
                sellingWeight = "5 lb"
            };
            var honey2 = new Ingredient("Honey") {
                recipeId = 3,
                measurement = "1/3 cup",
                ingredientId = 11,
                density = 12m,
                sellingWeight = "32 oz"
            };
            var granSugar3 = new Ingredient("Granulated Sugar") {
                recipeId = 3,
                measurement = "1 tablespoon",
                ingredientId = 12,
                density = 7.1m,
                sellingWeight = "4 lb"
            };
            var salt = new Ingredient("Morton Salt") {
                recipeId = 3,
                measurement = "1 tablespoon",
                ingredientId = 13,
                density = 10.72m,
                sellingWeight = "48 oz"
            };
            var allPurposeFlour = new Ingredient("All Purpose Flour") {
                recipeId = 3,
                measurement = "2 3/4 cups",
                ingredientId = 14,
                density = 5m,
                sellingWeight = "5 lb"
            };
            var cinnamonBreadIngredients = new List<Ingredient> { breadFlour, granSugar, cinnamon };
            var buttermilkBreadIngredients = new List<Ingredient> { breadFlour2, ginger, granSugar2, bakingSoda, activeDryYeast, honey };
            var wholeWheatBreadIngredients = new List<Ingredient> { wholeWheatFlour, honey2, granSugar3, salt, allPurposeFlour };
            var AllIngredients = new List<Ingredient> { breadFlour, granSugar, cinnamon, breadFlour2, ginger, granSugar2, bakingSoda, activeDryYeast, honey, wholeWheatFlour, honey2, granSugar3, salt, allPurposeFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(cinnamonBreadIngredients, cinnamonSwirlBread);
            t.insertListOfIngredientsIntoAllTables(buttermilkBreadIngredients, honeyButtermilkBread);
            t.insertListOfIngredientsIntoAllTables(wholeWheatBreadIngredients, wholeWheatBread);
            var myRecipeBox = t.MyRecipeBox();
            var myIngredients = t.queryIngredients();
            Assert.AreEqual(3, myRecipeBox.Count());
            //Assert.AreEqual(15, myIngredients.Count()); //this is with the butter included
            Assert.AreEqual(14, myIngredients.Count());
            Assert.AreEqual(1.70m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.13m, myIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.52m, myIngredients[2].priceOfMeasuredConsumption);
            //Assert.AreEqual(.37m, myIngredients[3].priceOfMeasuredConsumption); //this is the butter
            Assert.AreEqual(1.70m, myIngredients[3].priceOfMeasuredConsumption);
            Assert.AreEqual(.01m, myIngredients[4].priceOfMeasuredConsumption);
            Assert.AreEqual(.02m, myIngredients[5].priceOfMeasuredConsumption);
            Assert.AreEqual(.00m, myIngredients[6].priceOfMeasuredConsumption);
            Assert.AreEqual(.30m, myIngredients[7].priceOfMeasuredConsumption);
            Assert.AreEqual(.98m, myIngredients[8].priceOfMeasuredConsumption);
            Assert.AreEqual(1.18m, myIngredients[9].priceOfMeasuredConsumption);
            Assert.AreEqual(.98m, myIngredients[10].priceOfMeasuredConsumption);
            Assert.AreEqual(.02m, myIngredients[11].priceOfMeasuredConsumption);
            Assert.AreEqual(.03m, myIngredients[12].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngredients[13].priceOfMeasuredConsumption);
            Assert.AreEqual(2.35m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(3.01m, myRecipeBox[1].aggregatedPrice);
            Assert.AreEqual(2.84m, myRecipeBox[2].aggregatedPrice);
        }
        [Test]
        public void TestColumnsFromCostTable() {
            var t = new DatabaseAccess();
            var r = new Recipe("My Favorite Yellow Cake") {
                id = 1
            };
            var i = new Ingredient("Softasilk Cake Flour") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "32 oz",
                measurement = "2 1/8 cups",
                density = 4.5m
            };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredient = t.queryAllTablesForIngredient(i);
            Assert.AreEqual(2.98m, myIngredient.sellingPrice);
            Assert.AreEqual(.0931m, myIngredient.pricePerOunce);
        }
        [Test]
        public void TestInsertFileIntoDensityDatabase() {
            var t = new DatabaseAccess();
            var read = new Reader();
            var filename = @"C:\Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt";
            t.initializeDatabase();
            t.insertDensityTextFileIntoDensityInfoDatabase(filename);
            var myDensityInformationIngredients = t.queryDensityInfoTable();
            Assert.AreEqual(41, myDensityInformationIngredients.Count());
            Assert.AreEqual("all purpose flour", myDensityInformationIngredients[0].name);
            Assert.AreEqual(5m, myDensityInformationIngredients[0].density);
            Assert.AreEqual("bananas, mashed", myDensityInformationIngredients[39].name);
            Assert.AreEqual(12m, myDensityInformationIngredients[39].density);
        }
        [Test]
        public void TestAssignDictionaryValuesToIngredientObjects() {
            var t = new DatabaseAccess();
            var read = new Reader();
            var filename = @"C:\Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt";
            t.initializeDatabase();
            var myDensityDict = read.ReadDensityTextFile(filename);
            var myDensityUpdatedIngredients = new List<Ingredient>();
            myDensityUpdatedIngredients = t.assignIngredientDensityDictionaryValuesToListIngredients(myDensityDict);
            Assert.AreEqual(myDensityUpdatedIngredients.Count(), myDensityDict.Count());
            Assert.AreEqual(myDensityUpdatedIngredients[0].density, myDensityDict["all purpose flour"]);
        }
        [Test]
        public void testinsertingredientintodensityinfo() {
            var t = new DatabaseAccess();
            var i = new Ingredient("all purpose flour") {
                density = 5m
            };
            t.initializeDatabase();
            t.insertIngredientIntoDensityInfoDatabase(i);
            var myDensityInfoTable = t.queryDensityInfoTable();
            Assert.AreEqual(1, myDensityInfoTable.Count());
            Assert.AreEqual("all purpose flour", myDensityInfoTable[0].name);
            Assert.AreEqual(5m, myDensityInfoTable[0].density);
        }
        /// <summary>
        /// /
        /// </summary>
        [Test]
        public void TestInsertListOfIngredientsIntoDensityInfo() {
            var t = new DatabaseAccess();
            var i = new Ingredient("all purpose flour") { density = 5m };
            var i2 = new Ingredient("pastry flour") { density = 4.25m };
            var i3 = new Ingredient("vanilla extract") { density = 6.86m };
            var myIngredients = new List<Ingredient> { i, i2, i3 };
            t.initializeDatabase();
            t.insertListIntoDensityInfoDatabase(myIngredients);
            var myDensityInfoTable = t.queryDensityInfoTable();
            Assert.AreEqual(3, myDensityInfoTable.Count());
            Assert.AreEqual(i.name, myDensityInfoTable[0].name);
            Assert.AreEqual(i.density, myDensityInfoTable[0].density);
            Assert.AreEqual(i2.name, myDensityInfoTable[1].name);
            Assert.AreEqual(i2.density, myDensityInfoTable[1].density);
            Assert.AreEqual(i3.name, myDensityInfoTable[2].name);
            Assert.AreEqual(i3.density, myDensityInfoTable[2].density);
        }
        [Test]
        public void TestUpdateDensityInfoTable() {
            var t = new DatabaseAccess();
            var i = new Ingredient("all purpose flour");
            t.initializeDatabase();
            t.insertIngredientIntoDensityInfoDatabase(i);
            var BeforeDensityTableInfo = t.queryDensityInfoTable();
            i.density = 5m;
            t.updateDensityInfoTable(i);
            var AfterDensityTableInfo = t.queryDensityInfoTable();
            Assert.AreEqual(1, BeforeDensityTableInfo.Count());
            Assert.AreEqual(1, AfterDensityTableInfo.Count());
            Assert.AreEqual(0, BeforeDensityTableInfo[0].density);
            Assert.AreEqual(5m, AfterDensityTableInfo[0].density);
        }
        [Test]
        public void TestListOfIngredientsUpdateDensityInfoTable() {
            var t = new DatabaseAccess();
            var i = new Ingredient("all purpose flour");
            var i2 = new Ingredient("pastry flour");
            var i3 = new Ingredient("vanilla extract");
            var myIngredients = new List<Ingredient> { i, i2, i3 };
            t.initializeDatabase();
            t.insertListIntoDensityInfoDatabase(myIngredients);
            var BeforeMyDensityInfoTable = t.queryDensityInfoTable();
            i.density = 5m;
            i2.density = 4.25m;
            i3.density = 6.86m;
            t.updateListOfIngredientsInDensityInfoTable(myIngredients);
            var AfterMyDensityInfoTable = t.queryDensityInfoTable();
            Assert.AreEqual(3, BeforeMyDensityInfoTable.Count());
            Assert.AreEqual(3, AfterMyDensityInfoTable.Count());
            Assert.AreEqual(0, BeforeMyDensityInfoTable[0].density);
            Assert.AreEqual(0, BeforeMyDensityInfoTable[1].density);
            Assert.AreEqual(0, BeforeMyDensityInfoTable[2].density);
            Assert.AreEqual(5m, AfterMyDensityInfoTable[0].density);
            Assert.AreEqual(4.25m, AfterMyDensityInfoTable[1].density);
            Assert.AreEqual(6.86m, AfterMyDensityInfoTable[2].density);
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
            var myRecipes = t.queryRecipes();
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
        //[Test]
        //public void TestUpdateAllTablesDensityCostTableAdded() {
        //    var t = new DatabaseAccess();
        //    var coconutMacaroons = new Recipe("Coconut Macaroons") { id = 1 };
        //    var i = new Ingredient("Baker's Coconut Flakes") {
        //        recipeId = 1,
        //        measurement = "1 cup",
        //        sellingWeight = "14 oz",
        //        ingredientId = 1, //2.6, .46
        //        //.19
        //        density = 1m
        //    };
        //    var i2 = new Ingredient("All Purpose Flour") {
        //        recipeId = 1,
        //        measurement = "1/2 cup",
        //        sellingWeight = "5 lb",
        //        ingredientId = 2, //3.65
        //        density = 1m
        //        //.02
        //    };
        //    var coconutMcaroonsIngredients = new List<Ingredient> { i, i2 };
        //    t.initializeDatabase();
        //    t.insertListOfIngredientsIntoAllTables(coconutMcaroonsIngredients, coconutMacaroons);
        //    var myIngredients = t.queryAllTablesForAllIngredients(coconutMcaroonsIngredients, coconutMacaroons);
        //    var myRecipes = t.queryRecipes();
        //    i.density = 2.5m;
        //    i2.density = 5m; 
        //    t.updateListOfIngredientsForAllTables(coconutMcaroonsIngredients, coconutMacaroons);
        //    var afterMyIngredients = t.queryAllTablesForAllIngredients(coconutMcaroonsIngredients, coconutMacaroons);
        //    var afterMyRecipes = t.queryRecipes(); 
        //    Assert.AreEqual(1, myRecipes.Count());
        //    Assert.AreEqual(2,myIngredients.Count());
        //    Assert.AreEqual(2, afterMyIngredients.Count());
        //    //Assert.AreEqual(.19m, myIngredients[0].priceOfMeasuredConsumption);
        //    //Assert.AreEqual(.02m, myIngredients[1].priceOfMeasuredConsumption); 
        //    Assert.AreEqual(2.60m, afterMyIngredients[0].sellingPrice);
        //    Assert.AreEqual(.46m, afterMyIngredients[0].priceOfMeasuredConsumption);
        //    Assert.AreEqual(.1857m, afterMyIngredients[0].pricePerOunce);
        //    Assert.AreEqual(3.65m, afterMyIngredients[1].sellingPrice);
        //    Assert.AreEqual(.11m, afterMyIngredients[1].priceOfMeasuredConsumption);
        //    Assert.AreEqual(.0456m, afterMyIngredients[1].pricePerOunce);
        //    Assert.AreEqual(2.5m, afterMyIngredients[0].density);
        //    Assert.AreEqual(5m, afterMyIngredients[1].density); 
        //}
        [Test]
        public void TestQueryListOfFullRecipes() {
            var t = new DatabaseAccess();
            var brownies = new Recipe("Brownies") {
                id = 1
            };
            var cocoa = new Ingredient("Hershey's Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1,
                measurement = "1 cup",
                density = 4.16m,
                sellingWeight = "8 oz"
            };
            var cake = new Recipe("Cake") {
                id = 2
            };
            var breadFlour = new Ingredient("Softasilk Cake Flour") {
                recipeId = 2,
                ingredientId = 2,
                measurement = "1 1/2 cup",
                density = 4.5m,
                sellingWeight = "32 oz"
            };
            var browniesIngredients = new List<Ingredient> { cocoa };
            var cakeIngredients = new List<Ingredient> { breadFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(browniesIngredients, brownies);
            t.insertListOfIngredientsIntoAllTables(cakeIngredients, cake);
            t.GetFullRecipePrice(brownies);
            t.GetFullRecipePrice(cake);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(1.74m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.74m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngredients[1].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestGetRecipeIngredients() {
            var t = new DatabaseAccess();
            var brownies = new Recipe("Brownies") {
                id = 1
            };
            var cocoa = new Ingredient("Hershey's Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1,
                measurement = "1 cup",
                density = 4.16m,
                sellingWeight = "8 oz"
            };
            var cake = new Recipe("Cake") {
                id = 2
            };
            var breadFlour = new Ingredient("Softasilk Cake Flour") {
                recipeId = 2,
                ingredientId = 2,
                measurement = "1 1/2 cup",
                density = 4.5m,
                sellingWeight = "32 oz"
            };
            var browniesIngredients = new List<Ingredient> { cocoa };
            var cakeIngredients = new List<Ingredient> { breadFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(browniesIngredients, brownies);
            t.insertListOfIngredientsIntoAllTables(cakeIngredients, cake);
            var myBrownieIngredients = t.ReturnRecipeIngredients(brownies);
            var myCakeIngredients = t.ReturnRecipeIngredients(cake);
            var myRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(cocoa.name, myRecipeBox[0].ingredients[0].name);
            Assert.AreEqual(breadFlour.name, myRecipeBox[1].ingredients[0].name);
        }
        [Test]
        public void TestReturnPriceOfMeasuredIngredient() {
            var t = new DatabaseAccess();
            var r = new Recipe("Bread") { id = 1 };
            var i = new Ingredient("King Arthur Bread Flour") { recipeId = 1, ingredientId = 1, measurement = "6 cups", sellingWeight = "5 lb", density = 5.4m };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myPriceOfMeasuredConsumption = t.returnIngredientMeasuredPrice(i);
            Assert.AreEqual(1.70m, myPriceOfMeasuredConsumption);
        }
        [Test]
        public void TestAggregatedPriceDifferenceInYield() {
            var t = new DatabaseAccess();
            var winterCranberryCookies = new Recipe("Winter Cranberry Cookies") { id = 1, yield = 18 };
            var chocolateChips = new Ingredient("White Morsels") { recipeId = 1, ingredientId = 1, measurement = "1/2 cup", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            var pecans = new Ingredient("Chopped Pecans") { recipeId = 1, ingredientId = 2, measurement = "1/2 cup", sellingWeight = "24 oz", typeOfIngredient = "pecans" };
            var driedCranberries = new Ingredient("Dried Cranberries") { recipeId = 1, ingredientId = 3, measurement = "1/2 cup", sellingWeight = "24 oz", typeOfIngredient = "dried cranberries" };
            var brownSugar = new Ingredient("Brown Sugar") { recipeId = 1, ingredientId = 4, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "brown sugar" };
            var granSugar = new Ingredient("Granulated Sugar") { recipeId = 1, ingredientId = 5, measurement = "1/3 cup", sellingWeight = "4 lb", typeOfIngredient = "white sugar" };
            var allPurposeFlour = new Ingredient("All Purpose Flour") { recipeId = 1, ingredientId = 6, measurement = "1 1/8 cup", sellingWeight = "5 lb", typeOfIngredient = "all purpose flour" };
            var salt = new Ingredient("Salt") { recipeId = 1, ingredientId = 7, measurement = "1/2 teaspoon", sellingWeight = "48 oz", typeOfIngredient = "salt" };
            var bakingSoda = new Ingredient("Baking Soda") { recipeId = 1, ingredientId = 8, measurement = "1/2 teaspoon", sellingWeight = "4 lb", typeOfIngredient = "baking soda" };
            var winterCookiesIngredients = new List<Ingredient> { chocolateChips, pecans, driedCranberries, brownSugar, granSugar, allPurposeFlour, salt, bakingSoda };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(winterCookiesIngredients, winterCranberryCookies);
            var myIngredients = t.queryAllTablesForAllIngredients(winterCookiesIngredients);
            var myRecipeBox = t.MyRecipeBox();
            winterCranberryCookies.yield = 36;
            t.updateAllTablesForAllIngredients(winterCookiesIngredients, winterCranberryCookies);
            var myUpdatedIngredients = t.queryAllTablesForAllIngredients(winterCookiesIngredients);
            var myUpdatedRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(1, myRecipeBox.Count());
            Assert.AreEqual(8, myRecipeBox[0].ingredients.Count());
            Assert.AreEqual(8, myIngredients.Count());
            Assert.AreEqual(1, myUpdatedRecipeBox.Count());
            Assert.AreEqual(8, myUpdatedRecipeBox[0].ingredients.Count());
            Assert.AreEqual(8, myUpdatedIngredients.Count());
            Assert.AreEqual(18, myRecipeBox[0].yield);
            Assert.AreEqual(3.47m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(36, myUpdatedRecipeBox[0].yield);
            Assert.AreEqual(5.35m, myIngredients[0].density);
            Assert.AreEqual(5m, myIngredients[5].density);
            Assert.AreEqual(6.94m, myUpdatedRecipeBox[0].aggregatedPrice);
        }
        [Test]
        public void TestReturnDensityFromDensityTable() {
            var t = new DatabaseAccess();
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
            var actual = t.returnIngredientDensityFromDensityTable(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestReturnDensityFromDensityTable2() {
            var t = new DatabaseAccess();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("Salt") { recipeId = 1, ingredientId = 1, measurement = "1/2 teaspoon", sellingWeight = "48 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var expected = 10.72m;
            var actual = t.returnIngredientDensityFromDensityTable(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestReturnDensityFromDensityTable3() {
            var t = new DatabaseAccess();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("All Purpose Flour") { recipeId = 1, ingredientId = 1, measurement = "3 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var expected = 5m;
            var actual = t.returnIngredientDensityFromDensityTable(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestChangeInPriceFromChangeInYield() {
            var t = new DatabaseAccess();
            var r = new Recipe("bread") { id = 1, yield = 24 };
            var i = new Ingredient("All Purpose Flour") { recipeId = 1, ingredientId = 1, measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            r.yield = 48;
            t.updateAllTables(i, r);
            var myNewIngredients = t.queryIngredients();
            var myNewRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(24, myRecipeBox[0].yield);
            Assert.AreEqual(48, myNewRecipeBox[0].yield);
            Assert.AreEqual("6 cups", myIngredients[0].measurement);
            Assert.AreEqual("12 cups", myNewIngredients[0].measurement);

        }
        [Test]
        public void TestItemId() {
            var t = new DatabaseAccess();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("bread flour") { recipeId = 1, ingredientId = 1, measurement = "3 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = t.queryIngredients();
            var expected = 10308169;
            var actual = myIngredients[0].itemId;
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestItemId2() {
            var t = new DatabaseAccess();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("baking soda") { recipeId = 1, ingredientId = 1, sellingWeight = "4 lb", measurement = "1/2 teaspoon" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = t.queryIngredients();
            var expected = 11027507;
            var actual = myIngredients[0].itemId;
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGetListOfItemResponses() {
            var t = new DatabaseAccess();
            var rest = new MakeRESTCalls();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("bread flour") { ingredientId = 1, recipeId = 1, measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = t.queryIngredients();
            var listOfItemResponses = rest.GetListItemResponses(i);
            Assert.AreEqual(4, listOfItemResponses.Count());
            Assert.AreEqual(10308169, myIngredients[0].itemId);
            Assert.AreEqual(10308169, listOfItemResponses[0].itemId);
            Assert.AreEqual(true, listOfItemResponses[0].name.Contains(i.sellingWeight));
        }
        [Test]
        public void TestUpdateRecipeYield() {
            var t = new DatabaseAccess();
            var r = new Recipe("bread") { id = 1, yield = 18 };
            var i = new Ingredient("bread flour") { recipeId = 1, ingredientId = 1, measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            r.yield = 36;
            t.UpdateRecipeYield(r);
            var myUpdatedIngredients = t.queryIngredients();
            var myUpdatedRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(18, myRecipeBox[0].yield);
            Assert.AreEqual(1, myIngredients.Count());
            Assert.AreEqual("6 cups", myIngredients[0].measurement);
            Assert.AreEqual(1.06m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(36, myUpdatedRecipeBox[0].yield);
            Assert.AreEqual(1, myUpdatedRecipeBox.Count());
            Assert.AreEqual("12 cups", myUpdatedIngredients[0].measurement);
            Assert.AreEqual(2.12m, myUpdatedIngredients[0].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestUpatedRecipeYieldForMoreThanOneIngredientInARecipe() {
            var t = new DatabaseAccess();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var honey = new Ingredient("Honey") { recipeId = 1, ingredientId = 1, measurement = "1/3 cup", sellingWeight = "32 oz" };
            var salt = new Ingredient("Salt") { recipeId = 1, ingredientId = 2, measurement = "1 teaspoon", sellingWeight = "48 oz" };
            var granulatedSugar = new Ingredient("Granulated Sugar") { recipeId = 1, ingredientId = 3, measurement = "1 teaspoon", sellingWeight = "4 lb" };
            var activeDryYeast = new Ingredient("Active Dry Yeast") { recipeId = 1, ingredientId = 4, measurement = "2 1/4 teaspoons", sellingWeight = "4 oz" };
            var honeyButtermilkBreadIngredients = new List<Ingredient> { honey, salt, granulatedSugar, activeDryYeast };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(honeyButtermilkBreadIngredients, honeyButtermilkBread);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            honeyButtermilkBread.yield = 120;
            t.UpdateRecipeYield(honeyButtermilkBread);
            var myUpdatedIngredients = t.queryIngredients();
            var myUpdatedRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(1, myRecipeBox.Count());
            Assert.AreEqual(4, myIngredients.Count());
            Assert.AreEqual("1/3 cup", myRecipeBox[0].ingredients[0].measurement);
            Assert.AreEqual(.98m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual("1 teaspoon", myRecipeBox[0].ingredients[1].measurement);
            Assert.AreEqual(.01m, myRecipeBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual("1 teaspoon", myRecipeBox[0].ingredients[2].measurement);
            Assert.AreEqual(.01m, myRecipeBox[0].ingredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual("2 1/4 teaspoons", myRecipeBox[0].ingredients[3].measurement);
            Assert.AreEqual(.31m, myRecipeBox[0].ingredients[3].priceOfMeasuredConsumption);
            Assert.AreEqual(1.31m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(1, myUpdatedRecipeBox.Count());
            Assert.AreEqual(4, myUpdatedIngredients.Count());
            Assert.AreEqual("1.625 cups 2 teaspoons", myUpdatedRecipeBox[0].ingredients[0].measurement);
            Assert.AreEqual(4.92m, myUpdatedRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual("1 tablespoons 2 teaspoons", myUpdatedRecipeBox[0].ingredients[1].measurement);
            Assert.AreEqual(.06m, myUpdatedRecipeBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual("1 tablespoons 2 teaspoons", myUpdatedRecipeBox[0].ingredients[2].measurement); //2.44
            Assert.AreEqual(.03m, myUpdatedRecipeBox[0].ingredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual(".125 cups 1 tablespoons 2.25 teaspoons", myUpdatedRecipeBox[0].ingredients[3].measurement);
            Assert.AreEqual(1.56m, myUpdatedRecipeBox[0].ingredients[3].priceOfMeasuredConsumption);
            Assert.AreEqual(6.57m, myUpdatedRecipeBox[0].aggregatedPrice);
        }
        [Test]
        public void Test2RecipesAdjustYieldAndIngredients() {
            var t = new DatabaseAccess();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var cinnamonSwirlBread = new Recipe("Cinnamon Swirl Buttermilk Bread") { id = 2, yield = 18 };
            var honey = new Ingredient("Honey") { recipeId = 1, ingredientId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var activeDryYeast = new Ingredient("Active Dry Yeast") { recipeId = 1, ingredientId = 2, measurement = "2 1/4 teaspoons", sellingWeight = "4 oz", typeOfIngredient = "active dry yeast" };
            var cinnamon = new Ingredient("Cinnamon") { recipeId = 2, ingredientId = 3, measurement = "3 tablespoons", sellingWeight = "8.75 oz", typeOfIngredient = "ground cinnamon" };
            var breadFlour = new Ingredient("Bread Flour") { recipeId = 2, ingredientId = 4, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var honeyButtemrilkBreadIngredients = new List<Ingredient> { honey, activeDryYeast };
            var cinnamonSwirlBreadIngredients = new List<Ingredient> { cinnamon, breadFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(honeyButtemrilkBreadIngredients, honeyButtermilkBread);
            t.insertListOfIngredientsIntoAllTables(cinnamonSwirlBreadIngredients, cinnamonSwirlBread);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            honeyButtermilkBread.yield = 48; //*2
            cinnamonSwirlBread.yield = 54; //*3
            t.UpdateRecipeYield(honeyButtermilkBread);
            t.UpdateRecipeYield(cinnamonSwirlBread);
            var myUpdatedIngredients = t.queryIngredients();
            var myUpdatedRecipBox = t.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(4, myIngredients.Count());
            Assert.AreEqual("1/3 cup", myRecipeBox[0].ingredients[0].measurement);
            Assert.AreEqual(.98m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.31m, myRecipeBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.29m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(.52m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.06m, myRecipeBox[1].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.58m, myRecipeBox[1].aggregatedPrice);
            Assert.AreEqual(2, myUpdatedRecipBox.Count());
            Assert.AreEqual(4, myUpdatedIngredients.Count());
            Assert.AreEqual(".66 cups", myUpdatedRecipBox[0].ingredients[0].measurement);
            //this one is .625  cups 2 teaspoons, but it should be .66
            Assert.AreEqual(1.95m, myUpdatedRecipBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual("1 tablespoons 1.5 teaspoons", myUpdatedRecipBox[0].ingredients[1].measurement);
            Assert.AreEqual(.62m, myUpdatedRecipBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(2.57m, myUpdatedRecipBox[0].aggregatedPrice);
            Assert.AreEqual(".5 cups 1 tablespoons", myUpdatedRecipBox[1].ingredients[0].measurement);
            Assert.AreEqual(1.56m, myUpdatedRecipBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual("18 cups", myUpdatedRecipBox[1].ingredients[1].measurement);
            Assert.AreEqual(3.18m, myUpdatedRecipBox[1].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(4.74m, myUpdatedRecipBox[1].aggregatedPrice);
        }
        [Test]
        public void TestUpdateYieldForListOfRecipes() {
            var t = new DatabaseAccess();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var cinnamonSwirlBread = new Recipe("Cinnamon Swirl Buttermilk Bread") { id = 2, yield = 18 };
            var honey = new Ingredient("Honey") { recipeId = 1, ingredientId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var activeDryYeast = new Ingredient("Active Dry Yeast") { recipeId = 1, ingredientId = 2, measurement = "2 1/4 teaspoons", sellingWeight = "4 oz", typeOfIngredient = "active dry yeast" };
            var cinnamon = new Ingredient("Cinnamon") { recipeId = 2, ingredientId = 3, measurement = "3 tablespoons", sellingWeight = "8.75 oz", typeOfIngredient = "ground cinnamon" };
            var breadFlour = new Ingredient("Bread Flour") { recipeId = 2, ingredientId = 4, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
            var honeyButtemrilkBreadIngredients = new List<Ingredient> { honey, activeDryYeast };
            var cinnamonSwirlBreadIngredients = new List<Ingredient> { cinnamon, breadFlour };
            var myRecipes = new List<Recipe> { honeyButtermilkBread, cinnamonSwirlBread };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(honeyButtemrilkBreadIngredients, honeyButtermilkBread);
            t.insertListOfIngredientsIntoAllTables(cinnamonSwirlBreadIngredients, cinnamonSwirlBread);
            var myIngredients = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            honeyButtermilkBread.yield = 48; //*2
            cinnamonSwirlBread.yield = 54; //*3
            t.UpdateListOfRecipeYields(myRecipes);
            var myUpdatedIngredients = t.queryIngredients();
            var myUpdatedRecipBox = t.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(4, myIngredients.Count());
            Assert.AreEqual("1/3 cup", myRecipeBox[0].ingredients[0].measurement);
            Assert.AreEqual(.98m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.31m, myRecipeBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.29m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(.52m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.06m, myRecipeBox[1].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.58m, myRecipeBox[1].aggregatedPrice);
            Assert.AreEqual(2, myUpdatedRecipBox.Count());
            Assert.AreEqual(4, myUpdatedIngredients.Count());
            Assert.AreEqual(".66 cups", myUpdatedRecipBox[0].ingredients[0].measurement);
            //this one is .625  cups 2 teaspoons, but it should be .66
            Assert.AreEqual(1.95m, myUpdatedRecipBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual("1 tablespoons 1.5 teaspoons", myUpdatedRecipBox[0].ingredients[1].measurement);
            Assert.AreEqual(.62m, myUpdatedRecipBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(2.57m, myUpdatedRecipBox[0].aggregatedPrice);
            Assert.AreEqual(".5 cups 1 tablespoons", myUpdatedRecipBox[1].ingredients[0].measurement);
            Assert.AreEqual(1.56m, myUpdatedRecipBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual("18 cups", myUpdatedRecipBox[1].ingredients[1].measurement);
            Assert.AreEqual(3.18m, myUpdatedRecipBox[1].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(4.74m, myUpdatedRecipBox[1].aggregatedPrice);
        }
        [Test]
        public void TestChangeRecipeYield() {
            //this special dark baking cocoa is so picky!! I almost have to match the direct item response name... there's gotta be a better way to do this...
            var t = new DatabaseAccess();
            var yellowCake = new Recipe("Golden Cake") { id = 1, yield = 12 };
            var marbleCake = new Recipe("Marble Cake") { id = 2, yield = 16 };
            var chocolateCake = new Recipe("Chocolate Cake") { id = 3, yield = 24 };
            var softasilkCakeFlour = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" }; //2.98 .63 .6286
            var bakingSoda = new Ingredient("Baking Soda") { ingredientId = 2, recipeId = 1, measurement = "3/4 teaspoons", sellingWeight = "4 lb", typeOfIngredient = "baking soda" }; // 2.36 8.57  .0049
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Morsels") { ingredientId = 3, recipeId = 2, measurement = "1 3/4 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" }; //3.56 5.35 2.78
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 4, recipeId = 2, measurement = "1 1/2 teaspoons", sellingWeight = "10 oz", typeOfIngredient = "baking powder" }; // 2.9  .0761
            var cocoa = new Ingredient("Special Dark Cocoa") { ingredientId = 5, recipeId = 3, measurement = "1 cup", sellingWeight = "8 oz", typeOfIngredient = "baking cocoa" }; //2.99 1.7368 1.55
            var softasilkFlour2 = new Ingredient("Softasilk Cake Flour") { ingredientId = 6, recipeId = 3, measurement = "3 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" }; //2.98 1.25
            var yellowCakeIngredients = new List<Ingredient> { softasilkCakeFlour, bakingSoda };
            var marbleCakeIngredients = new List<Ingredient> { chocolateChips, bakingPowder };
            var chocolateCakeIngredients = new List<Ingredient> { cocoa, softasilkFlour2 };
            var myCakeRecipes = new List<Recipe> { yellowCake, marbleCake, chocolateCake };
            var myCakeIngredients = new List<Ingredient> { softasilkCakeFlour, bakingSoda, chocolateChips, bakingPowder, cocoa, softasilkFlour2 };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(yellowCakeIngredients, yellowCake);
            t.insertListOfIngredientsIntoAllTables(marbleCakeIngredients, marbleCake);
            t.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            var myIngredients = t.queryIngredients();
            var myCakeRecipeBox = t.MyRecipeBox();
            yellowCake.yield = 150; //  12.5
            marbleCake.yield = 128; //  8
            chocolateCake.yield = 36; //  1.5
            t.UpdateListOfRecipeYields(myCakeRecipes);
            var myUpdatedIngredientBox = t.queryAllTablesForAllIngredients(myCakeIngredients);
            var mySoftasilkFlour = t.queryAllTablesForIngredient(softasilkFlour2);
            var myUpdatedCakeRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(3, myCakeRecipeBox.Count());
            Assert.AreEqual(1.55m, myIngredients[4].priceOfMeasuredConsumption);
            Assert.AreEqual(1.26m, myIngredients[5].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myCakeRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(2.86m, myCakeRecipeBox[1].aggregatedPrice);
            Assert.AreEqual(2.81m, myCakeRecipeBox[2].aggregatedPrice); //1.55 + 1.26
            Assert.AreEqual(3, myUpdatedCakeRecipeBox.Count());
            Assert.AreEqual(6, myUpdatedIngredientBox.Count());
            Assert.AreEqual(1.89m, mySoftasilkFlour.priceOfMeasuredConsumption); //mind you, this is taking the second softasilk flour, which is why i'm using 4.5 cups to measure out the expected here
            Assert.AreEqual(4.5m, myUpdatedIngredientBox[0].density);
            Assert.AreEqual(7.85m, myUpdatedCakeRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(22.84m, myUpdatedCakeRecipeBox[1].aggregatedPrice); //2.33 + 1.89
            Assert.AreEqual(4.22m, myUpdatedCakeRecipeBox[2].aggregatedPrice);
        }
        [Test]
        public void TestInsertIngredientIntoDensityInfoDatabase() {
            var t = new DatabaseAccess();
            var r = new Recipe("Sample") { id = 1 };
            var i = new Ingredient("Softasilk Flour") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var i2 = new Ingredient("Ground Ginger") { ingredientId = 2, recipeId = 1, measurement = "1 teaspoon", sellingWeight = "8 oz", typeOfIngredient = "ground ginger", density = 2.93m };
            var myIngredients = new List<Ingredient> { i, i2 };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            t.insertIngredientIntoAllTables(i2, r);
            var myIngredientBoxDensities = t.queryDensityInfoTable();
            var myIngredientBox = t.queryAllTablesForAllIngredients(myIngredients);
            Assert.AreEqual("ground ginger", myIngredientBoxDensities[41].name);
            Assert.AreEqual(2.93m, myIngredientBoxDensities[41].density);
            Assert.AreEqual(4.5m, myIngredientBoxDensities[2].density);
            Assert.AreEqual(4.5m, myIngredientBox[0].density);
            Assert.AreEqual(2.93m, myIngredientBox[1].density);
        }
        [Test]
        public void TestMultipleIngredientsWithTheSameName() {
            var t = new DatabaseAccess();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 24 };
            var yellowCake = new Recipe("Yellow Cake") { id = 2, yield = 16 };
            var strawberryShortcake = new Recipe("Strawberry Shortcake") { id = 3, yield = 8 };
            var softasilkFlour1 = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "1 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour2 = new Ingredient("Softasilk Cake Flour") { ingredientId = 2, recipeId = 1, measurement = "2 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour3 = new Ingredient("Softasilk Cake Flour") { ingredientId = 3, recipeId = 1, measurement = "3 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour4 = new Ingredient("Softasilk Cake Flour") { ingredientId = 4, recipeId = 2, measurement = "1/2 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour5 = new Ingredient("Softasilk Cake Flour") { ingredientId = 5, recipeId = 2, measurement = "1 1/2 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour6 = new Ingredient("Softasilk Cake Flour") { ingredientId = 6, recipeId = 2, measurement = "2 1/2 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour7 = new Ingredient("Softasilk Cake Flour") { ingredientId = 7, recipeId = 3, measurement = "2 1/4 cup", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour8 = new Ingredient("Softasilk Cake Flour") { ingredientId = 8, recipeId = 3, measurement = "1 tablespoon", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var softasilkFlour9 = new Ingredient("Softasilk Cake Flour") { ingredientId = 9, recipeId = 3, measurement = "1 tablespoon 2 teaspoons", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var chocolateCakeIngredients = new List<Ingredient> { softasilkFlour1, softasilkFlour2, softasilkFlour3 };
            var yellowCakeIngredients = new List<Ingredient> { softasilkFlour4, softasilkFlour5, softasilkFlour6 };
            var strawberryShortcakeIngredients = new List<Ingredient> { softasilkFlour7, softasilkFlour8, softasilkFlour9 };
            var myRecipes = new List<Recipe> { chocolateCake, yellowCake, strawberryShortcake };
            var myIngredients = new List<Ingredient> { softasilkFlour1, softasilkFlour2, softasilkFlour3, softasilkFlour4, softasilkFlour5, softasilkFlour6, softasilkFlour7, softasilkFlour8, softasilkFlour9 };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(chocolateCakeIngredients, chocolateCake);
            t.insertListOfIngredientsIntoAllTables(yellowCakeIngredients, yellowCake);
            t.insertListOfIngredientsIntoAllTables(strawberryShortcakeIngredients, strawberryShortcake);
            var myIngredientBoxDensities = t.queryDensityInfoTable();
            var myIngredientBox = t.queryAllTablesForAllIngredients(myIngredients);
            Assert.AreEqual(4.5m, myIngredientBox[0].density);
            Assert.AreEqual(4.5m, myIngredientBox[1].density);
            Assert.AreEqual(4.5m, myIngredientBox[2].density);
            Assert.AreEqual(4.5m, myIngredientBox[3].density);
            Assert.AreEqual(4.5m, myIngredientBox[4].density);
            Assert.AreEqual(4.5m, myIngredientBox[5].density);
            Assert.AreEqual(4.5m, myIngredientBox[6].density);
            Assert.AreEqual(4.5m, myIngredientBox[7].density);
            Assert.AreEqual(4.5m, myIngredientBox[8].density);
            Assert.AreEqual(.42m, myIngredientBox[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.84m, myIngredientBox[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.26m, myIngredientBox[2].priceOfMeasuredConsumption);
            Assert.AreEqual(.21m, myIngredientBox[3].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngredientBox[4].priceOfMeasuredConsumption);
            Assert.AreEqual(1.05m, myIngredientBox[5].priceOfMeasuredConsumption);
            Assert.AreEqual(.94m, myIngredientBox[6].priceOfMeasuredConsumption);
            Assert.AreEqual(.03m, myIngredientBox[7].priceOfMeasuredConsumption);
            Assert.AreEqual(.04m, myIngredientBox[8].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestOverwritingTheCostTable() {
            var t = new DatabaseAccess();
            var yellowCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var milk = new Ingredient("Whole Milk") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "1 gallon", typeOfIngredient = "milk", classification = "dairy" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(milk, yellowCake);
            var myIngredient = t.queryAllTablesForIngredient(milk);
            milk.sellingPrice = 2.98m;
            t.updateCostDataTable(milk);
            var myCostIngredient = t.queryCostTable();
            t.updateAllTables(milk, yellowCake);
            var myUpdatedIngredient = t.queryAllTablesForIngredient(milk);
            var myRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(1, myRecipeBox[0].ingredients.Count());
            Assert.AreEqual(2.98m, myUpdatedIngredient.sellingPrice);
            Assert.AreEqual(8.2m, myIngredient.density);
            Assert.AreEqual(.19m, myUpdatedIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(.0233m, myUpdatedIngredient.pricePerOunce);
        }
        [Test]
        public void TestOverwritingTheCostTable2() {
            var t = new DatabaseAccess();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 24 };
            var yellowCake = new Recipe("Yellow Cake") { id = 2, yield = 24 };
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 3, yield = 24 };
            var sourCream = new Ingredient("Sour Cream") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "16 oz", typeOfIngredient = "sour cream", classification = "dairy" };
            var milk = new Ingredient("Whole Milk") { ingredientId = 2, recipeId = 2, measurement = "1 1/2 cups", sellingWeight = "1 gallon", typeOfIngredient = "milk", classification = "dairy" };
            var butter = new Ingredient("Unsalted Butter") { ingredientId = 3, recipeId = 3, measurement = "1/4 cup", sellingWeight = "1 lb", typeOfIngredient = "butter", classification = "dairy" };
            var buttermilk = new Ingredient("Buttermilk") { ingredientId = 4, recipeId = 3, measurement = "2 cups", sellingWeight = "1/4 gallon", typeOfIngredient = "buttermilk", classification = "dairy" };
            var honeyButtermilkBreadIngredients = new List<Ingredient> { butter, buttermilk };
            var allIngredients = new List<Ingredient> { sourCream, milk, butter, buttermilk };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(sourCream, chocolateCake);
            t.insertIngredientIntoAllTables(milk, yellowCake);
            t.insertListOfIngredientsIntoAllTables(honeyButtermilkBreadIngredients, honeyButtermilkBread);
            var myIngredients = t.queryAllTablesForAllIngredients(allIngredients);
            sourCream.sellingPrice = 1.69m;
            milk.sellingPrice = 2.98m;
            butter.sellingPrice = 3.99m;
            buttermilk.sellingPrice = 1.69m;
            var costTable = t.queryCostTable();
            t.updateAllTables(sourCream, chocolateCake);
            t.updateAllTables(milk, yellowCake);
            t.updateAllTablesForAllIngredients(honeyButtermilkBreadIngredients, honeyButtermilkBread);
            var myUpdatedIngredients = t.queryAllTablesForAllIngredients(allIngredients);
            var myRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(1.69m, myUpdatedIngredients[0].sellingPrice);
            Assert.AreEqual(2.98m, myUpdatedIngredients[1].sellingPrice);
            Assert.AreEqual(3.99m, myUpdatedIngredients[2].sellingPrice);
            Assert.AreEqual(1.69m, myUpdatedIngredients[3].sellingPrice);
            Assert.AreEqual(8.6m, myUpdatedIngredients[0].density);
            Assert.AreEqual(8.2m, myUpdatedIngredients[1].density);
            Assert.AreEqual(8m, myUpdatedIngredients[2].density);
            Assert.AreEqual(8.2m, myUpdatedIngredients[3].density);
            Assert.AreEqual(.91m, myUpdatedIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.91m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.29m, myUpdatedIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.29m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.5m, myUpdatedIngredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual(.5m, myRecipeBox[2].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.87m, myUpdatedIngredients[3].priceOfMeasuredConsumption);
            Assert.AreEqual(.87m, myRecipeBox[2].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.91m, myRecipeBox[0].aggregatedPrice);
            Assert.AreEqual(.29m, myRecipeBox[1].aggregatedPrice);
            Assert.AreEqual(1.37m, myRecipeBox[2].aggregatedPrice);
        }
        [Test]
        public void TestOverwritingTheCostTable3() {
            var t = new DatabaseAccess();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 16 };
            var softasilk = new Ingredient("Softasilk Flour") { recipeId = 1, ingredientId = 1, sellingWeight = "32 oz", measurement = "3 cups", typeOfIngredient = "cake flour", classification = "flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(softasilk, chocolateCake);
            var myIngredient = t.queryAllTablesForIngredient(softasilk);
            softasilk.sellingPrice = 5m;
            var costTable = t.queryCostTable();
            t.updateAllTables(softasilk, chocolateCake);
            var myUpdatedIngredient = t.queryAllTablesForIngredient(softasilk);
            var myRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(1.26m, myIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(2.11m, myUpdatedIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(2.11m, myRecipeBox[0].aggregatedPrice);
        }
        [Test]
        public void TestDeleteIngredientFromIngredientsTable() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 1, recipeId = 1, classification = "flour", typeOfIngredient = "bread flour", measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredient(breadFlour, bread);
            var myIngredients = t.queryIngredients();
            t.DeleteIngredientFromIngredientTable(breadFlour);
            var myUpdatedIngredients = t.queryIngredients();
            Assert.AreEqual(1, myIngredients.Count());
            Assert.AreEqual(0, myUpdatedIngredients.Count());
        }
        [Test]
        public void TestDeleteIngredientFromCostTable() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, typeOfIngredient = "honey", measurement = "1/3 cup", sellingWeight = "32 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = t.MyRecipeBox();
            var myIngredients = t.queryAllTablesForIngredient(honey);
            t.DeleteIngredientFromCostTable(honey);
            var myCostIngredients = t.queryCostTable();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myCostIngredients.Count());
        }
        [Test]
        public void TestDeleteIngredientFromDensitiesTable() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = t.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            t.DeleteIngredientFromDensitiesTable(honey);
            var myDensitiesIngredients = t.queryDensitiesTable();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myDensitiesIngredients.Count());
        }
        [Test]
        public void TestDeleteIngredeintFromConsumptionTable() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = t.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            t.DeleteIngredientFromConsumptionTable(honey);
            var myConsumptionTable = t.queryConsumptionTable();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myConsumptionTable.Count());
        }
        [Test]
        public void TestDeleteIngredientFromAllTablesNeeded() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, typeOfIngredient = "honey", measurement = "2 tablespoons", sellingWeight = "32 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = t.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            t.DeleteIngredientFromAllRelevantTables(honey);
            var myIngredientsTable = t.queryIngredients();
            var myCostTable = t.queryCostTable();
            var myDensitiesTable = t.queryDensitiesTable();
            var myConsumptionTable = t.queryConsumptionTable();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myIngredientsTable.Count());
            Assert.AreEqual(0, myCostTable.Count());
            Assert.AreEqual(0, myDensitiesTable.Count());
            Assert.AreEqual(0, myConsumptionTable.Count());
        }
        [Test]
        public void TestDeleteRecipeAndRecipeIngredients() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, measurement = "2/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = t.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            t.DeleteRecipeAndRecipeIngredients(bread);
            var myUpdatedIngredients = t.queryIngredients();
            var myUpdatedRecipeBox = t.MyRecipeBox();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myUpdatedRecipeBox.Count());
            Assert.AreEqual(0, myUpdatedIngredients.Count());
        }
        [Test]
        public void TestItemResponseInformationInConsumptionTable() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, sellingWeight = "32 oz", measurement = "2 tablespoons", typeOfIngredient = "honey" };
            //i would eventually like for this to be a drop down menu, to show all options for the typeOfIngredient in the density table}
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = t.MyRecipeBox();
            var myIngredient = t.queryAllTablesForIngredient(honey);
            var myConsumptionTable = t.queryConsumptionTable();
            Assert.AreEqual(.37m, myIngredient.priceOfMeasuredConsumption);
        }
        [Test]
        public void TestOuncesRemainingAndConsumedInConsumptionTable() {
            var t = new DatabaseAccess();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, sellingWeight = "32 oz", measurement = "1/3 cup", typeOfIngredient = "honey" };
            var breadFlour = new Ingredient("bread flour") { ingredientId = 2, recipeId = 1, sellingWeight = "5 lb", measurement = "6 cups", typeOfIngredient = "bread flour" };
            var breadIngredients = new List<Ingredient> { honey, breadFlour };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(breadIngredients, bread);
            var myConsumptionTable = t.queryConsumptionTable();
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
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1 };
            var cinnamonSwirlBread = new Recipe("Cinnamon Swirl Bread") { id = 2 };
            var honey = new Ingredient("honey") { ingredientId = 1, recipeId = 1, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var honey2 = new Ingredient("honey") { ingredientId = 2, recipeId = 2, measurement = "1 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            var honeyListOfIngredients = new List<Ingredient> { honey, honey2 };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, honeyButtermilkBread);
            var honeyIngInfo = t.queryConsumptionTable();
            t.insertIngredientIntoAllTables(honey2, cinnamonSwirlBread);
            var honey2IngInfo = t.queryConsumptionTable();
            var myRecipes = t.MyRecipeBox();
            var honeyAllTablesInfo = t.queryAllTablesForIngredient(honey);
            var honey2AllTablesInfo = t.queryAllTablesForIngredient(honey2);
            var myIngredientBox = t.queryAllTablesForAllIngredients(honeyListOfIngredients);
            var myConsumptionTable = t.queryConsumptionTable();
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
            var myConsumptionTable = t.queryConsumptionTable();
            t.insertListOfIngredientsIntoAllTables(cinnBreadIngredients, cinnamonSwirlBread);
            var myUpdatedConsumptionTable = t.queryConsumptionTable();
            var myCinnamonSwirlBreadIngredients = t.queryAllTablesForAllIngredients(cinnBreadIngredients);
            var myRecipes = t.MyRecipeBox();
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
            var consumptionTable1 = t.queryConsumptionTable();
            t.insertIngredientIntoAllTables(breadFlour2, bread2);
            var consumptionTable2 = t.queryConsumptionTable();
            t.insertIngredientIntoAllTables(breadFlour3, bread3);
            var consumptionTable3 = t.queryConsumptionTable();
            t.insertIngredientIntoAllTables(breadFlour4, bread4);
            var consumptionTable4 = t.queryConsumptionTable();
            var myRecipeBox = t.MyRecipeBox();
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
            var chocolateChipCookies = new Recipe("Chocolate Chip Cookies") { id = 1 };
            var chocolateChips = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "1 3/4 cups", sellingWeight = "12 oz", typeOfIngredient = "chocolate chips" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(chocolateChips, chocolateChipCookies);
            var myConsumptionTable = t.queryConsumptionTable();
            var myIngredientData = t.queryAllTablesForIngredient(chocolateChips);
            t.refillIngredientInConsumptionDatabase(chocolateChips, "24 oz");
            var myUpdatedConsumptionTable = t.queryConsumptionTable();
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
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1 };
            var buttermilk = new Ingredient("Buttermilk") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "1 quart", typeOfIngredient = "buttermilk", sellingPrice = 1.79m, classification = "dairy" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(buttermilk, honeyButtermilkBread);
            var myConsumptionTable = t.queryConsumptionTable();
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
            var fluffyWhiteCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var eggWhites = new Ingredient("egg whites, stiffly beaten") { ingredientId = 1, recipeId = 1, measurement = "3 egg whites", sellingWeight = "1 dozen", sellingPrice = 2.99m, typeOfIngredient = "egg", classification = "egg" }; //for the record, one egg white merginued does not equal 1.70 oz to my knowledge.. should be decently lighter
            var eggs = new Ingredient("Eggs") { ingredientId = 2, recipeId = 1, measurement = "2 eggs", sellingWeight = "1 dozen", sellingPrice = 2.99m, typeOfIngredient = "egg", classification = "egg" };
            var fluffyWhiteCakeEggIngredients = new List<Ingredient> { eggWhites, eggs };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeEggIngredients, fluffyWhiteCake);
            var myIngredients = t.queryAllTablesForAllIngredients(fluffyWhiteCakeEggIngredients);
            var myRecipes = t.MyRecipeBox();
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
            var myRecipeBox = t.MyRecipeBox();
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
        public void TestPricePerServing() {
            var t = new DatabaseAccess();
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield = 16 };
            var cakeFlour = new Ingredient("Softasilk") { ingredientId = 1, recipeId = 1, measurement = "2 cups 2 tablespoons", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var vanilla = new Ingredient("Vanilla Extract") { ingredientId = 2, recipeId = 1, measurement = "1 1/2 teaspoons", typeOfIngredient = "vanilla extract", sellingWeight = "16 oz" }; //27.59
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 3, recipeId = 1, measurement = "2 teaspoons", typeOfIngredient = "baking powder", sellingWeight = "10 oz" }; //2.90
            var fluffyWhiteCakeIngredients = new List<Ingredient> { cakeFlour, vanilla, bakingPowder };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeIngredients, fluffyWhiteCake);
            var myIngredients = t.queryAllTablesForAllIngredients(fluffyWhiteCakeIngredients);
            var myrecipeBox = t.MyRecipeBox();
            Assert.AreEqual(.89m, myrecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.36m, myrecipeBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.10m, myrecipeBox[0].ingredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual(1.35m, myrecipeBox[0].aggregatedPrice);
            Assert.AreEqual(16, myrecipeBox[0].yield);
            Assert.AreEqual(.08m, myrecipeBox[0].pricePerServing);
        }
        [Test]
        public void TestGettingAllDistictIngredientsFromQueryIngredients() {
            var t = new DatabaseAccess();
            var fluffyWhiteCake = new Recipe("White Cake") { id = 1 };
            var cakeflour = new Ingredient("Softasilk") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var cakeFlour2 = new Ingredient("Softasilk") { ingredientId = 2, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var fluffyWhiteCakeIngredients = new List<Ingredient> { cakeflour, cakeFlour2 };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeIngredients, fluffyWhiteCake);
        }
        [Test]
        public void TestGettingUniqueEntriesINIngredientTable() {
            var t = new DatabaseAccess();
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield = 12};
            var chocolateCake = new Recipe("My Favorite Chocolat Cake") { id = 2,yield= 18 };
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
            var myIngredientsTable = t.queryIngredients();
            var myRecipeBox = t.MyRecipeBox();
            var myDistictIngredientTable = t.getListOfDistintIngredients();
            var myIngredientBoxFilled = t.queryAllTablesForAllIngredients(myIngredientBox); 
            Assert.AreEqual(4, myIngredientsTable.Count());
            Assert.AreEqual(2, myDistictIngredientTable.Count());
            Assert.AreEqual("Softasilk Cake Flour", myDistictIngredientTable[0].name);
            Assert.AreEqual("Baking Powder", myDistictIngredientTable[1].name);
            Assert.AreEqual(.89m, softasilk.priceOfMeasuredConsumption);
            Assert.AreEqual(1.26m, softasilk2.priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, softasilk3.priceOfMeasuredConsumption);
            Assert.AreEqual(.10m, bakingPowder.priceOfMeasuredConsumption); 
        }
        //[Test]
        //public void TestingConsumptionTableWithMoreIngredientsMoreExtensiveTest() {
        //    var t = new DatabaseAccess();
        //    t.initializeDatabase();
        //    var chocolateChipCookies = new Recipe("Chocolate Chip Cookies") { id = 1 };
        //    var chocolateBananaBread = new Recipe("Chocolate Banana Bread") { id = 2 };
        //    var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 3 };
        //    var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 4 };
        //    //---
        //    var chocolateChips = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 1, recipeId = 1, measurement = "1 3/4 cups", sellingWeight = "12 oz", typeOfIngredient = "chocoalte chips" };
        //    var APFlour = new Ingredient("All Purpose Flour") { ingredientId = 2, recipeId = 1, measurement = "1 cup 2 tablespoons", sellingWeight = "5 lb", typeOfIngredient = "all purpose flour" };
        //    var bakingSoda = new Ingredient("Baking Soda") { ingredientId = 3, recipeId = 1, measurement = "3/4 teaspoon", sellingWeight = "4 lb", typeOfIngredient = "baking soda" };
        //    var salt = new Ingredient("Salt") { ingredientId = 4, recipeId = 1, measurement = "1 teaspoon", sellingWeight = "48 oz", typeOfIngredient = "salt" };
        //    var brownSugar = new Ingredient("Brown Sugar") { ingredientId = 5, recipeId = 1, measurement = "1/2 cup", sellingWeight = "2 lb", typeOfIngredient = "brown sugar" };
        //    var granSugar = new Ingredient("Granualted Sugar") { ingredientId = 6, recipeId = 1, measurement = "1/2 cup", sellingWeight = "4 lb", typeOfIngredient = "white sugar" };
        //    //---
        //    t.refillIngredientInConsumptionDatabase(chocolateChips, "12 oz");
        //    var chocolateChips2 = new Ingredient("Semi Sweet Chocolate Chips") { ingredientId = 7, recipeId = 2, measurement = "2 cups", sellingWeight = "12 oz", typeOfIngredient = "chocoalte chips" };
        //    var bananas = new Ingredient("Mashed Bananas") { ingredientId = 8, recipeId = 2, measurement = "2 cups", sellingWeight = "1 lb", sellingPrice = .49m, typeOfIngredient = "bananas, mashed" };
        //    var cocoa = new Ingredient("Unsweetened Cocoa") { ingredientId = 9, recipeId = 2, measurement = "3/4 cup", sellingWeight = "16 oz", typeOfIngredient = "baking cocoa" };
        //    var APFlour2 = new Ingredient("All Purpose Flour") { ingredientId = 10, recipeId = 2, measurement = "2 cups", sellingWeight = "5 lb", typeOfIngredient = "flour" };
        //    var granSugar2 = new Ingredient("Granualted Sugar") { ingredientId = 11, recipeId = 2, measurement = "1 1/2 cups", sellingWeight = "4 lb", typeOfIngredient = "white sugar" };
        //    //---
        //    var honey = new Ingredient("Honey") { ingredientId = 12, recipeId = 3, measurement = "1/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
        //    var breadFlour = new Ingredient("Bread Flour") { ingredientId = 13, recipeId = 3, measurement = "6 cups", sellingWeight = "5 lb", typeOfIngredient = "bread flour" };
        //    var salt2 = new Ingredient("Salt") { ingredientId = 14, recipeId = 3, measurement = "1 tablespoon", sellingWeight = "48 oz", typeOfIngredient = "salt" };
        //    var granSugar3 = new Ingredient("Granulated Sugar") { ingredientId = 15, recipeId = 3, measurement = "1 tablespoon", sellingWeight = "4 lb", typeOfIngredient = "white sugar" };
        //    var buttermilk = new Ingredient("Buttermilk") { ingredientId = 16, recipeId = 3, measurement = "2 cups", sellingWeight = "1 quart", sellingPrice = 1.69m, typeOfIngredient = "buttermilk" };
        //    //---
        //    var cakeFlour = new Ingredient("Softasilk") { ingredientId = 17, recipeId = 4, measurement = "2 cups 2 tablespoons", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
        //    var butter = new Ingredient("Butter") { ingredientId = 18, recipeId = 4, measurement = "1/2 cup", sellingWeight = "1 lb", sellingPrice = 3.99m, typeOfIngredient = "butter" };
        //    var granSugar4 = new Ingredient("Granulated Sugar") { ingredientId = 19, recipeId = 4, measurement = "1 1/2 cups", sellingWeight = "4 lb", typeOfIngredient = "white sugar" };
        //     var salt3 = new Ingredient("Salt") { ingredientId = 20, recipeId = 4, measurement = "1 teaspoon", sellingWeight = "48 oz", typeOfIngredient = "salt" };
        //    var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 21, recipeId =4, measurement = "1 1/2 teaspoons", sellingWeight = "10 0z", typeOfIngredient = "baking powder" };
        //    var vanilla = new Ingredient("Vanilla Extract") { ingredientId = 22, recipeId = 4, measurement = "1 tablespoon", sellingWeight = "4 oz", typeOfIngredient = "vanilla extract" };
        //    //---
        //    var chocolateChipCookiesRecipeIngredients = new List<Ingredient> { chocolateChips, APFlour, bakingSoda, salt, brownSugar, granSugar };
        //    var chocolateBananaBreadRecipeIngredients = new List<Ingredient> { chocolateChips2, bananas, cocoa, APFlour2, granSugar2 };
        //    var honeyButtermilkBreadRecipeIngredients = new List<Ingredient> { honey, breadFlour, salt2, granSugar3, buttermilk };
        //    var fluffywhiteCakeRecipeIngredients = new List<Ingredient> { cakeFlour, butter, granSugar4, salt3, bakingPowder, vanilla };
        //    t.insertListOfIngredientsIntoAllTables(chocolateBananaBreadRecipeIngredients, chocolateChipCookies);
        //    t.insertListOfIngredientsIntoAllTables(chocolateBananaBreadRecipeIngredients, chocolateBananaBread);
        //    t.insertListOfIngredientsIntoAllTables(honeyButtermilkBreadRecipeIngredients, honeyButtermilkBread);
        //    t.insertListOfIngredientsIntoAllTables(fluffywhiteCakeRecipeIngredients, fluffyWhiteCake);
        //    var myconsumptionTable = t.queryConsumptionTable();
        //    var myRecipes = t.MyRecipeBox();
        //    var myChocolateChipCookieIngredientDataInfo = t.queryAllTablesForAllIngredients(chocolateChipCookiesRecipeIngredients);
        //    var myChocolateBananaBreadIngredientDataInfo = t.queryAllTablesForAllIngredients(chocolateBananaBreadRecipeIngredients);
        //    var myHoneyButtermilkBreadIngredientDataInfo = t.queryAllTablesForAllIngredients(honeyButtermilkBreadRecipeIngredients);
        //    var myFluffyWhiteCakeIngredientDataInfo = t.queryAllTablesForAllIngredients(fluffywhiteCakeRecipeIngredients);
        //    Assert.AreEqual(4, myRecipes.Count());
        //    //these should accumulate, just commend them out as you and keep tabs of what ingredient line is what in these tests, aggregate the ounces consumed
        //    Assert.AreEqual(9.36m, myChocolateChipCookieIngredientDataInfo[0].ouncesConsumed); //chocolate chips
        //    Assert.AreEqual(6.25m, myChocolateChipCookieIngredientDataInfo[1].ouncesConsumed); //all purpose flour
        //    Assert.AreEqual(.13m, myChocolateChipCookieIngredientDataInfo[2].ouncesConsumed); //baking soda
        //    Assert.AreEqual(.22m, myChocolateChipCookieIngredientDataInfo[3].ouncesConsumed); //salt
        //    Assert.AreEqual(3.88m, myChocolateChipCookieIngredientDataInfo[4].ouncesConsumed); //brown sugar    
        //    Assert.AreEqual(3.55m, myChocolateChipCookieIngredientDataInfo[5].ouncesConsumed); //gran sugar
        //    Assert.AreEqual(20.06m, myChocolateBananaBreadIngredientDataInfo[0].ouncesConsumed); //chocolate chips 20.06+9.36
        //    Assert.AreEqual(24m, myChocolateBananaBreadIngredientDataInfo[1].ouncesConsumed); //bananas
        //    Assert.AreEqual(3.12m, myChocolateBananaBreadIngredientDataInfo[2].ouncesConsumed); //cocoa powder
        //    Assert.AreEqual(10m, myChocolateBananaBreadIngredientDataInfo[3].ouncesConsumed); //all purpose flour 6.25 + 10
        //    Assert.AreEqual(10.65m, myChocolateBananaBreadIngredientDataInfo[4].ouncesConsumed); //gran sugar
        //    Assert.AreEqual(4m, myHoneyButtermilkBreadIngredientDataInfo[0].ouncesConsumed); // honey
        //    Assert.AreEqual(32.4m, myHoneyButtermilkBreadIngredientDataInfo[1].ouncesConsumed); //bread flour
        //    Assert.AreEqual(.67m, myHoneyButtermilkBreadIngredientDataInfo[2].ouncesConsumed); //salt .22 + .67 + .22
        //    Assert.AreEqual(.44m, myHoneyButtermilkBreadIngredientDataInfo[3].ouncesConsumed); //gran sugar 3.55 + .44 + 10.65
        //    Assert.AreEqual(16.4m, myHoneyButtermilkBreadIngredientDataInfo[4].ouncesConsumed); //buttermilk
        //    Assert.AreEqual(9.28m, myFluffyWhiteCakeIngredientDataInfo[0].ouncesConsumed); //cake flour 
        //    Assert.AreEqual(4m, myFluffyWhiteCakeIngredientDataInfo[1].ouncesConsumed); //butter
        //    Assert.AreEqual(10.65m, myFluffyWhiteCakeIngredientDataInfo[2].ouncesConsumed); //gran sugar
        //    Assert.AreEqual(.22m, myFluffyWhiteCakeIngredientDataInfo[3].ouncesConsumed); //salt
        //    Assert.AreEqual(.26m, myFluffyWhiteCakeIngredientDataInfo[4].ouncesConsumed); //baking powder
        //    Assert.AreEqual(.43m, myFluffyWhiteCakeIngredientDataInfo[5].ouncesConsumed); //vanilla extract

        //I would like something like this to be the final test... 
        //}
    }
}
