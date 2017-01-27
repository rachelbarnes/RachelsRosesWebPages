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
    class DatabaseAccessConsumptionOuncesConsumedTests{
        [Test]
        public void TestConsumptionOuncesConsumedTable() {
            var t = new DatabaseAccess();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed(); 
            var cakeFlour = new Ingredient("Cake Flour") { ingredientId = 1, ouncesConsumed = 20m };
            t.initializeDatabase();
            dbCOC.insertIngredientIntoConsumptionOuncesConsumed(cakeFlour);
            var myIngredients =dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(20m, myIngredients[0].ouncesConsumed);
        }
        [Test]
        public void TestConsumptionOuncesConsumedTable2() {
          
            var t = new DatabaseAccess();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed(); 

            var cakeFlour = new Ingredient("Cake Flour") { ingredientId = 1, ouncesConsumed = 30m };
            var allPurposeFlour = new Ingredient("All Purpose Flour") { ingredientId = 2, ouncesConsumed = .4m };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 3, ouncesConsumed = 35m };
            var myIngredientBox = new List<Ingredient> { cakeFlour, allPurposeFlour, breadFlour };
            t.initializeDatabase();
            dbCOC.insertListOfIngredientsIntoConsumptionOuncesConsumed(myIngredientBox);
            //t.insertIngredientIntoConsumptionOuncesConsumed(cakeFlour);
            var myIngredients =dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(30m, myIngredients[0].ouncesConsumed);
            Assert.AreEqual(.4m, myIngredients[1].ouncesConsumed);
            Assert.AreEqual(35m, myIngredients[2].ouncesConsumed);
        }
        [Test]
        public void TestUpdateOuncesConsumedTable() {
            var t = new DatabaseAccess();
            var dbCOC = new DatabaseAccessConsumptionOuncesConsumed(); 
            var cakeFlour = new Ingredient("Cake Flour") { ingredientId = 1, ouncesConsumed = 5m };
            var breadFlour = new Ingredient("Bread Flour") { ingredientId = 2, ouncesConsumed = 10m };
            var myIngredientBox = new List<Ingredient> { cakeFlour, breadFlour };
            t.initializeDatabase();
            dbCOC.insertListOfIngredientsIntoConsumptionOuncesConsumed(myIngredientBox);
            cakeFlour.ouncesConsumed = 20m;
            breadFlour.ouncesConsumed = 40m;
            dbCOC.updateIngredientInConsumptionouncesConsumed(cakeFlour);
            dbCOC.updateIngredientInConsumptionouncesConsumed(breadFlour);
            var myIngredients =dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(20m, myIngredientBox[0].ouncesConsumed);
            Assert.AreEqual(40m, myIngredientBox[1].ouncesConsumed);
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
            var myIngredients =dbCOC.queryConsumptionOuncesConsumed();
            Assert.AreEqual(10m, myIngredientBox[0].ouncesConsumed);
            Assert.AreEqual(20m, myIngredientBox[1].ouncesConsumed);
            Assert.AreEqual(30m, myIngredientBox[2].ouncesConsumed);
            Assert.AreEqual(40m, myIngredientBox[3].ouncesConsumed);
            Assert.AreEqual(50m, myIngredientBox[4].ouncesConsumed);
        }
    }
}
