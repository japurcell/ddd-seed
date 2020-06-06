using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Psns.DDD.UnitTests
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        [DataRow(1, 1, true)]
        [DataRow(null, null, true)]
        [DataRow(null, 1, false)]
        [DataRow(1, null, false)]
        [DataRow(1, 0, false)]
        [DataRow(0, 1, false)]
        public void ItShouldDetermineEquality(int? leftId, int? rightId, bool areEqual)
        {
            var left = BookFactory(leftId);
            var right = BookFactory(rightId);

            if (areEqual)
            {
                Assert.AreEqual(left, right);
            }
            else
            {
                Assert.AreNotEqual(left, right);
            }

            Assert.AreEqual(areEqual, left == right);
            Assert.AreEqual(!areEqual, left != right);
        }

        [TestMethod]
        public void ItShouldDetermineHashCode()
        {
            var v1 = BookFactory(1);
            var v2 = BookFactory(1);

            Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
        }

        static Book BookFactory(int? id = default) =>
            id == default ? default : new Book(id.Value);
    }

    public class Book : Entity
    {
        public Book(int id) => Id = id;
    }
}
