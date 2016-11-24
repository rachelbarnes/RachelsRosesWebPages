using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework;
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class FractionTests {
        [Test]
        public void TestSimpleFractionValue() {
            var divide = new ParseFraction();
            var expected = .67m;
            var actual = divide.Parse("2/3");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSimpleFractionValue2() {
            var divide = new ParseFraction();
            var expected = .12m; //i guess the question is whether or not it rounds up or down... i guess it rounds down (this is .125)
            var actual = divide.Parse("1/8");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSimpleFractionValue3() {
            var divide = new ParseFraction();
            var expected = .06m; //this was .057... so i guess it did round up?
            var actual = divide.Parse("45/783");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSimpleFractionValue4() {
            var divide = new ParseFraction();
            var expected = 4.75;
            var actual = divide.Parse("19/4");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestComplexFractionValue() {
            var divide = new ParseFraction();
            var expected = 1.50m;
            var actual = divide.Parse("1 1/2");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestComplexFractionValue2() {
            var divide = new ParseFraction();
            var expected = 2.75m;
            var actual = divide.Parse("2 3/4");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestComplexFractionValue3() {
            var divide = new ParseFraction();
            var expected = 10.33m;
            var actual = divide.Parse("10 1/3");
            Assert.AreEqual(expected, actual); 
        }
    }
}
