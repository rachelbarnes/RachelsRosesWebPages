using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RachelsRosesWebPages;
using RachelsRosesWebPages.Models;
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    class DatabaseAccessCostsTests {
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
            var myIngredient = t.queryAllRelevantTablesSQL(i);
            Assert.AreEqual(2.98m, myIngredient.sellingPrice);
            Assert.AreEqual(.0931m, myIngredient.pricePerOunce);
        }
        [Test]
        public void TestOverwritingTheCostTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessCosts();
            var dbR = new DatabaseAccessRecipe();
            var yellowCake = new Recipe("Yellow Cake") { id = 1, yield = 12 };
            var milk = new Ingredient("Whole Milk") { ingredientId = 1, recipeId = 1, measurement = "1 cup", sellingWeight = "1 gallon", typeOfIngredient = "milk", classification = "dairy" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(milk, yellowCake);
            var myIngredient = t.queryAllRelevantTablesSQL(milk);
            milk.sellingPrice = 2.98m;
            dbC.updateCostDataTable(milk);
            var myCostIngredient = dbC.queryCostTable();
            t.updateAllTables(milk, yellowCake);
            var myUpdatedIngredient = t.queryAllRelevantTablesSQL(milk);
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(1, myRecipeBox[0].ingredients.Count());
            Assert.AreEqual(2.98m, myUpdatedIngredient.sellingPrice);
            Assert.AreEqual(8.2m, myIngredient.density);
            Assert.AreEqual(.19m, myUpdatedIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(.0233m, myUpdatedIngredient.pricePerOunce);
        }
        [Test]
        public void TestOverwritingTheCostTable2() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessCosts();
            var dbR = new DatabaseAccessRecipe();
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
            var costTable = dbC.queryCostTable();
            t.updateAllTables(sourCream, chocolateCake);
            t.updateAllTables(milk, yellowCake);
            t.updateAllTablesForAllIngredients(honeyButtermilkBreadIngredients, honeyButtermilkBread);
            var myUpdatedIngredients = t.queryAllTablesForAllIngredients(allIngredients);
            var myRecipeBox = dbR.MyRecipeBox();
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
            var dbC = new DatabaseAccessCosts();
            var dbR = new DatabaseAccessRecipe();
            var chocolateCake = new Recipe("Chocolate Cake") { id = 1, yield = 16 };
            var softasilk = new Ingredient("Softasilk Flour") { recipeId = 1, ingredientId = 1, sellingWeight = "32 oz", measurement = "3 cups", typeOfIngredient = "cake flour", classification = "flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(softasilk, chocolateCake);
            var myIngredient = t.queryAllRelevantTablesSQL(softasilk);
            softasilk.sellingPrice = 5m;
            var costTable = dbC.queryCostTable();
            t.updateAllTables(softasilk, chocolateCake);
            var myUpdatedIngredient = t.queryAllRelevantTablesSQL(softasilk);
            var myRecipeBox = dbR.MyRecipeBox();
            Assert.AreEqual(1.26m, myIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(2.11m, myUpdatedIngredient.priceOfMeasuredConsumption);
            Assert.AreEqual(2.11m, myRecipeBox[0].aggregatedPrice);
        }
        [Test]
        public void TestDeleteIngredientFromCostTable() {
            var t = new DatabaseAccess();
            var dbC = new DatabaseAccessCosts();
            var dbR = new DatabaseAccessRecipe();
            var bread = new Recipe("Bread") { id = 1 };
            var honey = new Ingredient("Honey") { ingredientId = 1, recipeId = 1, typeOfIngredient = "honey", measurement = "1/3 cup", sellingWeight = "32 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(honey, bread);
            var myRecipes = dbR.MyRecipeBox();
            var myIngredients = t.queryAllRelevantTablesSQL(honey);
            dbC.DeleteIngredientFromCostTable(honey);
            var myCostIngredients = dbC.queryCostTable();
            Assert.AreEqual(1, myRecipes.Count());
            Assert.AreEqual(1, myRecipes[0].ingredients.Count());
            Assert.AreEqual(0, myCostIngredients.Count());
        }
    }
}
