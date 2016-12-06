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
            }; //this is done because the Recipe constructor only takes the name as a parameter, but you can identify the yield and the list of ingredients still in a constructor form
            t.InsertRecipe(r);
            r = new Recipe("other") {
                yield = 1
            };
            t.InsertRecipe(r);
            var returns = t.queryRecipes();
            Assert.AreEqual(2, returns.Count());
            Assert.AreEqual(1, returns[0].id);
        }
        //[Test]
        //public void TestInsertRecipe2() {
        //    var t = new DatabaseAccess();
        //    var newR = new Recipe("Pecan Pie") {
        //        yield = 16
        //    };
        //    t.initializeDatabase();
        //    t.InsertRecipe(newR);
        //    var returns = t.queryRecipes();
        //    Assert.AreEqual(2, returns.Count());
        //    Assert.AreEqual(2, returns[1].id);
        //}
        //[Test]
        //public void TestInsertRecipe3() {
        //    var t = new DatabaseAccess();
        //    var newRecipe = new Recipe("White Cake") {
        //        yield = 18
        //    };
        //    t.initializeDatabase();
        //    t.InsertRecipe(newRecipe);
        //    var myRecipes = t.queryRecipes();
        //    Assert.AreEqual(1, myRecipes.Count());
        //    Assert.AreEqual(1, myRecipes[1].id);
        //    Assert.AreEqual(18, myRecipes[1].yield);
        //}
        //[Test]
        //public void TestIngredientTable() {
        //    var t = new DatabaseAccess();
        //    var i = new Ingredient("All-Purpose Flour", "2 1/2 cups");
        //    var r = new Recipe();
        //    t.initializeDatabase();
        //    var recipes = t.queryRecipes();
        //    foreach (var recipe in recipes) {
        //        if (recipe.name == "White Cake")
        //            r = recipe;
        //    }
        //    t.InsertIngredient(i, r);
        //    Assert.AreEqual(3, r.id);
        //}
        //these are off because of the InistializeDatabase(), i have to make sure the tests are now accurate
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
                //i'm hardcoding this recipe id here, but in the controller, when an ingredient is entered for a recipe in the browser,
                //    i need to assign the recipe id by the recipe name, (ingredient.recipeid = recipe.id)
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

    }
}
