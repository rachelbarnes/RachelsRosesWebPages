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
            t.DeleteRecipe(r.name);
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
            Recipe returnedRecipe = t.GetFullRecipe(r.name);
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
            var myRecipe = t.GetFullRecipe(r.name);
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
            var myRecipe = t.GetFullRecipe(r.name);
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
            var myIngredientInformation = t.queryDensityTable();
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
            var myIngredientInformation = t.queryDensityTable();
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
            var myIngredientInformation = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
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
            var myIngInfo = t.queryDensityTable();
            Assert.AreEqual(.0525m, myIngInfo[0].pricePerOunce);
        }
        [Test]
        public void TestPricePerOunce2() {
            var t = new DatabaseAccess();
            var rest = new MakeRESTCalls();
            var i = new Ingredient("King Arthur Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb",
                sellingWeightInOunces = 80m
            };
            var i2 = new Ingredient("Softasilk Cake Flour") {
                ingredientId = 2,
                sellingWeight = "32 oz",
                sellingWeightInOunces = 32m
            };
            var i3 = new Ingredient("Rumford Baking Powder") {
                ingredientId = 3,
                sellingWeight = "10 oz",
                sellingWeightInOunces = 10m
            };
            var i4 = new Ingredient("Vanilla Extract") {
                ingredientId = 4,
                sellingWeight = "8 fl oz",
                sellingWeightInOunces = 8m
            };
            var response = new ItemResponse() {
                name = "King Arthur Flour Unbleached Bread Flour, 5.0 LB"
            };
            var response2 = new ItemResponse() {
                name = "Pillsbury Softasilk: Enriched Bleached Cake Flour, 32 Oz"
            };
            var response3 = new ItemResponse() {
                name = "Rumford Premium Aluminum-Free Baking Powder, 10 oz"
            };
            var response4 = new ItemResponse() {
                name = "McCormick Pure Vanilla Extract, 8.0 FL OZ"
            };
            t.initializeDatabase();
            t.insertIngredientDensityData(i);
            t.insertIngredientDensityData(i2);
            t.insertIngredientDensityData(i3);
            t.insertIngredientDensityData(i4);
            var myIngInfo = t.queryDensityTable();
            var iPPO = Math.Round((rest.GetItemResponsePrice(i) / i.sellingWeightInOunces), 4);
            var i2PPO = Math.Round((rest.GetItemResponsePrice(i2) / i2.sellingWeightInOunces), 4);
            var i3PPO = Math.Round((rest.GetItemResponsePrice(i3) / i3.sellingWeightInOunces), 4);
            var i4PPO = Math.Round((rest.GetItemResponsePrice(i4) / i4.sellingWeightInOunces), 4);
            Assert.AreEqual(4, myIngInfo.Count());
            Assert.AreEqual(iPPO, myIngInfo[0].pricePerOunce);
            Assert.AreEqual(i2PPO, myIngInfo[1].pricePerOunce);
            Assert.AreEqual(i3PPO, myIngInfo[2].pricePerOunce);
            Assert.AreEqual(i4PPO, myIngInfo[3].pricePerOunce);
            Assert.AreEqual(1.3138, myIngInfo[3].pricePerOunce);
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
            var myIngInfo = t.queryDensityTable();
            Assert.AreEqual(4, myIngInfo.Count());
            Assert.AreEqual(rest.GetItemResponsePrice(i), myIngInfo[0].sellingPrice);
            Assert.AreEqual(rest.GetItemResponsePrice(i2), myIngInfo[1].sellingPrice);
            Assert.AreEqual(rest.GetItemResponsePrice(i3), myIngInfo[2].sellingPrice);
            Assert.AreEqual(rest.GetItemResponsePrice(i4), myIngInfo[3].sellingPrice);
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
            Assert.AreEqual(i.ingredientId, myIngInfo[0].ingredientId);
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
                ouncesRemaining = (6m - 8m)
            };
            var i2 = new Ingredient("Whole Wheat Flour") {
                recipeId = 1,
                ingredientId = 2,
                density = 4.5m,
                ouncesConsumed = 13.5m,
                ouncesRemaining = (13.5m - 80m)
            };
            t.initializeDatabase();
            t.insertIngredient(i, r);
            t.insertIngredient(i2, r);
            t.insertIngredientConsumtionData(i);
            t.insertIngredientConsumtionData(i2);
            var myIngInfo = t.queryConsumptionTable();
            Assert.AreEqual(2, myIngInfo.Count());
            Assert.AreEqual(i.ingredientId, myIngInfo[0].ingredientId);
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
                ouncesRemaining = 80m
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
            var myIngDens = t.queryDensityTable();
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
            t.insertAllIngredientsIntoAllTables(ListOfIngredients, r);
            var myIngredients = t.queryAllTablesForAllIngredients(ListOfIngredients, r);
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
        public void TestAggregatedChocolateChipRecipeConsumedPrice() {
            var t = new DatabaseAccess();
            var r = new Recipe("Chocolate Chip Cookies") {
                id = 1
            };
            var chococateChips = new Ingredient("Nestle Tollhouse Semi Sweet Chocolate Chips") {
                recipeId = 1,
                ingredientId = 1,
                sellingWeight = "12 ounces",
                measurement = "1 3/4 cups",
                density = 5.35m //3.26
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
            t.insertAllIngredientsIntoAllTables(ListOfIngredients, r);
            t.GetFullRecipePrice(r);
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1.29m, myRecipes[0].aggregatedPrice); 
        }
        [Test]
        public void TestWinterCookiesInAJarPriceCostEvaluation() {
            var t = new DatabaseAccess();
            var winterCookies = new Recipe("Winter Cookies") {
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
            t.insertAllIngredientsIntoAllTables(winterCookieIngredients, winterCookies);
            t.GetFullRecipePrice(winterCookies); 
            //this getFullRecipePrice is what is allowing me to get the price... why is it updating my ingredients? 
            var myIngredients = t.queryIngredients();
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(9, myIngredients.Count());
            Assert.AreEqual(.26m, myIngredients[0].priceOfMeasuredConsumption); 
            Assert.AreEqual(.00m, myIngredients[1].priceOfMeasuredConsumption); 
            Assert.AreEqual(.01m, myIngredients[2].priceOfMeasuredConsumption); 
            Assert.AreEqual(.15m, myIngredients[3].priceOfMeasuredConsumption); 
            Assert.AreEqual(.17m, myIngredients[4].priceOfMeasuredConsumption); 
            Assert.AreEqual(.09m, myIngredients[5].priceOfMeasuredConsumption); 
            Assert.AreEqual(.43m, myIngredients[6].priceOfMeasuredConsumption); 
            Assert.AreEqual(.76m, myIngredients[7].priceOfMeasuredConsumption); 
            Assert.AreEqual(1.56m, myIngredients[8].priceOfMeasuredConsumption);
            Assert.AreEqual(3.43m, myRecipes[0].aggregatedPrice); 
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
            var winterCookieIngredients = new List<Ingredient> { flour, bakingSoda, salt, rolledOats, brownSugar, granulatedSugar, driedCranberries, whiteChocolate, choppedPecans, chocolateChips };
            var chocolateChipCookieIngredients = new List<Ingredient> { chocolateChips }; 
            t.insertAllIngredientsIntoAllTables(winterCookieIngredients, winterCookies);
            t.insertAllIngredientsIntoAllTables(chocolateChipCookieIngredients, chocolateChipCookies);
            t.GetFullRecipePrice(winterCookies); 
            //this getFullRecipePrice is what is allowing me to get the price... why is it updating my ingredients? 
            var myIngredients = t.queryIngredients();
            var myRecipes = t.queryRecipes();
            Assert.AreEqual(2, myRecipes.Count());
            Assert.AreEqual(10, myIngredients.Count());
            Assert.AreEqual(.26m, myIngredients[0].priceOfMeasuredConsumption); 
            Assert.AreEqual(.00m, myIngredients[1].priceOfMeasuredConsumption); 
            Assert.AreEqual(.01m, myIngredients[2].priceOfMeasuredConsumption); 
            Assert.AreEqual(.15m, myIngredients[3].priceOfMeasuredConsumption); 
            Assert.AreEqual(.17m, myIngredients[4].priceOfMeasuredConsumption); 
            Assert.AreEqual(.09m, myIngredients[5].priceOfMeasuredConsumption); 
            Assert.AreEqual(.43m, myIngredients[6].priceOfMeasuredConsumption); 
            Assert.AreEqual(.76m, myIngredients[7].priceOfMeasuredConsumption); 
            Assert.AreEqual(1.56m, myIngredients[8].priceOfMeasuredConsumption);
            Assert.AreEqual(.89m, myIngredients[9].priceOfMeasuredConsumption); 
            //the purpose of this test is to make sure my chocolateChips get ignored because they're a part of another recipe
            //Assert.AreEqual(3.43m, myRecipes[0].aggregatedPrice);
            //Assert.AreEqual(.89m, myRecipes[1].aggregatedPrice); 
            }
        //read the density database, have to put that in a database, have to read the database and put it in for all these...
        //i also have to return a list of the items received from walmart's database so you know what you're getting and you can choose which one you would rather have, and be able to select it
        //i would also like to get other stores's databases too, like costco, target maybe, etc. 
        //in the list of itemresponses, look at which one is the best deal for it's price, and bold that one in the view
        //still have to refactor and do tech debt on QueryAllTablesFroAllIngredients
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
    }
}
