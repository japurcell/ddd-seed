using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Psns.DDD.UnitTests
{
    [TestClass]
    public class EnumerationTests
    {
        [TestMethod]
        public void ItShouldGetAllFields()
        {
            var cardType = CardType.Amex;
            var fields = Enumeration.GetAll<CardType>();

            Assert.AreEqual("1 Amex,2 Visa", string.Join(",", fields.Select(c => $"{c.Id} {c.Name}")));
        }

        [TestMethod]
        [DataRow(1, 1, true)]
        public void ItShouldDetermineEquality(int leftTypeId, int rightTypeId, bool areEqual)
        {
            var left = FindType(leftTypeId);
            var right = FindType(rightTypeId);

            Assert.AreEqual(areEqual, left.Equals(right));
        }

        [TestMethod]
        [DataRow(1, 1, 0)]
        [DataRow(2, 1, 1)]
        [DataRow(1, 2, -1)]
        public void ItShouldDetermineComparability(int leftTypeId, int rightTypeId, int result)
        {
            var left = FindType(leftTypeId);
            var right = FindType(rightTypeId);

            Assert.AreEqual(result, left.CompareTo(right));
        }

        [TestMethod]
        [DataRow(1, 1, true)]
        [DataRow(2, 1, false)]
        [DataRow(1, 2, false)]
        public void ItShouldDetermineHashCode(int leftTypeId, int rightTypeId, bool areEqual)
        {
            var left = FindType(leftTypeId);
            var right = FindType(rightTypeId);

            Assert.AreEqual(areEqual, left.GetHashCode() == right.GetHashCode());
        }

        [TestMethod]
        public void ItShouldGenerateAName()
        {
            Assert.AreEqual("Visa", CardType.Visa.ToString());
        }

        [TestMethod]
        public void ItShouldResolveFromNameOrId()
        {
            Assert.AreEqual(2, Enumeration.FromDisplayName<CardType>("Visa").Id);
            Assert.AreEqual("Amex", Enumeration.FromValue<CardType>(1).Name);
        }

        static CardType FindType(int id) =>
            Enumeration.GetAll<CardType>().Where(c => c.Id == id).First();
    }

    public class CardType : Enumeration
    {
        public static readonly CardType Amex = new CardType(1, "Amex");
        public static readonly CardType Visa = new CardType(2, "Visa");

        public CardType(int id, string name) : base(id, name) { }
    }
}
