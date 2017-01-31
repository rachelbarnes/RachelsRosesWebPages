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
    public class DatabaseAccessRecipeTests {
        [Test]
        public void TestSeveralRecipes() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            t.initializeDatabase();
            var r = new Recipe("test") {
                yield = 4
            };
            dbR.InsertRecipe(r);
            r = new Recipe("other") {
                yield = 1
            };
            dbR.InsertRecipe(r);
            var returns = dbR.queryRecipes();
            Assert.AreEqual(2, returns.Count());
            Assert.AreEqual(1, returns[0].id);
        }
        [Test]
        public void TestDeleteRecipe() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("Pecan Pie") {
                yield = 8
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            dbR.DeleteRecipeAndRecipeIngredients(r);
            var myRecipes = dbR.queryRecipes();
            Assert.AreEqual(0, myRecipes.Count());
            Assert.AreEqual(false, myRecipes.Contains(r));
        }
        [Test]
        public void CompileRecipeAndProperIngredients() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            dbR.InsertRecipe(r);
            dbR.InsertRecipe(r2);
            var myRecipes = dbR.queryRecipes();
            r = myRecipes.First(x => x.yield == 8);
            r2 = myRecipes.First(y => y.yield == 16);
            dbI.insertIngredient(i, r);
            dbI.insertIngredient(i2, r2);
            Recipe returnedRecipe = dbR.GetFullRecipe(r);
            Assert.AreEqual("flour", returnedRecipe.ingredients.First().name);
            Assert.AreEqual(1, returnedRecipe.ingredients.Count());
        }
        [Test]
        public void TestProperRecipeIds() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("White Cake");
            var r2 = new Recipe("Pecan Pie");
            var r3 = new Recipe("Cranberry Swirl Loaf");
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            dbR.InsertRecipe(r2);
            dbR.InsertRecipe(r3);
            var myRecipes = dbR.queryRecipes();
            Assert.AreEqual(1, myRecipes[0].id);
            Assert.AreEqual(2, myRecipes[1].id);
            Assert.AreEqual(3, myRecipes[2].id);
        }
        [Test]
        public void TestEditRecipeName() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("White Cake") {
                id = 1
            };
            //this id here, this "for what" is a very important part of this that I forgot
            var newRecipeName = "Fluffy White Cake";
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            r.name = newRecipeName;
            dbR.UpdateRecipe(r);
            var myRecipeBox = dbR.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
        }
        [Test]
        public void TestEditRecipeName2() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
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
            dbR.InsertRecipe(r);
            dbR.InsertRecipe(r2);
            dbR.InsertRecipe(r3);
            dbR.UpdateRecipe(r);
            dbR.UpdateRecipe(r2);
            dbR.UpdateRecipe(r3);
            var myRecipeBox = dbR.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
            Assert.AreEqual(newRecipe2Name, myRecipeBox[1].name);
            Assert.AreEqual(newRecipe3Name, myRecipeBox[2].name);
        }
        [Test]
        public void TestEditRecipeName3() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("Cranberry Swirl Bread") {
                //as a note to self, assign this id with the identity insert (so make sure it's the next chronological id for the recipe in the initialized table
                id = 1
            };
            var newRecipeName = "Cranberry Apple Bread";
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            r.name = newRecipeName;
            dbR.UpdateRecipe(r);
            var myRecipeBox = dbR.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
        }
        [Test]
        public void TestUpdateRecipe() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("My Favorite White Cake") {
                id = 1
            };
            var updatedRecipeName = "My Fluffy White Cake";
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            var myRecipeBoxBefore = dbR.queryRecipes();
            r.name = updatedRecipeName;
            dbR.UpdateRecipe(r);
            var myRecipeBoxAfterwards = dbR.queryRecipes();
            Assert.AreEqual("My Favorite White Cake", myRecipeBoxBefore[0].name);
            Assert.AreEqual(r.name, myRecipeBoxAfterwards[0].name);
        }
        [Test]
        public void TestInsertingYield() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("Honey Buttermilk Bread") {
                id = 1,
                yield = 0
            };
            var r2 = new Recipe("Fluffy White Cake") {
                id = 2,
                yield = 0
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            dbR.InsertRecipe(r2);
            r.yield = 24;
            r2.yield = 16;
            dbR.UpdateRecipe(r);
            dbR.UpdateRecipe(r2);
            var myRecipeBox = dbR.queryRecipes();
            Assert.AreEqual(24, myRecipeBox[0].yield);
            Assert.AreEqual(16, myRecipeBox[1].yield);
        }
        [Test]
        public void TestGetFullRecipe() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            dbR.InsertRecipe(r);
            dbI.insertIngredient(i, r);
            dbI.insertIngredient(i2, r);
            var myRecipeBox = dbR.queryRecipes();
            var myIngredientBox = dbI.queryIngredients();
            var myRecipe = dbR.GetFullRecipe(r);
            Assert.AreEqual(2, myIngredientBox.Count());
            Assert.AreEqual(2, myRecipe.ingredients.Count());
            Assert.AreEqual(i.name, myRecipe.ingredients[0].name);
            Assert.AreEqual(i.measurement, myRecipe.ingredients[0].measurement);
            Assert.AreEqual(i2.name, myRecipe.ingredients[1].name);
            Assert.AreEqual(i2.measurement, myRecipe.ingredients[1].measurement);
        }
        [Test]
        public void TestAggregatedPriceInRecipesTable() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("Sour Cream Chocolate Cake") {
                id = 1,
                aggregatedPrice = 4.53m
            };
            var i = new Ingredient("Hershey's Special Dark Cocoa") {
                recipeId = 1,
                ingredientId = 1
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            var myRecipes = dbR.queryRecipes();
            Assert.AreEqual(4.53m, myRecipes[0].aggregatedPrice);
        }
        [Test]
        public void TestAggregatedPriceInRecipesTableUpdate() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var r = new Recipe("Sour Cream Chocolate Cake") {
                id = 1
            };
            t.initializeDatabase();
            dbR.InsertRecipe(r);
            r.aggregatedPrice = 4.53m;
            dbR.UpdateRecipe(r);
            var myRecipes = dbR.queryRecipes();
            Assert.AreEqual(r.aggregatedPrice, myRecipes[0].aggregatedPrice);
            Assert.AreEqual(4.53m, myRecipes[0].aggregatedPrice);
        }
        [Test]
        public void TestAggregatedRecipePrice() {
            var t = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbR = new DatabaseAccessRecipe();
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
            dbR.GetFullRecipePrice(r);
            var myIngInfo = dbI.queryIngredients();
            var myRecipesInfo = dbR.queryRecipes();
            Assert.AreEqual(2, myIngInfo.Count());
            Assert.AreEqual(1, myRecipesInfo.Count());
            Assert.AreEqual(.87m, myIngInfo[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngInfo[1].priceOfMeasuredConsumption);
            Assert.AreEqual(1.50m, myRecipesInfo[0].aggregatedPrice);
        }
        [Test]
        public void TestAggregatedChocolateChipRecipeConsumedPrice() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
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
            var myRecieBox = dbR.MyRecipeBox();
            Assert.AreEqual(1, myRecieBox.Count());
            Assert.AreEqual(1.84m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.29m, myRecieBox[0].aggregatedPrice);
        }
        [Test]
        public void TestWinterCookiesAggregatedPrice() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
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
            var myRecipes = dbR.GetFullRecipe(winterCookies);
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
            var dbR = new DatabaseAccessRecipe();
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
            dbR.GetFullRecipePrice(winterCookies);
            dbR.GetFullRecipePrice(chocolateChipCookies);
            var myWinterCookieIngredients = t.queryAllTablesForAllIngredients(winterCookieIngredients);
            var myChocolateChipCookieIngredients = t.queryAllTablesForAllIngredients(chocolateChipCookieIngredients);
            var myRecipes = dbR.queryRecipes();
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
        [Test]
        public void Test2Recipse() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            dbR.GetFullRecipePrice(brownies);
            dbR.GetFullRecipePrice(cake);
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(1.74m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.74m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngredients[1].priceOfMeasuredConsumption);
        }
        [Test]
        public void Test3Recipes() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            var myRecipeBox = dbR.MyRecipeBox();
            var myIngredients = dbI.queryIngredients();
            Assert.AreEqual(3, myRecipeBox.Count());
            Assert.AreEqual(14, myIngredients.Count());
            Assert.AreEqual(1.70m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.13m, myIngredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.52m, myIngredients[2].priceOfMeasuredConsumption);
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
        public void TestQueryListOfFullRecipes() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            dbR.GetFullRecipePrice(brownies);
            dbR.GetFullRecipePrice(cake);
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(1.74m, myRecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(1.74m, myIngredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myRecipeBox[1].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.63m, myIngredients[1].priceOfMeasuredConsumption);
        }
        [Test]
        public void TestGetRecipeIngredients() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
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
            var myBrownieIngredients = dbR.ReturnRecipeIngredients(brownies);
            var myCakeIngredients = dbR.ReturnRecipeIngredients(cake);
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(2, myRecipeBox.Count());
            Assert.AreEqual(cocoa.name, myRecipeBox[0].ingredients[0].name);
            Assert.AreEqual(breadFlour.name, myRecipeBox[1].ingredients[0].name);
        }
        [Test]
        public void TestAggregatedPriceDifferenceInYield() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
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
            var myRecipeBox = dbR.MyRecipeBox();
            winterCranberryCookies.yield = 36;
            t.updateAllTablesForAllIngredients(winterCookiesIngredients, winterCranberryCookies);
            var myUpdatedIngredients = t.queryAllTablesForAllIngredients(winterCookiesIngredients);
            var myUpdatedRecipeBox = dbR.MyRecipeBox();
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
        public void TestChangeInPriceFromChangeInYield() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("bread") { id = 1, yield = 24 };
            var i = new Ingredient("All Purpose Flour") { recipeId = 1, ingredientId = 1, measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            r.yield = 48;
            t.updateAllTables(i, r);
            var myNewIngredients = dbI.queryIngredients();
            var myNewRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(24, myRecipeBox[0].yield);
            Assert.AreEqual(48, myNewRecipeBox[0].yield);
            Assert.AreEqual("6 cups", myIngredients[0].measurement);
            Assert.AreEqual("12 cups", myNewIngredients[0].measurement);
        }
        [Test]
        public void TestUpdateRecipeYield() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var r = new Recipe("bread") { id = 1, yield = 18 };
            var i = new Ingredient("bread flour") { recipeId = 1, ingredientId = 1, measurement = "6 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            r.yield = 36;
            dbR.UpdateRecipeYield(r);
            var myUpdatedIngredients = dbI.queryIngredients();
            var myUpdatedRecipeBox = dbR.MyRecipeBox();
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
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var honeyButtermilkBread = new Recipe("Honey Buttermilk Bread") { id = 1, yield = 24 };
            var honey = new Ingredient("Honey") { recipeId = 1, ingredientId = 1, measurement = "1/3 cup", sellingWeight = "32 oz" };
            var salt = new Ingredient("Salt") { recipeId = 1, ingredientId = 2, measurement = "1 teaspoon", sellingWeight = "48 oz" };
            var granulatedSugar = new Ingredient("Granulated Sugar") { recipeId = 1, ingredientId = 3, measurement = "1 teaspoon", sellingWeight = "4 lb" };
            var activeDryYeast = new Ingredient("Active Dry Yeast") { recipeId = 1, ingredientId = 4, measurement = "2 1/4 teaspoons", sellingWeight = "4 oz" };
            var honeyButtermilkBreadIngredients = new List<Ingredient> { honey, salt, granulatedSugar, activeDryYeast };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(honeyButtermilkBreadIngredients, honeyButtermilkBread);
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            honeyButtermilkBread.yield = 120;
            dbR.UpdateRecipeYield(honeyButtermilkBread);
            var myUpdatedIngredients = dbI.queryIngredients();
            var myUpdatedRecipeBox = dbR.MyRecipeBox();
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
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            honeyButtermilkBread.yield = 48; //*2
            cinnamonSwirlBread.yield = 54; //*3
            dbR.UpdateRecipeYield(honeyButtermilkBread);
            dbR.UpdateRecipeYield(cinnamonSwirlBread);
            var myUpdatedIngredients = dbI.queryIngredients();
            var myUpdatedRecipBox = dbR.MyRecipeBox();
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
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            var myIngredients = dbI.queryIngredients();
            var myRecipeBox = dbR.MyRecipeBox();
            honeyButtermilkBread.yield = 48; //*2
            cinnamonSwirlBread.yield = 54; //*3
            dbR.UpdateListOfRecipeYields(myRecipes);
            var myUpdatedIngredients = dbI.queryIngredients();
            var myUpdatedRecipBox = dbR.MyRecipeBox();
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
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
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
            var myIngredients = dbI.queryIngredients();
            var myCakeRecipeBox = dbR.MyRecipeBox();
            yellowCake.yield = 150; //  12.5
            marbleCake.yield = 128; //  8
            chocolateCake.yield = 36; //  1.5
            dbR.UpdateListOfRecipeYields(myCakeRecipes);
            var myUpdatedIngredientBox = t.queryAllTablesForAllIngredients(myCakeIngredients);
            var mySoftasilkFlour = t.queryAllRelevantTablesSQL(softasilkFlour2);
            var myUpdatedCakeRecipeBox = dbR.MyRecipeBox();
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
        public void TestDeleteRecipeAndRecipeIngredients() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, measurement = "2/3 cup", sellingWeight = "32 oz", typeOfIngredient = "honey" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = dbR.MyRecipeBox();
            var myIngredient = t.queryAllRelevantTablesSQL(honey);
            dbR.DeleteRecipeAndRecipeIngredients(bread);
            var myUpdatedIngredients = dbI.queryIngredients();
            var myUpdatedRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myUpdatedRecipeBox.Count());
            Assert.AreEqual(0, myUpdatedIngredients.Count());
        }
        [Test]
        public void TestPricePerServing() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield = 16 };
            var cakeFlour = new Ingredient("Softasilk") { ingredientId = 1, recipeId = 1, measurement = "2 cups 2 tablespoons", typeOfIngredient = "cake flour", sellingWeight = "32 oz" };
            var vanilla = new Ingredient("Vanilla Extract") { ingredientId = 2, recipeId = 1, measurement = "1 1/2 teaspoons", typeOfIngredient = "vanilla extract", sellingWeight = "16 oz" }; //27.59
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 3, recipeId = 1, measurement = "2 teaspoons", typeOfIngredient = "baking powder", sellingWeight = "10 oz" }; //2.90
            var fluffyWhiteCakeIngredients = new List<Ingredient> { cakeFlour, vanilla, bakingPowder };
            t.initializeDatabase();
            t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeIngredients, fluffyWhiteCake);
            var myIngredients = t.queryAllTablesForAllIngredients(fluffyWhiteCakeIngredients);
            var myrecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(.89m, myrecipeBox[0].ingredients[0].priceOfMeasuredConsumption);
            Assert.AreEqual(.36m, myrecipeBox[0].ingredients[1].priceOfMeasuredConsumption);
            Assert.AreEqual(.10m, myrecipeBox[0].ingredients[2].priceOfMeasuredConsumption);
            Assert.AreEqual(1.35m, myrecipeBox[0].aggregatedPrice);
            Assert.AreEqual(16, myrecipeBox[0].yield);
            Assert.AreEqual(.08m, myrecipeBox[0].pricePerServing);
        }
        [Test]
        public void GetRecipeIngredientsWithJoin() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient(); 
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield= 16};
            var eggs = new Ingredient("Eggs, Meringued") { ingredientId = 1, recipeId = 1, measurement = "4 eggs", typeOfIngredient = "egg whtie", sellingWeight = "1 dozen", sellingPrice = 1.99m, classification = "eggs" };
            var softasilk = new Ingredient("Softasilk Cake Flour") { ingredientId = 2, recipeId = 1, measurement = "2 cups 2 tablespoons", typeOfIngredient = "cake flour", sellingWeight = "32 oz", classification = "flour" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 3, recipeId = 1, measurement = "2 teaspoons", typeOfIngredient = "baking powder", sellingWeight = "10 oz", classification = "baking powder" };
            t.initializeDatabase();
            dbR.InsertRecipe(fluffyWhiteCake);
            dbI.insertIngredient(eggs, fluffyWhiteCake);
            dbI.insertIngredient(softasilk, fluffyWhiteCake);
            dbI.insertIngredient(bakingPowder, fluffyWhiteCake); 
            var myRecipeIngredients = dbR.GetRecipeIngredients(fluffyWhiteCake);
            Assert.AreEqual(3, myRecipeIngredients.Count());
            Assert.AreEqual("Eggs, Meringued", myRecipeIngredients[0].name);
            Assert.AreEqual("4 eggs", myRecipeIngredients[0].measurement); 
            Assert.AreEqual("Softasilk Cake Flour", myRecipeIngredients[1].name);
            Assert.AreEqual("2 cups 2 tablespoons", myRecipeIngredients[1].measurement); 
            Assert.AreEqual("Baking Powder", myRecipeIngredients[2].name);
            Assert.AreEqual("2 teaspoons", myRecipeIngredients[2].measurement); 
        }
        [Test]
        public void GetRecipeIngredientsWithJoin2() {
            var t = new DatabaseAccess();
            var dbR = new DatabaseAccessRecipe();
            var dbI = new DatabaseAccessIngredient();
            var fluffyWhiteCake = new Recipe("Fluffy White Cake") { id = 1, yield = 16 }; 
            var eggs = new Ingredient("Eggs, Meringued") { ingredientId = 1, recipeId = 1, measurement = "4 eggs", typeOfIngredient = "egg white", sellingWeight = "1 dozen", sellingPrice = 1.99m, classification = "eggs", expirationDate = new DateTime(2017, 4, 4)};
            var softasilk = new Ingredient("Softasilk Cake Flour") { ingredientId = 2, recipeId = 1, measurement = "2 cups 2 tablespoons", typeOfIngredient = "cake flour", sellingWeight = "32 oz", classification = "flour" };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 3, recipeId = 1, measurement = "2 teaspoons", typeOfIngredient = "baking powder", sellingWeight = "10 oz", classification = "baking powder" };
            t.initializeDatabase();
            //var fluffyWhiteCakeIngredients = new List<Ingredient> { eggs, softasilk, bakingPowder };
            //t.insertListOfIngredientsIntoAllTables(fluffyWhiteCakeIngredients, fluffyWhiteCake);
            t.insertIngredientIntoAllTables(eggs, fluffyWhiteCake);
            t.insertIngredientIntoAllTables(softasilk, fluffyWhiteCake);
            t.insertIngredientIntoAllTables(bakingPowder, fluffyWhiteCake); 
            var myRecipeIngredients = dbR.GetRecipeIngredients(fluffyWhiteCake); 
            Assert.AreEqual(3, myRecipeIngredients.Count());
            Assert.AreEqual("Eggs, Meringued", myRecipeIngredients[0].name);
            Assert.AreEqual("4 eggs", myRecipeIngredients[0].measurement);
            Assert.AreEqual(.66m, myRecipeIngredients[0].priceOfMeasuredConsumption); 
            Assert.AreEqual("Softasilk Cake Flour", myRecipeIngredients[1].name);
            Assert.AreEqual("2 cups 2 tablespoons", myRecipeIngredients[1].measurement);
            Assert.AreEqual(.89m, myRecipeIngredients[1].priceOfMeasuredConsumption); 
            Assert.AreEqual("Baking Powder", myRecipeIngredients[2].name);
            Assert.AreEqual("2 teaspoons", myRecipeIngredients[2].measurement);
            Assert.AreEqual(.06m, myRecipeIngredients[2].priceOfMeasuredConsumption); 
        }
    }
}