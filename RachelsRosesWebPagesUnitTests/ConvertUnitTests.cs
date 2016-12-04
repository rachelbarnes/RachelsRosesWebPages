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
        [Test]
        public void TestSplittingMeasurement() {
            var convert = new Convert();
            var expected = new string[] { "1 cup", "2 tablespoons" };
            var actual = convert.SplitMultiLevelMeasurement("1 cup 2 tablespoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplittingMeasurement2() {
            var convert = new Convert();
            var expected = new string[] { "6 1/2 cups", "3 tablespoons" };
            var actual = convert.SplitMultiLevelMeasurement("6 1/2 cups 3 tablespoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TesSplittingMeasurement3() {
            var convert = new Convert();
            var expected = new string[] { "3 1/4 cups", "2 teaspoons" };
            var actual = convert.SplitMultiLevelMeasurement("3 1/4 cups 2 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplittingMeasurements4() {
            var convert = new Convert();
            var expected = new string[] { "1 cup", "1 tablespoon", "1 teaspoon" };
            var actual = convert.SplitMultiLevelMeasurement("1 cup 1 tablespoon 1 teaspoon");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplittingMeasurements5() {
            var convert = new Convert();
            var expected = new string[] { "4 1/3 cup", "1 1/2 tablespoons", "2 1/2 teaspoons" };
            var actual = convert.SplitMultiLevelMeasurement("4 1/3 cup 1 1/2 tablespoons 2 1/2 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplittingMeasurements6() {
            var convert = new Convert();
            var expected = new string[] { "6 3/4 cup", "1 1/2 tablespoons", "2 teaspoons" };
            var actual = convert.SplitMultiLevelMeasurement("6 3/4 cup 1 1/2 tablespoons 2 teaspoons");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSplittingMeasurements7() {
            var convert = new Convert();
            var expected = new string[] { "4 7/8 cups", "3 1/2 tablespoons", "1/2 teaspoons" };
            var actual = convert.SplitMultiLevelMeasurement("4 7/8 cups 3 1/2 tablespoons 1/2 teaspoons");
            Assert.AreEqual(expected, actual); 
        }
        //testing splitting the egg measurements
        [Test]
        public void TestSplittingEggMeasurement() {
            var convert = new Convert();
            var expected = new string[] { "1", "egg" };
            var actual = convert.SplitEggMeasurement("1 egg");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSplittingEggMeasurement2() {
            var convert = new Convert();
            var expected = new string[] { "1 1/2", "eggs" };
            var actual = convert.SplitEggMeasurement("1 1/2 eggs");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSplittingEggMeasurements3() {
            var convert = new Convert();
            var expected = new string[] { "12", "eggs" };
            var actual = convert.SplitEggMeasurement("12 eggs");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSplittingEggMeasurements4() {
            var convert = new Convert();
            var expected = new string[] { "13 3/4", "eggs" };
            var actual = convert.SplitEggMeasurement("13 3/4 eggs");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSplittingEggMeasurements5() {
            var convert = new Convert();
            var expected = new string[] { "256 1/8", "eggs" };
            var actual = convert.SplitEggMeasurement("256 1/8 eggs");
            Assert.AreEqual(expected, actual); 
        }
        //testing accumulated teaspoons from various measurements
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements() {
            var convert = new Convert();
            var expected = 51m;
            var actual = convert.AccumulatedTeaspoonMeasurement("1 cup 1 tablespoon");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements2() {
            var convert = new Convert();
            var expected = 14.5m;
            var actual = convert.AccumulatedTeaspoonMeasurement("4 tablespoons 2 1/2 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements3() {
            var convert = new Convert();
            var expected = 2.5m;
            var actual = convert.AccumulatedTeaspoonMeasurement("2 1/2 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements4() {
            var convert = new Convert();
            var expected = 78.25m;
            var actual = convert.AccumulatedTeaspoonMeasurement("1 1/2 cups 2 tablespoons 1/4 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements5() {
            var convert = new Convert();
            var expected = 110.5m;
            var actual = convert.AccumulatedTeaspoonMeasurement("2 cups 4 tablespoons 2 1/2 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements6() {
            var convert = new Convert();
            var expected = 56.5m;
            var actual = convert.AccumulatedTeaspoonMeasurement("1 cup 2 tablespoons 2 1/2 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AccumulatingTeaspoonsFromVariousMeasurements7() {
            var convert = new Convert();
            var expected = 77.12m;
            var actual = convert.AccumulatedTeaspoonMeasurement("1 1/2 cups 1 tablespoon 2 1/8 teaspoons");
            Assert.AreEqual(expected, actual);
        }
        //testing multiplied teaspoons
        [Test]
        public void AdjustTeaspoonValues() {
            var convert = new Convert();
            var expected = 1.5m;
            var actual = convert.ApplyMultiplierToTeaspoons(1m, 1.5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTeaspoonValues2() {
            var convert = new Convert();
            var expected = 2.5m;
            var actual = convert.ApplyMultiplierToTeaspoons(5m, .5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTeaspoonValues3() {
            var convert = new Convert();
            var expected = 1m;
            var actual = convert.ApplyMultiplierToTeaspoons(3m, .333m);
            Assert.AreEqual(expected, actual);
        }
        //condensed measurements
        [Test]
        public void CondenseTeaspoonMeasurement() {
            var convert = new Convert();
            var expected = "1 cups";
            var actual = convert.CondenseTeaspoonMeasurement(48);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void CondenseTeaspoonMeasurement2() {
            var convert = new Convert();
            var expected = "1 cups 1 tablespoons";
            var actual = convert.CondenseTeaspoonMeasurement(51);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void CondenseTeaspoonMeasurement3() {
            var convert = new Convert();
            var expected = "2 cups";
            var actual = convert.CondenseTeaspoonMeasurement(96);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void CondenseTeaspoonMeasurement4() {
            var convert = new Convert();
            var expected = "4 cups 2 tablespoons 1.5 teaspoons";
            var actual = convert.CondenseTeaspoonMeasurement(199.5m);
            Assert.AreEqual(expected, actual);
        }
        //this failed appropriately... nice
        [Test]
        public void CondenseTeaspoonMeasurement5() {
            var convert = new Convert();
            //var expected = "6.5 cups 10 tablespoons 2.125 teaspoons";
            //this above expected failed appropriately, nice. 
            var expected = "7 cups 2 tablespoons 2.125 teaspoons";
            var actual = convert.CondenseTeaspoonMeasurement(344.125m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void CondenseTeaspoonMeasurement6() {
            var convert = new Convert();
            var expected = "1 tablespoons";
            var actual = convert.CondenseTeaspoonMeasurement(3m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void CondenseTeaspoonsMeasurement7() {
            var convert = new Convert();
            var expected = "0.5 teaspoons";
            var actual = convert.CondenseTeaspoonMeasurement(.5m);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void CondenseTeaspoonsMeasurements8() {
            var convert = new Convert();
            var expected = "3 cups";
            var actual = convert.CondenseTeaspoonMeasurement(144m);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void CondenseTeaspoonsMeasurements9() {
            var convert = new Convert(); 
            var expected = "6 cups";
            var actual = convert.CondenseTeaspoonMeasurement(288m);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void CondenseTeaspoonsMeasurements10() {
            var convert = new Convert();
            var expected = "9 cups";
            var actual = convert.CondenseTeaspoonMeasurement(432m);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void CondenseTeaspoonsMeasurements11() {
            var convert = new Convert();
            var expected = "3.5 cups";
            var actual = convert.CondenseTeaspoonMeasurement(168m);
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void CondenseTeaspoonsMeasurements12() {
            var convert = new Convert();
            var expected = "13.75 cups 2 tablespoons 1.5 teaspoons"; 
            var actual = convert.CondenseTeaspoonMeasurement(667.5m);
            Assert.AreEqual(expected, actual); 
        }
        //adjust total measurement, this is where it all comes together
        [Test]
        public void AdjustTotalMeasurement() {
            var convert = new Convert();
            var expected = "1 cups";
            var actual = convert.AdjustIngredientMeasurement("1/2 cup", 2, 4);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement2() {
            var convert = new Convert();
            var expected = "2 tablespoons";
            var actual = convert.AdjustIngredientMeasurement("1/2 cup", 8, 2);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement3() {
            var convert = new Convert();
            var expected = "1 teaspoons";
            var actual = convert.AdjustIngredientMeasurement("1 tablespoon", 6, 2);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement4() {
            var convert = new Convert();
            var expected = "1 cups 2 tablespoons 1.5 teaspoons";
            var actual = convert.AdjustIngredientMeasurement("9 1/4 tablespoons", 24, 48);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement5() {
            var convert = new Convert();
            var expected = "2 cups 2 tablespoons";
            var actual = convert.AdjustIngredientMeasurement("1/2 cup 1 1/2 teaspoon", 4, 16);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement8() {
            var convert = new Convert();
            var expected = "0.5 cups";
            var actual = convert.AdjustIngredientMeasurement("1/2 cup", 5, 5);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement9() {
            var convert = new Convert();
            var expected = "1 tablespoons 1.5 teaspoons";
            var actual = convert.AdjustIngredientMeasurement("3 tablespoons", 2, 1);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement11() {
            var convert = new Convert();
            var expected = "0.25 teaspoons";
            var actual = convert.AdjustIngredientMeasurement("1/8 teaspoon", 2, 4);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement12() {
            var convert = new Convert();
            var expected = "0.125 teaspoons";
            var actual = convert.AdjustIngredientMeasurement("1/4 teaspoon", 4, 2);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement13() {
            var convert = new Convert();
            var expected = "2 cups 3 tablespoons 0.25 teaspoons";
            var actual = convert.AdjustIngredientMeasurement("1 cup 1 1/2 tablespoons 1/8 teaspoon", 15, 30);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustTotalMeasurement14() {
            var convert = new Convert();
            var expected = "0.25 cups 1 tablespoons";
            var actual = convert.AdjustIngredientMeasurement("1/4 cup 2 tablespoons 2 teaspoon", 8, 6);
            Assert.AreEqual(expected, actual); 
        }
    }
}