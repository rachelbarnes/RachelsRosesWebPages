using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace RachelsRosesWebPages.Controllers {
    [TestFixture]
    public class ConvertUnitTests {
        //teaspoons to cups
        [Test]
        public void TeaspoonsToCups() {
            var convert = new Convert();
            var expected = 1;
            var actual = convert.teaspoonsToCups(48m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TeaspoonsToCups2() {
            var convert = new Convert();
            var expected = .75;
            var actual = convert.teaspoonsToCups(36m);
            Assert.AreEqual(expected, actual);
        }
        //teaspoons to tablespoons
        [Test]
        public void TeaspoonsToTablespoons() {
            var convert = new Convert();
            var expected = 1;
            var actual = convert.teaspoonsToTablespoons(3m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TeaspoonsToTablespoons2() {
            var convert = new Convert();
            var expected = 5.67m;
            var actual = convert.teaspoonsToTablespoons(17m);
            Assert.AreEqual(expected, actual);
        }
        //tablespoons to teaspoons
        [Test]
        public void TablespoonsToTeaspoons() {
            var convert = new Convert();
            var expected = 3;
            var actual = convert.TablespoonsToTeaspoons(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TablespoonsToTesaspoons2() {
            var convert = new Convert();
            var expected = 19.50;
            var actual = convert.TablespoonsToTeaspoons(6.5m);
            Assert.AreEqual(expected, actual);
        }
        //tablespoons to cups
        [Test]
        public void TablespoonsToCups() {
            var convert = new Convert();
            var expected = 1;
            var actual = convert.TablespoonsToCups(16m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TablespoonsToCups2() {
            var convert = new Convert();
            var expected = .66;
            var actual = convert.TablespoonsToCups(10.56m);
            Assert.AreEqual(expected, actual);
        }
        //cups to teaspoons
        [Test]
        public void cupsToTeaspoons() {
            var convert = new Convert();
            var expected = 48;
            var actual = convert.CupsToTeaspoons(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void cupsToTeaspoons2() {
            var convert = new Convert();
            var expected = 12;
            var actual = convert.CupsToTeaspoons(.25m);
            Assert.AreEqual(expected, actual);
        }
        //cups to tablespoons
        [Test]
        public void cupsToTablespoons() {
            var convert = new Convert();
            var expected = 16;
            var actual = convert.CupsToTablespoons(1m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void cupsToTablespoons2() {
            var convert = new Convert();
            var expected = 5;
            var actual = convert.CupsToTablespoons(.3125m);
            Assert.AreEqual(expected, actual);
        }
        //changing serving sizes 
        [Test]
        public void ChangeServingSize() {
            var convert = new Convert();
            var expected = 24;
            var actual = convert.ChangeYield(8, 3);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ChangeServingSize2() {
            var convert = new Convert();
            var expected = 16;
            var actual = convert.ChangeYield(12, 1.3m);
            Assert.AreEqual(expected, actual);
        }
        //get multiplier for changing the serving sizes
        [Test]
        public void GetMultiplier() {
            var convert = new Convert();
            var expected = 1.5m;
            var actual = convert.ChangeYieldMultiplier(48, 72);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void GetMultiplier2() {
            var convert = new Convert();
            var expected = 2.4348m;
            var actual = convert.ChangeYieldMultiplier(23, 56);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void GetMultiplier3() {
            var convert = new Convert();
            var expected = .3333m;
            var actual = convert.ChangeYieldMultiplier(48, 16);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestMultiplerOnTeaspoons() {
            var convert = new Convert();
            var expected = 16m;
            var actual = convert.AdjustTeaspoonsBasedOnMultiplier(4m, 4m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestMultiplierOnTeaspoons2() {
            var convert = new Convert();
            var expected = .25m;
            var actual = convert.AdjustTeaspoonsBasedOnMultiplier(2, .125m);
            Assert.AreEqual(expected, actual);
        }
        //convert a string measurement of cups to a decimal value of teaspoons
        [Test]
        public void ConvertStringMeasurementCupsToDecimalTeaspoons() {
            var convert = new Convert();
            var expected = 48m;
            var actual = convert.AdjustToTeaspoons("1 cup");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ConvertStringMeasurementCupsToDecimalTeaspoons2() {
            var convert = new Convert();
            var expected = 3m;
            var actual = convert.AdjustToTeaspoons("1/16 cups");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ConvertStringMeasurementCupsToDecimalTeaspoons3() {
            var convert = new Convert();
            var exepected = 16m;
            var actual = convert.AdjustToTeaspoons("1/3 cup");
            Assert.AreEqual(exepected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementCupsToDecimalTeaspoons4() {
            var convert = new Convert();
            var expected = 312m;
            var actual = convert.AdjustToTeaspoons("6 1/2 cups");
            Assert.AreEqual(expected, actual); 
        }
        //convert a string measuremnt of tablespoons to decimal value of teaspoons
        [Test]
        public void ConvertStringMeasurementTablespoonsToDecimalTeaspoons() {
            var convert = new Convert();
            var expected = 3m;
            var actual = convert.AdjustToTeaspoons("1 Tablespoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementTablespoonsToDecimalTeaspoons2() {
            var convert = new Convert();
            var expected = 1m;
            var actual = convert.AdjustToTeaspoons("1/3 Tablespoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementTablespoonsToDecimalTeaspoons3() {
            var convert = new Convert();
            var expected = 48m;
            var actual = convert.AdjustToTeaspoons("16 tablespoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementTablespoonsToDecimalTeaspoons4() {
            var convert = new Convert();
            var expected = 4.5m;
            var actual = convert.AdjustToTeaspoons("1 1/2 tablespoons");
            Assert.AreEqual(expected, actual); 
        }
        //convert a string measuremnt of teaspoons to decimal value of teaspoons
        [Test]
        public void ConvertStringMeasurementTeaspoonsToDecimalTeaspoons() {
            var convert = new Convert();
            var expected = 1m;
            var actual = convert.AdjustToTeaspoons("1 teaspoon");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementTeaspoonsToDecimalTeaspoons2() {
            var convert = new Convert();
            var expected = .33m;
            var actual = convert.AdjustToTeaspoons("1/3 teaspoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementTeaspoonsToDecimalTeaspoons3() {
            var convert = new Convert();
            var expected = 16;
            var actual = convert.AdjustToTeaspoons("16 teaspoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void ConvertStringMeasurementTeaspoonsToDecimalTeaspoons4() {
            var convert = new Convert();
            var expected = 1.5m;
            var actual = convert.AdjustToTeaspoons("1 1/2 teaspoons");
            Assert.AreEqual(expected, actual); 
        }
        //convert multilevel mesurement (1 cup 2 Tablespoons) to teaspoons
        [Test]
        public void ConvertMultiLevelMeasurementToDecimalTeaspoons() {
            var convert = new Convert();
            var expected = 54m;
            var actual = convert.AdjustToTeaspoons("1 cup 2 tablespoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void SplitMultiLevelMeasurement() {
            var convert = new Convert();
            var expected = new string[] { "1 cu", " 2 tablespoons" };
            var actual = convert.SplitMeasurement("1 cup 2 tablespoons");
            Assert.AreEqual(expected, actual); 
        }
        //[Test]
        //public void ConvertMultiLevelMeasurement2() {
        //    var convert = new Convert();
        //    var expected = 24m;
        //    var actual = convert.SplitMultiLevelMeasurements("1/2 cup");
        //    Assert.AreEqual(expected, actual); 
        //}
    }
}