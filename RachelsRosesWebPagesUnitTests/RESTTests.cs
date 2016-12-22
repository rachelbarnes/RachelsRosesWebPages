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
            var expected = 2.62m;
            //on 12.14, this was on sale for 2.62, but the normal price is 2.78
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAllPurposeFlour5Lb() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("All-Purpose Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var expected = 3.65m;
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAllPurposeFlour10Lb() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("All-Purpose Flour") {
                ingredientId = 1,
                sellingWeight = "10 lb"
            };
            var expected = 6.29m;
            //this is taking after the God Medal Unblached All-Purpose FLour 10 lb Bag
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAllPurposeFlour10LbNoHyphen() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("All Purpose Flour") {
                ingredientId = 1,
                sellingWeight = "10 lb"
            };
            var expected = 3.98m;
            //Pillsbury is originally 4.64, but on sale it was 3.98
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBreadFlourRestCallKingArthur() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("King Arthur Flour Unbleached Bread Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var expected = 4.2m;
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBakingPowder() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Baking Powder") {
                ingredientId = 1,
                sellingWeight = "10 oz"
            };
            var expected = 2.23m;
            var actual = rest.GetItemResponsePrice(i);
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
            var actual = rest.GetItemResponsePrice(i);
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
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestBakingSoda8oz() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Baking Soda") {
                ingredientId = 1,
                sellingWeight = "32 oz"
            };
            var expected = 2.00m;
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestActiveDryYeast() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Red Star Active Dry Yeast") {
                ingredientId = 1,
                sellingWeight = "4 oz"
            };
            var expected = 4.62m;
            var actual = rest.GetItemResponsePrice(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestParseItemResponse() {
            var rest = new MakeRESTCalls();
            var expected = new string[] { "Red Star Active Dry Yeast", "4 oz" };
            var response = new ItemResponse() {
                name = "Red Star Active Dry Yeast 4 oz"
            };
            var actual = rest.parseItemResponseName(response);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestParseItemResponse2() {
            var rest = new MakeRESTCalls();
            var expected = new string[] { "Whole Wheat Flour", "3 1/4 lb" };
            var response = new ItemResponse() {
                name = "Whole Wheat Flour 3 1/4 lb"
            };
            var actual = rest.parseItemResponseName(response);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestComparingWeight() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Butter") {
                sellingWeight = "1 lb"
            };
            var response = new ItemResponse() {
                name = "Unsalted Buter, 4 count, 1 lb"
            };
            var expected = true;
            var actual = rest.CompareWeightInOuncesFromItemResponseToIngredientSellingWeight(response, i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplitCompareItemResponse() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Red Star Active Dry Yeast") {
                ingredientId = 1,
                sellingWeight = "4 oz"
            };
            var response = new ItemResponse() {
                name = "Red Star: Active Dry Yeast 4 oz"
            };
            var actual = rest.CompareItemResponseNameAndIngredientName(response, i);
            Assert.AreEqual(5, i.name.Split(' ').Count());
            Assert.AreEqual(true, actual);
        }
        [Test]
        public void TestSplitCompareItemResponse2() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("Whole Wheat Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var response = new ItemResponse() {
                name = "King Arthur Flour 100% Whole Grain Whole Wheat Flour, 5.0 LB"
            };
            var actual = rest.CompareItemResponseNameAndIngredientName(response, i);
            Assert.AreEqual(true, actual);
        }
        [Test]
        public void TestSplitCompareItemResponse3() {
            var rest = new MakeRESTCalls();
            var i = new Ingredient("All-Purpose Flour") {
                ingredientId = 1,
                sellingWeight = "5 lb"
            };
            var response = new ItemResponse() {
                name = "King Arthur Flour 100% Whole Grain Whole Wheat Flour, 5.0 LB"
            };
            var actual = rest.CompareItemResponseNameAndIngredientName(response, i);
            Assert.AreEqual(false, actual);
        }
        [Test]
        public void TestAverage6() {
            var rest = new MakeRESTCalls();
            var response = new ItemResponse() {
                name = "All Purpose Flour",
                salePrice = 4.20m,
            };
            var response2 = new ItemResponse() {
                name = "All-Purpose Flour",
                salePrice = 2.69m,
            };
            var response3 = new ItemResponse() {
                name = "AP Flour",
                salePrice = 71.20m
            };
            var response4 = new ItemResponse() {
                name = "Flour",
                salePrice = 3.45m
            };
            var myItemResponses = new List<ItemResponse> { response, response2, response3, response4 };
            var expected = new List<ItemResponse> { response, response2, response4 };
            var actual = rest.AverageItemResponseSalePrices(myItemResponses);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAverage7() {
            var rest = new MakeRESTCalls();
            var response = new ItemResponse() {
                name = "All Purpose Flour",
                salePrice = 4.20m,
            };
            var response2 = new ItemResponse() {
                name = "All-Purpose Flour",
                salePrice = 2.69m,
            };
            var response3 = new ItemResponse() {
                name = "AP Flour",
                salePrice = 71.20m
            };
            var response4 = new ItemResponse() {
                name = "Flour",
                salePrice = 3.45m
            };
            var response5 = new ItemResponse() {
                name = "Flour",
                salePrice = 4.50m,
            };
            var response6 = new ItemResponse() {
                name = "Flour",
                salePrice = 1.30m,
            };
            var response7 = new ItemResponse() {
                name = "Flour",
                salePrice = 6.5m,
            };
            var response8 = new ItemResponse() {
                name = "Flour",
                salePrice = 4.69m,
            };
            var response9 = new ItemResponse() {
                name = "Flour",
                salePrice = 5.31m
            };
            var response10 = new ItemResponse() {
                name = "Flour",
                salePrice = 4.99m,
            };
            var response11 = new ItemResponse() {
                name = "Flour",
                salePrice = 56.83m
            };
            var myItemResponses = new List<ItemResponse> { response, response2, response3, response4, response5, response6, response7, response8, response9, response10 };
            var expected = new List<ItemResponse> { response, response2, response4, response5, response6, response7, response8, response9, response10 };
            var actual = rest.AverageItemResponseSalePrices(myItemResponses);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestAverage8() {
            var rest = new MakeRESTCalls();
            var response = new ItemResponse() {
                name = "All Purpose Flour",
                salePrice = 4.20m,
            };
            var response2 = new ItemResponse() {
                name = "All-Purpose Flour",
                salePrice = 2.69m,
            };
            var response3 = new ItemResponse() {
                name = "AP Flour",
                salePrice = 71.20m
            };
            var response4 = new ItemResponse() {
                name = "Flour",
                salePrice = 3.45m
            };
            var response5 = new ItemResponse() {
                name = "Flour",
                salePrice = 4.50m,
            };
            var response6 = new ItemResponse() {
                name = "Flour",
                salePrice = 1.30m,
            };
            var response7 = new ItemResponse() {
                name = "Flour",
                salePrice = 6.5m,
            };
            var response8 = new ItemResponse() {
                name = "Flour",
                salePrice = 4.69m,
            };
            var response9 = new ItemResponse() {
                name = "Flour",
                salePrice = 5.31m
            };
            var response10 = new ItemResponse() {
                name = "Flour",
                salePrice = 4.99m,
            };
            var response11 = new ItemResponse() {
                name = "Flour",
                salePrice = 56.83m
            };
            var myItemResponses = new List<ItemResponse> { response, response2, response3, response4, response5, response6, response7, response8, response9, response10 };
            var expected = new List<ItemResponse> { response, response2, response4, response5, response6, response7, response8, response9, response10 };
            var actual = rest.AverageItemResponseSalePrices(myItemResponses);
            Assert.AreEqual(expected, actual);
        }
    }
}
