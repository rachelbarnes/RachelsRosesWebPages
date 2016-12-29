using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework; 
namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class ReaderTests { 
        [Test]
        public void TestReaderDensityDatabaseTextFile() {
            var read = new Reader();
            var expected = new Dictionary<string, decimal>();
            expected.Add("all purpose flour", 5m);
            expected.Add("bread flour", 5.4m);
            expected.Add("white sugar", 7.1m);
            expected.Add("honey", 12m);
            var actual = read.ReadDensityTextFile(@"C:\Users\Rachel\Documents\Visual Studio 2015\Projects\RachelsRosesWebPages\RachelsRosesWebPages\densityTxtDatabase.txt");
            Assert.AreEqual(expected["all purpose flour"], actual["all purpose flour"]);
            Assert.AreEqual(expected["bread flour"], actual["bread flour"]);
            Assert.AreEqual(expected["white sugar"], actual["white sugar"]);
            Assert.AreEqual(expected["honey"], actual["honey"]); 
        }
    }
}
