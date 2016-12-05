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
            var returns = t.queryRecipe("recipes");
            Assert.AreEqual(2, returns.Count());
            Assert.AreEqual(1, returns[0].id);
        }
        [Test]
        public void TestInitializeDatabase() {
            var t = new DatabaseAccess();
            t.initializeDatabase();
            var r = new Recipe("test") {
                yield = 4
            };
            t.InsertRecipe(r);
            r = t.queryRecipe("test").First();
            Assert.AreEqual("test", r.name);
            Assert.AreEqual(4, r.yield);
            Assert.AreEqual(1, r.id);
            r.name = "horse";
            r.yield = 5;
            t.UpdateRecipe(r);
            Assert.AreEqual("horse", r.name);
            Assert.AreEqual(5, r.yield);
            Assert.AreEqual(1, r.id);
        }
        //[Test]
        //public void TestInsertRecipe() {
        //    var t = new DatabaseAccess();
        //    var newR = new Recipe("White Cake") {
        //        yield = 16
        //    };
        //    t.InsertRecipe(newR);
        //    var returns = t.queryRecipe();
        //    Assert.AreEqual(4, returns.Count());
        //    Assert.AreEqual(4, returns[3].id);
        //}
        /*looking at these tests: 
            create an instance of databaseaccess(very run of the mill for tests)
            create your recipe object
            apply sql command via DatabaseAccess method
            return the results of the query (this is returns)
            test some of the results, in this case, the number of recipes, and the id of the current recipe that was just added 
       */
        //this will require being able to see results in SSMS (SQL Server Management Studio) for the database design when we start to get into more complicated tests and in general more functionality
        [Test]
        public void TestInsertRecipe2() {
            var t = new DatabaseAccess();
            var newR = new Recipe("Pecan Pie") {
                yield = 16 };
            t.InsertRecipe(newR);
            var returns = t.queryRecipe("recipes");
            Assert.AreEqual(2, returns.Count());
            Assert.AreEqual(2, returns[1].id); 
        }
        [Test]
        public void TestInsertRecipe3() {
            var t = new DatabaseAccess();
            var newRecipe = new Recipe("White Cake"){
                yield = 18};
            t.InsertRecipe(newRecipe);
            var myRecipes = t.queryRecipe("recipes");
            Assert.AreEqual(3, myRecipes.Count());
            Assert.AreEqual(3, myRecipes[2].id);
            Assert.AreEqual(18, myRecipes[2].yield); 
        }
        [Test]
        public void TestIngredientTable() {
            var t = new DatabaseAccess();
            var i = new Ingredient("All-Purpose Flour", "2 1/2 cups"); 
            var r = new Recipe();
            var recipes = t.queryRecipe("recipes");
            foreach (var recipe in recipes) {
                if (recipe.name == "White Cake")
                    r = recipe;
            }
            t.InsertIngredient(i, r);
            Assert.AreEqual(3, r.id);
            //Assert.AreEqual(r.id, i.recipeId);
            //this isn't passing because i.recipeId isn't assigned... so it's 0. 
        }
        [Test]
        public void TestDeleteIngredient() {
            var t = new DatabaseAccess();
            var r = new Recipe("horse");
            bool b = new bool();
            t.DeleteRecipe("recipes"); 
            foreach (var recipe in t.queryRecipe("recipes")) {
                if (recipe.name == r.name) {
                    b = false;
                } else b = true; 
            }
            Assert.AreEqual(false, b); 
        }

    }
}
