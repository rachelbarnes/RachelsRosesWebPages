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
    }
}
