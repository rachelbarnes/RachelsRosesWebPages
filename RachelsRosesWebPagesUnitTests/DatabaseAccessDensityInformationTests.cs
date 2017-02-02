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
    class DatabaseAccessDensityInformationTests {
        [Test]
        public void TestInsertFileIntoDensityDatabase() {
            var t = new DatabaseAccess();
            var dbDI = new DatabaseAccessDensityInformation();
            var read = new Reader();
            t.initializeDatabase();
            dbDI.insertDensityTextFileIntoDensityInfoDatabase();
            var myDensityInformationIngredients = dbDI.queryDensityInfoTable();
            Assert.AreEqual(41, myDensityInformationIngredients.Count());
            Assert.AreEqual("all purpose flour", myDensityInformationIngredients[0].name);
            Assert.AreEqual(5m, myDensityInformationIngredients[0].density);
            Assert.AreEqual("bananas, mashed", myDensityInformationIngredients[39].name);
            Assert.AreEqual(12m, myDensityInformationIngredients[39].density);
        }
        [Test]
        public void TestInsertListOfIngredientsIntoDensityInfo() {
            var t = new DatabaseAccess();
            var dbDI = new DatabaseAccessDensityInformation();
            var i = new Ingredient("all purpose flour") { density = 5m };
            var i2 = new Ingredient("pastry flour") { density = 4.25m };
            var i3 = new Ingredient("vanilla extract") { density = 6.86m };
            var myIngredients = new List<Ingredient> { i, i2, i3 };
            t.initializeDatabase();
            dbDI.insertListIntoDensityInfoDatabase(myIngredients);
            var myDensityInfoTable = dbDI.queryDensityInfoTable();
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
            var dbD = new DatabaseAccessDensityInformation();
            var i = new Ingredient("all purpose flour");
            t.initializeDatabase();
            dbD.insertIngredientIntoDensityInfoDatabase(i);
            var BeforeDensityTableInfo = dbD.queryDensityInfoTable();
            i.density = 5m;
            dbD.updateDensityInfoTable(i);
            var AfterDensityTableInfo = dbD.queryDensityInfoTable();
            Assert.AreEqual(1, BeforeDensityTableInfo.Count());
            Assert.AreEqual(1, AfterDensityTableInfo.Count());
            Assert.AreEqual(0, BeforeDensityTableInfo[0].density);
            Assert.AreEqual(5m, AfterDensityTableInfo[0].density);
        }
        [Test]
        public void TestListOfIngredientsUpdateDensityInfoTable() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensityInformation();
            var i = new Ingredient("all purpose flour");
            var i2 = new Ingredient("pastry flour");
            var i3 = new Ingredient("vanilla extract");
            var myIngredients = new List<Ingredient> { i, i2, i3 };
            t.initializeDatabase();
            dbD.insertListIntoDensityInfoDatabase(myIngredients);
            var BeforeMyDensityInfoTable = dbD.queryDensityInfoTable();
            i.density = 5m;
            i2.density = 4.25m;
            i3.density = 6.86m;
            dbD.updateListOfIngredientsInDensityInfoTable(myIngredients);
            var AfterMyDensityInfoTable = dbD.queryDensityInfoTable();
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
        public void TestInsertIngredientIntoDensityInfoDatabase() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensityInformation();
            var r = new Recipe("Sample") { id = 1 };
            var i = new Ingredient("Softasilk Flour") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour" };
            var i2 = new Ingredient("Ground Ginger") { ingredientId = 2, recipeId = 1, measurement = "1 teaspoon", sellingWeight = "8 oz", typeOfIngredient = "ground ginger", density = 2.93m };
            var myIngredients = new List<Ingredient> { i, i2 };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            t.insertIngredientIntoAllTables(i2, r);
            var myIngredientBoxDensities = dbD.queryDensityInfoTable();
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
            var dbD = new DatabaseAccessDensityInformation();
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
            var myIngredientBoxDensities = dbD.queryDensityInfoTable();
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
        public void TestGetListOfIngredientTypes() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensityInformation();
            t.initializeDatabase();
            var firstType = "all purpose flour";
            var secondType = "bread flour";
            var thirdType = "cake flour";
            var lastType = "egg yolk";
            var actual = dbD.getListOfIngredientTypesFromDensityTable();
            Assert.AreEqual(47, actual.Count());
            Assert.AreEqual(firstType, actual[0]);
            Assert.AreEqual(secondType, actual[1]);
            Assert.AreEqual(thirdType, actual[2]);
            Assert.AreEqual(lastType, actual[46]);
        }
        [Test]
        public void TestReturnDensityFromDensityTable3() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensityInformation();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("All Purpose Flour") { recipeId = 1, ingredientId = 1, measurement = "3 cups", sellingWeight = "5 lb" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var expected = 5m;
            var actual = dbD.queryDensityTableRowDensityValueByName(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestReturnDensityFromDensityTable2() {
            var t = new DatabaseAccess();
            var dbD = new DatabaseAccessDensityInformation();
            var r = new Recipe("bread") { id = 1 };
            var i = new Ingredient("Salt") { recipeId = 1, ingredientId = 1, measurement = "1/2 teaspoon", sellingWeight = "48 oz" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(i, r);
            var expected = 10.72m;
            var actual = dbD.queryDensityTableRowDensityValueByName(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestQueryDensityTableDensityByName() {
            var db = new DatabaseAccess();
            var dbDI = new DatabaseAccessDensityInformation();
            var flour = new Ingredient("All Purpose Flour") { density = 5m };
            db.initializeDatabase();
            dbDI.insertDensityTextFileIntoDensityInfoDatabase();
            var myIngredientDensityInfo = dbDI.queryDensityTableRowDensityValueByName(flour);
            Assert.AreEqual(5m, myIngredientDensityInfo); 
        }
        [Test]
        public void TestQueryDensityTableDensityByName2() {
            var db = new DatabaseAccess();
            var dbDI = new DatabaseAccessDensityInformation();
            var softasilkCakeFlour = new Ingredient("Softasilk Cake Flour") { typeOfIngredient = "cake flour"}; 
            db.initializeDatabase();
            dbDI.insertDensityTextFileIntoDensityInfoDatabase();
            var myIngredientDensityInfo = dbDI.queryDensityTableRowDensityValueByName(softasilkCakeFlour);
            Assert.AreEqual(4.5m, myIngredientDensityInfo); 
        }
    }
}