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
            t.InsertIngredient(i, r);
            t.InsertIngredient(i2, r);
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
            t.InsertIngredient(i, r);
            t.InsertIngredient(i2, r2);
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
            t.InsertIngredient(i, r);
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
            t.InsertIngredient(i, r);
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
            t.InsertIngredient(i, r);
            t.InsertIngredient(i2, r);
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
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                density = 4.5m,
                pricePerOunce = .03m,
                sellingWeight = "5 lbs",
                sellingPrice = 4.99m
            };
            t.initializeDatabase();
            t.InsertIngredientDensityAndSellingInformation(i);
            var myIngredientInformation = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            var myIngredientInformation = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            i.density = 12m;
            t.updateDensitiesAndPrice(i);
            var myIngredientInformation = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            var myIngInfo = t.queryDensitiesAndPrices();
            Assert.AreEqual(1, myIngInfo.Count());
            Assert.AreEqual(1, myIngInfo[0].ingredientId);
            Assert.AreEqual(i.density, myIngInfo[0].density);
            Assert.AreEqual(i.sellingWeight, myIngInfo[0].sellingWeight);
            Assert.AreEqual(i.sellingPrice, myIngInfo[0].sellingPrice);
        }
        [Test]
        public void TestCalculatingPricePerOunce() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Butter") {
                ingredientId = 1,
                sellingPrice = 3.99m,
                sellingWeightInOunces = 16m
            };
            var getPricePerOunce = t.CalculatePricePerOunce(i);
            Assert.AreEqual(.25, getPricePerOunce);
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
            t.InsertIngredientDensityAndSellingInformation(i);
            var myIngInfo = t.queryDensitiesAndPrices();
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
            var i2 = new Ingredient("Whole Milk") {
                ingredientId = 2,
                sellingWeight = "1 gallon"
            };
            var i2SWO = 128;
            t.initializeDatabase();
            t.InsertIngredientDensityAndSellingInformation(i);
            t.InsertIngredientDensityAndSellingInformation(i2);
            i.sellingWeightInOunces = iSWO;
            i2.sellingWeightInOunces = i2SWO;
            t.updateDensitiesAndPrice(i);
            t.updateDensitiesAndPrice(i2);
            var myIngInfo = t.queryDensitiesAndPrices();
            Assert.AreEqual(2, myIngInfo.Count());
            Assert.AreEqual(iSWO, myIngInfo[0].sellingWeightInOunces);
            Assert.AreEqual(i2SWO, myIngInfo[1].sellingWeightInOunces);
        }
        [Test]
        public void TestUpdatingSellingWeightInOunces() {
            var t = new DatabaseAccess();
            var i = new Ingredient("Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lbs"
            };
            t.initializeDatabase();
            t.InsertIngredientDensityAndSellingInformation(i);
            t.UpdateDensityTable(i);
            var myIngInfo = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            t.InsertIngredientDensityAndSellingInformation(i2);
            t.InsertIngredientDensityAndSellingInformation(i3);
            t.UpdateDensityTable(i);
            t.UpdateDensityTable(i2);
            t.UpdateDensityTable(i3);
            var myIngInfo = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            t.UpdateDensityTable(i);
            var myIngInfo = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            t.UpdateDensityTable(i);
            var myIngInfo = t.queryDensitiesAndPrices();
            Assert.AreEqual(.0542m, myIngInfo[0].pricePerOunce);
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
                sellingWeight = "8 fl oz", //i should still be ok... it contains oz, which should be fine if the rest call has fluid ounces and i list it as fl oz
                sellingWeightInOunces =  8m
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
            t.InsertIngredientDensityAndSellingInformation(i);
            t.InsertIngredientDensityAndSellingInformation(i2);
            t.InsertIngredientDensityAndSellingInformation(i3);
            t.InsertIngredientDensityAndSellingInformation(i4);
            t.UpdateDensityTable(i);
            t.UpdateDensityTable(i2);
            t.UpdateDensityTable(i3);
            t.UpdateDensityTable(i4);
            var myIngInfo = t.queryDensitiesAndPrices();
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
            t.InsertIngredientDensityAndSellingInformation(i);
            t.InsertIngredientDensityAndSellingInformation(i2);
            t.InsertIngredientDensityAndSellingInformation(i3);
            t.InsertIngredientDensityAndSellingInformation(i4);
            t.UpdateDensityTable(i);
            t.UpdateDensityTable(i2);
            t.UpdateDensityTable(i3);
            t.UpdateDensityTable(i4);
            var myIngInfo = t.queryDensitiesAndPrices();
            Assert.AreEqual(4, myIngInfo.Count());
            Assert.AreEqual(rest.GetItemResponsePrice(i), myIngInfo[0].sellingPrice);
            Assert.AreEqual(rest.GetItemResponsePrice(i2), myIngInfo[1].sellingPrice);
            Assert.AreEqual(rest.GetItemResponsePrice(i3), myIngInfo[2].sellingPrice);
            Assert.AreEqual(rest.GetItemResponsePrice(i4), myIngInfo[3].sellingPrice);
        }
    }
}
