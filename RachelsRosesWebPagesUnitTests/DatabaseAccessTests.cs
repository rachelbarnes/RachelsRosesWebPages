using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RachelsRosesWebPages;
using NUnit.Framework;
using RachelsRosesWebPages.Models;
using RachelsRosesWebPages.Controllers;

namespace RachelsRosesWebPagesUnitTests {
    [TestFixture]
    public class DatabaseAccessTests {

        [Test]
        public void TestSeveralRecipes() {
            var t = new DatabaseAccess();
            t.initializeDatabase();

            var r = new Recipe("test") {
                yield = 4
            };

            t.InsertRecipe(r);
            r = new Recipe("other") {
                yield = 1
            };
            t.InsertRecipe(r);

            var returns = t.queryRecipe();

            Assert.AreEqual(2, returns.Count());
            Assert.AreEqual(1,returns[0].id);
        }

        [Test]
        public void TestInitializeDatabase() {
            var t = new DatabaseAccess();
            t.initializeDatabase();

            var r = new Recipe("test") {
                yield = 4
            };

            t.InsertRecipe(r);

            r = t.queryRecipe().First();

            Assert.AreEqual("test", r.name);
            Assert.AreEqual(4,r.yield);
            Assert.AreEqual(1,r.id);

            r.name = "horse";
            r.yield = 5;
            t.UpdateRecipe(r);

            Assert.AreEqual("horse", r.name);
            Assert.AreEqual(5,r.yield);
            Assert.AreEqual(1,r.id);
        }
    }
}
