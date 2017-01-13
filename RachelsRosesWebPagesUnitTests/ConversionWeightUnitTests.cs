using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace RachelsRosesWebPages {
    [TestFixture]
    public class ConversionWeightUnitTests {
        [Test]
        public void TestPoundsToOunces() {
            var convert = new ConvertWeight();
            var expected = 16m;
            var actual = convert.PoundsToOunces(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPoundsToOunces2() {
            var convert = new ConvertWeight();
            var expected = 88m;
            var actual = convert.PoundsToOunces(5.5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPoundsToOunces3() {
            var convert = new ConvertWeight();
            var expected = 1642.08m;
            var actual = convert.PoundsToOunces(102.63m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToPounds() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.OuncesToPounds(16m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToPounds2() {
            var convert = new ConvertWeight();
            var expected = 5.25m;
            var actual = convert.OuncesToPounds(84m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToPounds3() {
            var convert = new ConvertWeight();
            var expected = 10.12m;
            var actual = convert.OuncesToPounds(162m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestQuartsToOunces() {
            var convert = new ConvertWeight();
            var expected = 32m;
            var actual = convert.QuartsToOunces(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestQuartsToOunces2() {
            var convert = new ConvertWeight();
            var expected = 48m;
            var actual = convert.QuartsToOunces(1.5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestQuartsToOunces3() {
            var convert = new ConvertWeight();
            var expected = 182.08m;
            var actual = convert.QuartsToOunces(5.69m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToQuarts() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.OuncesToQuarts(32m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToQuarts2() {
            var convert = new ConvertWeight();
            var expected = 1.33m;
            var actual = convert.OuncesToQuarts(42.56m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToQuarts3() {
            var convert = new ConvertWeight();
            var expected = 6.5m;
            var actual = convert.OuncesToQuarts(208m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGallonsToOunces() {
            var convert = new ConvertWeight();
            var expected = 128m;
            var actual = convert.GallonsToOunces(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGallonsToOunces2() {
            var convert = new ConvertWeight();
            var expected = 192m;
            var actual = convert.GallonsToOunces(1.5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGallonsToOunces3() {
            var convert = new ConvertWeight();
            var expected = 966.40m;
            var actual = convert.GallonsToOunces(7.55m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToGallons() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.OuncesToGallons(128m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToGallons2() {
            var convert = new ConvertWeight();
            var expected = 1.56m;
            var actual = convert.OuncesToGallons(199.68m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPintsToOunces() {
            var convert = new ConvertWeight();
            var expected = 16m;
            var actual = convert.PintsToOunces(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPintsToOunces2() {
            var convert = new ConvertWeight();
            var expected = 157.60m;
            var actual = convert.PintsToOunces(9.85m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToPints() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.OuncesToPints(16m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToPints2() {
            var convert = new ConvertWeight();
            var expected = 7.25m;
            var actual = convert.OuncesToPints(116m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestCupsToOunces() {
            var convert = new ConvertWeight();
            var expected = 8m;
            var actual = convert.CupsToOunces(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestCupsToOunces2() {
            var convert = new ConvertWeight();
            var expected = 16m;
            var actual = convert.CupsToOunces(2m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToCups() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.OuncesToCups(8m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToCups2() {
            var convert = new ConvertWeight();
            var expected = 9.5m;
            var actual = convert.OuncesToCups(76m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGramsToOunces() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.GramsToOunces(28.3495m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGramsToOunces2() {
            var convert = new ConvertWeight();
            var expected = 5m;
            var actual = convert.GramsToOunces(141.748m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGramsToOunces3() {
            var convert = new ConvertWeight();
            var expected = 0.07m;
            var actual = convert.GramsToOunces(2);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGramsToOunces4() {
            var convert = new ConvertWeight();
            var expected = 3.95m;
            var actual = convert.GramsToOunces(112m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToGrams() {
            var convert = new ConvertWeight();
            var expected = 28.35m;
            var actual = convert.OuncesToGrams(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToGrams2() {
            var convert = new ConvertWeight();
            var expected = 179.45m;
            var actual = convert.OuncesToGrams(6.33m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToGrams3() {
            var convert = new ConvertWeight();
            var expected = 326.02m;
            var actual = convert.OuncesToGrams(11.5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestOuncesToGrams4() {
            var convert = new ConvertWeight();
            var expected = 56.7m;
            var actual = convert.OuncesToGrams(2m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPoundsToGrams() {
            var convert = new ConvertWeight();
            var expected = 453.59m;
            var actual = convert.PoundsToGrams(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestPoundsToGrams2() {
            var convert = new ConvertWeight();
            var expected = 2267.96m;
            var actual = convert.PoundsToGrams(5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGramsToPounds() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.GramsToPounds(453.592m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestGramsToPounds2() {
            var convert = new ConvertWeight();
            var expected = .24m;
            var actual = convert.GramsToPounds(106.8663m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplitWeightMeasurement() {
            var convert = new ConvertWeight();
            var expected = new string[] { "1", "pound" };
            var actual = convert.SplitWeightMeasurement("1 pound");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplitWeightMeasurement2() {
            var convert = new ConvertWeight();
            var expected = new string[] { "1", "gallon whole milk" };
            var actual = convert.SplitWeightMeasurement("1 gallon whole milk");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplitWeightMeasurement3() {
            var convert = new ConvertWeight();
            var expected = new string[] { "1/2", "pint heavy whipping cream" };
            var actual = convert.SplitWeightMeasurement("1/2 pint heavy whipping cream");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplitWeightMeasurement4() {
            var convert = new ConvertWeight();
            var expected = new string[] { "11", "oz" };
            var actual = convert.SplitWeightMeasurement("11oz");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplitWeightMeasurement5() {
            var convert = new ConvertWeight();
            var expected = new string[] { "56", "Gall" };
            var actual = convert.SplitWeightMeasurement("56Gall");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces() {
            var convert = new ConvertWeight();
            var expected = 128m;
            var actual = convert.ConvertWeightToOunces("1 gallon");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces2() {
            var convert = new ConvertWeight();
            var expected = 32m;
            var actual = convert.ConvertWeightToOunces("1/4 gallon milk");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces3() {
            var convert = new ConvertWeight();
            var expected = 16m;
            var actual = convert.ConvertWeightToOunces("1 pint");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces4() {
            var convert = new ConvertWeight();
            var expected = 108m;
            var actual = convert.ConvertWeightToOunces("6.75 pint");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces5() {
            var convert = new ConvertWeight();
            var expected = 16m;
            var actual = convert.ConvertWeightToOunces("1 lb");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces6() {
            var convert = new ConvertWeight();
            var expected = 58.67m;
            var actual = convert.ConvertWeightToOunces("3 2/3 pint");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces7() {
            var convert = new ConvertWeight();
            var expected = 8m;
            var actual = convert.ConvertWeightToOunces("1 cup");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces8() {
            var convert = new ConvertWeight();
            var expected = 24m;
            var actual = convert.ConvertWeightToOunces("3 cups");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces9() {
            var convert = new ConvertWeight();
            var expected = 128m;
            var actual = convert.ConvertWeightToOunces("1 gallon");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces10() {
            var convert = new ConvertWeight();
            var expected = 377.60m; ;
            var actual = convert.ConvertWeightToOunces("2.95 gallons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces11() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.ConvertWeightToOunces("28.35 gram");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces12() {
            var convert = new ConvertWeight();
            var expected = 2.26m;
            var actual = convert.ConvertWeightToOunces("64 grams");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces13() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.ConvertWeightToOunces("1 ounce");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces14() {
            var convert = new ConvertWeight();
            var expected = 3.67m;
            var actual = convert.ConvertWeightToOunces("3 2/3 ounces");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces15() {
            var convert = new ConvertWeight();
            var expected = 1.67m;
            var actual = convert.ConvertWeightToOunces("1 2/3 ounces");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces16() {
            var convert = new ConvertWeight();
            var expected = 13.12m;
            var actual = convert.ConvertWeightToOunces("13 1/8 ounces");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertWeightToOunces17() {
            var convert = new ConvertWeight();
            var expected = .33m;
            var actual = convert.ConvertWeightToOunces("1/3 ounces");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestCOovertImproperWeight() {
            var convert = new ConvertWeight();
            var expected = 0m;
            var actual = convert.ConvertWeightToOunces("6)");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestConvertImproperWeight() {
            var convert = new ConvertWeight();
            var expected = 0m;
            var actual = convert.ConvertWeightToOunces("5-6");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void SplitEggMeasurement() {
            var convert = new ConvertWeight();
            var expected = new string[] { "1", "egg" };
            var actual = convert.SplitWeightMeasurement("1 egg");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void getNumberOfEggsSoldFromSellingQuantity() {
            var convert = new ConvertWeight();
            var expected = 12m;
            var actual = convert.NumberOfEggsFromSellingQuantity("dozen");
            Assert.AreEqual(expected, actual);  
        }
        [Test]
        public void getNumberOfEggsSoldFromSellingQuantity2() {
            var convert = new ConvertWeight();
            var expected = 12m;
            var actual = convert.NumberOfEggsFromSellingQuantity("1 dozen");
            Assert.AreEqual(expected, actual);  
        }
        [Test]
        public void getNumberOfEggsSoldFromSellingQuantity3() {
            var convert = new ConvertWeight();
            var expected = 24m;
            var actual = convert.NumberOfEggsFromSellingQuantity("2 dozen");
            Assert.AreEqual(expected, actual);  
        }
        [Test]
        public void getNumberOfEggsSoldFromSellingQuantity4() {
            var convert = new ConvertWeight();
            var expected = 36m;
            var actual = convert.NumberOfEggsFromSellingQuantity("36");
            Assert.AreEqual(expected, actual);  
        }
        [Test]
        public void getEggsOuncesConsumed() {
            var convert = new ConvertWeight();
            var expected = 1m;
            var actual = convert.EggsConsumedFromIngredientMeasurement("1 egg");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void getEggsOuncesConsumed2() {
            var convert = new ConvertWeight();
            var expected = 1.5m;
            var actual = convert.EggsConsumedFromIngredientMeasurement("1 1/2 eggs");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void getEggsOuncesConsumed3() {
            var convert = new ConvertWeight();
            var expected = 2m;
            var actual = convert.EggsConsumedFromIngredientMeasurement("2 eggs");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void getEggsOuncesConsumed4() {
            var convert = new ConvertWeight();
            var expected = 12.75m;
            var actual = convert.EggsConsumedFromIngredientMeasurement("12 3/4 eggs");
            Assert.AreEqual(expected, actual); 
        }
        /*
       i need to think about what I want the eggs to look like in all of this:  
       for ingredients, eggs should have: 
        name: "eggs"
        meas = "2 eggs"
        class = "eggs"
        type = "egg"
        price = 2/12 * eggPrice
       for densities, eggs should have: 
        density = 1.70m
        sellingweight = "dozen" || "12" (this will obviously have to be different from the rest of the ingredients... 
        sellingweightounces = 12||24||36
        priceperounce = eggPrice /  sellingweightounces
       for consumption, eggs should have: 
        density = 1.70 for each egg
        ouncesCon = 1||2
        ouncesremaining = sellingweightounces - ounces con
       for costs, eggs should have: 
        sellingweight = "dozen" || "12"
        sellingPrice= 2.99m
        pricePerOunce = 2.99 / 12

        make sure to do the update table methods as well


        */
    }
}
