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
    class DatabaseAccessConsumptionOuncesConsumedTests {
        [Test]
        public void TestInsertIngredientIntoConsumptionOuncesConsumedTable() {
            var db = new DatabaseAccess();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed();
            var cake = new Recipe("Cake") { id = 1, yield = 24 };
            var bakingPowder = new Ingredient("Baking Powder") { ingredientId = 2, recipeId = 1, measurement = "1 tablespoon", sellingWeight = "10 oz", typeOfIngredient = "baking powder", classification = "rising agent" };
            var cakeFlour = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour", classification = "flour" };
            db.initializeDatabase();
            db.insertIngredientIntoAllTables(cakeFlour, cake);
            db.insertIngredientIntoAllTables(bakingPowder, cake);
            var myConsumptionOuncesConsumedTable = dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual("Softasilk Cake Flour", myConsumptionOuncesConsumedTable[0].name);
            Assert.AreEqual("2 cups", myConsumptionOuncesConsumedTable[0].measurement);
            Assert.AreEqual(9m, myConsumptionOuncesConsumedTable[0].ouncesConsumed);
            Assert.AreEqual(23m, myConsumptionOuncesConsumedTable[0].ouncesRemaining);
            Assert.AreEqual("Baking Powder", myConsumptionOuncesConsumedTable[1].name);
            Assert.AreEqual("1 tablespoon", myConsumptionOuncesConsumedTable[1].measurement);
            Assert.AreEqual(.52m, myConsumptionOuncesConsumedTable[1].ouncesConsumed);
            Assert.AreEqual(9.48m, myConsumptionOuncesConsumedTable[1].ouncesRemaining);
        }
        [Test]
        public void TestConsumptionOuncesConsumedTable() {
            var t = new DatabaseAccess();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed();
            var cake = new Recipe("Cake") { id = 1, yield = 24 };
            var cakeFlour = new Ingredient("Cake Flour") { ingredientId = 1, recipeId = 1, measurement = "1 1/2 cups", sellingWeight = "32 oz", typeOfIngredient = "cake flour", classification = "flour" };
            t.initializeDatabase();
            t.insertIngredientIntoAllTables(cakeFlour, cake);
            var myIngredients = dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(6.75m, myIngredients[0].ouncesConsumed);
            Assert.AreEqual(25.25m, myIngredients[0].ouncesRemaining); 
        }
        [Test]
        public void TestMultipleIngredientsWithSameNameOuncesConsumedTable() {
            var t = new DatabaseAccess();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed();
            var cakeFlour = new Ingredient("Cake Flour") { ingredientId = 1, ouncesConsumed = 10m };
            var cakeFlour2 = new Ingredient("Cake Flour") { ingredientId = 2, ouncesConsumed = 20m };
            var cakeFlour3 = new Ingredient("Cake Flour") { ingredientId = 3, ouncesConsumed = 30m };
            var cakeFlour4 = new Ingredient("Cake Flour") { ingredientId = 4, ouncesConsumed = 40m };
            var cakeFlour5 = new Ingredient("Cake Flour") { ingredientId = 5, ouncesConsumed = 50m };
            var myIngredientBox = new List<Ingredient> { cakeFlour, cakeFlour2, cakeFlour3, cakeFlour4, cakeFlour5 };
            t.initializeDatabase();
            dbCOC.insertListOfIngredientsIntoConsumptionOuncesConsumed(myIngredientBox);
            var myIngredients = dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(10m, myIngredientBox[0].ouncesConsumed);
            Assert.AreEqual(20m, myIngredientBox[1].ouncesConsumed);
            Assert.AreEqual(30m, myIngredientBox[2].ouncesConsumed);
            Assert.AreEqual(40m, myIngredientBox[3].ouncesConsumed);
            Assert.AreEqual(50m, myIngredientBox[4].ouncesConsumed);
        }
        [Test]
        public void TestConsumptionOuncesConsumedTableQueryByName() {
            var t = new DatabaseAccess();
            var DOC = new DatabaseAccessConsumptionOuncesConsumed();
            var dbI = new DatabaseAccessIngredient();
            var dbC = new DatabaseAccessConsumption();
            var dbDI = new DatabaseAccessDensityInformation();
            var cake = new Recipe("Cake") { id = 1, yield = 12 };
            var SoftasilkCakeFlour = new Ingredient("Softasilk Cake Flour") { ingredientId = 1, recipeId = 1, sellingWeight = "32 oz", measurement = "1 cup", density = 4.5m, ouncesConsumed = 4.5m, ouncesRemaining = 27.5m, typeOfIngredient = "cake flour", classification = "flour" };
            t.initializeDatabase();
            dbDI.insertDensityTextFileIntoDensityInfoDatabase();
            dbI.insertIngredient(SoftasilkCakeFlour, cake);
            dbC.insertIngredientConsumtionData(SoftasilkCakeFlour);
            DOC.insertIngredientIntoConsumptionOuncesConsumed(SoftasilkCakeFlour);
            var COCTableIngredient = DOC.queryConsumptionOuncesConsumedTableByName(SoftasilkCakeFlour);
            Assert.AreEqual("Softasilk Cake Flour", COCTableIngredient.name);
            Assert.AreEqual("1 cup", COCTableIngredient.measurement);
            Assert.AreEqual(4.5m, COCTableIngredient.ouncesConsumed);
            Assert.AreEqual(27.5m, COCTableIngredient.ouncesRemaining);
        }
    }
}
