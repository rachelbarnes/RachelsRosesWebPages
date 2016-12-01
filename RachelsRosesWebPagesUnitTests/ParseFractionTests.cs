using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework;
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class ParseFractionTests {
        [Test]
        public void TestSimpleFractionValue() {
            var divide = new ParseFraction();
            var expected = .6667m;
            var actual = divide.Parse("2/3");
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void TestSimpleFractionValue2() {
            var divide = new ParseFraction();
            var expected = .125m; //i guess the question is whether or not it rounds up or down... i guess it rounds down (this is .125)
            var actual = divide.Parse("1/8");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestSimpleFractionValue3() {
            var divide = new ParseFraction();
            var expected = .0575m; //this was .057... so i guess it did round up?
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
            var expected = 10.3333m;
            var actual = divide.Parse("10 1/3");
            Assert.AreEqual(expected, actual); 
        }
        //i'm getting errors on only a few conversions... i'm not sure why and i haven't noticed the pattern yet.
            //i need to put in a conversion functinoality to parse the decimals back into fractions... 
                //the biggest probelm with that is i lose precision... 
                //for what functionality i have here, i am only setting a solution for a misguided functionality
        [Test]
        public void TestDecimalFractions() {
            var parse = new ParseFraction();
            var expected = 1.5m;
            var actual = parse.Parse("1.5");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestDecimalFractions1() {
            var parse = new ParseFraction();
            var expected = 10.125m;
            var actual = parse.Parse("10.125");
            Assert.AreEqual(expected, actual); 
        }
        [Test]
        public void TestDecimalFractions2() {
            var parse = new ParseFraction();
            var expected = .025m;
            var actual = parse.Parse(".025");
            Assert.AreEqual(expected, actual); 
        }
    }
}
