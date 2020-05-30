using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Psns.DDD.UnitTests
{
    [TestClass]
    public class ValueObjectTests
    {
        [TestMethod]
        public void ItShouldDetermineEquality()
        {
            Assert.AreEqual(new UnderTest(1), new UnderTest(1));
        }
    }

    public class UnderTest : ValueObject
    {
        public int Id { get; }
        public string? Name { get; }

        public UnderTest(int id, string? name = default)
        {
            Id = id;
            Name = name;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return Name ?? string.Empty;
        }
    }
}
