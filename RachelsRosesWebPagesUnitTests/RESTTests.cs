using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework;
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class RESTTests {
        //these tests may not always pass, depending on sales from the walmart database??
        [Test]
        public void TestBreadFlourRestCallPillsbury() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Pillsbury Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var expected = 2.98m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAllPurposeFlour5Lb() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("All-Purpose Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var expected = 2.98m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAllPurposeFlour10Lb() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("All-Purpose Flour") {
                ingredientId = 1,
                sellingWeight = "10 lb"
            };
            var expected = 4.88m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBreadFlourRestCallKingArthur() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("King Arthur Flour Unbleached Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var expected = 4.12m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBakingPowder() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Baking Powder") {
                ingredientId = 1,
                sellingWeight = "10 oz"
            };
            var expected = 2.9m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBakingPowder2() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Baking Powder") {
                ingredientId = 1,
                sellingWeight = "8.1 oz"
            };
            var expected = 11.40m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBakingSoda4Lb() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Baking Soda") {
                ingredientId = 1,
                sellingWeight = "4 lb"
            };
            var expected = 2.24m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBakingSoda8oz() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Baking Soda") {
                ingredientId = 1,
                sellingWeight = "8 oz"
            };
            var expected = 3.98m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestActiveDryYeast() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Active Dry Yeast") {
                ingredientId = 1,
                sellingWeight = "4 oz"
            };
            var expected = 4.58m;
            var actual = rest.GetItemResponse(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestParseItemResponse() {
            var rest = new MakeRESTCalls();
            var expected = new string[] { "Red Star Active Dry Yeast", "4 oz" };
            var actual = rest.parseItemResponseName("Red Star Active Dry Yeast 4 oz");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestParseItemResponse2() {
            var rest = new MakeRESTCalls();
            var expected = new string[] { "Whole Wheat Flour", "3 1/4 lb" };
            var actual = rest.parseItemResponseName("Whole Wheat Flour 3 1/4 lb");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestComparingWeight() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Butter") {
                sellingWeight = "1 lb"
            };
            var expected = true;
            var actual = rest.CompareWeightInOuncesFromItemResponseToIngredientSellingWeight("Unsalted Butter, 4 count, 1 lb", i);
            Assert.AreEqual(expected, actual); 
        }
        //[Test]
        //public void TestGetItemResponseList() {
        //    var rest = new MakeRESTCalls();
        //    var i = new Ingredient("Bread Flour") {
        //        sellingWeight = "5 lb"
        //    };
        //    var expected = new List<ItemResponse>(); 
        //    var actual = rest.GetListOfItemsFromItemResponse(i);
        //    Assert.AreEqual(expected, actual); 
        //}
    }
}
