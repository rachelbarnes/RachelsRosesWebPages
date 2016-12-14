using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework; 
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class ConvertDensityUnitTest {
        [Test]
        public void TestPercentageIngredientMeasurementToStandardMeasurement() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Bread Flour") {
                measurement = "1 cup"
            };
            var expected = 1m;
            var actual = convert.PercentageUsedMeasurementToStandardMeasurement(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPercentageIngredientMeasurementToStandardMeasurement2() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Bread Flour") {
                measurement = "1 teaspoon"
            };
            var expected = .0208m;
            var actual = convert.PercentageUsedMeasurementToStandardMeasurement(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPercentageIngredientMeasurementToStandardMeasurement3() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Bread Flour") {
                measurement = "6 1/3 cup"
            };
            var expected = 6.3333m;
            var actual = convert.PercentageUsedMeasurementToStandardMeasurement(i);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPercentageIngredientMeasurementToStandardMeasurement4() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Vanilla Extract") {
                measurement = "1 1/2 tablespoons"
            };
            var expected = .0938m;
            var actual = convert.PercentageUsedMeasurementToStandardMeasurement(i);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestOuncesConsumedBasedOnMeasurement() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Confectioner's Sugar") {
                measurement = "3 cups",
                density = 4.4m
            };
            var expected = 13.2m;
            var actual = convert.CalculateOuncesUsed(i);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestOuncesConsumedBasedOnMeasurement2() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Bread Flour") {
                measurement = "5 2/3 cups",
                density = 5.4m
            };
            var expected = 30.60m;
            var actual = convert.CalculateOuncesUsed(i);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestOuncesConsumedBasedOnMeasurement3() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Grandulated Sugar") {
                measurement = "1 teaspoon",
                density = 7.1m
            };
            var expected = .15m;
            var actual = convert.CalculateOuncesUsed(i);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestOuncesConsumedBasedOnMeasurement4() {
            var convert = new ConvertDensity();
            var i = new Ingredient("Baking Soda") { 
                measurement = "1/4 teaspoon",
                density = 8.57m
            };
            var expected = .04m;
            var actual = convert.CalculateOuncesUsed(i);
            Assert.AreEqual(expected, actual); 
        }
    }
}
