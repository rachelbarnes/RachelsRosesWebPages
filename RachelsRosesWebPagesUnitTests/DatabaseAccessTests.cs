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
        [Test]
        public void TestIngredientTable() {
            var t = new DatabaseAccess();
            var i = new Ingredient("all-purpose flour", "2 1/2 cups");
            var r = new Recipe("White Cake");
            t.initializeDatabase();
            var recipes = t.queryRecipes();
            foreach (var recipe in recipes) {
                if (recipe.name == "White Cake")
                    r = recipe;
            }
            t.InsertIngredient(i, r);
            Assert.AreEqual(1, r.id);
            Assert.AreEqual("all-purpose flour", r.ingredients[0].name);
            Assert.AreEqual("2 1/2 cups", r.ingredients[0].measurement);
            Assert.AreEqual("all-purpose flour", recipes[0].ingredients[0].name);
            Assert.AreEqual("2 1/2 cups", recipes[0].ingredients[0].measurement);
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
        [Test]
        public void TestEditRecipeName() {
            var t = new DatabaseAccess();
            var r = new Recipe("White Cake");
            var newRecipeName = "Fluffy White Cake";
            t.initializeDatabase();
            t.InsertRecipe(r);
            r.name = newRecipeName;
            t.UpdateRecipe(r);
            var myRecipeBox = t.queryRecipes();
            Assert.AreEqual(newRecipeName, myRecipeBox[0].name);
            //i'm not sure why this isn't passing... need to figure this out later or ask Steve
        }
        [Test]
        public void TestInsertIngredientToIngredientDatabase() {
            var t = new DatabaseAccess();
            var r = new Recipe("Cranberry Swirl Bread") {
                id = 1 //this is set to match the recipe in the recipe box, as the query recipes
            };
            var i = new Ingredient("Cranberries", "2 cups") {
                recipeId = r.id
            };
            t.initializeDatabase();
            t.InsertRecipe(r);
            t.InsertIngredient(i, r);
            var myRecipeBox = t.queryRecipes();
            var myIngredientBox = t.queryIngredients();
            Assert.AreEqual(r.name, myRecipeBox[0].name);
            Assert.AreEqual(i.name, myIngredientBox[0].name);
            Assert.AreEqual(i.measurement, myIngredientBox[0].measurement);
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
    }
}
