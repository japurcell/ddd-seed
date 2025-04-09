namespace DddSeed.UnitTests;

[TestClass]
internal class ValueObjectTests
{
    [TestMethod]
    [DataRow(1, 1, true)]
    [DataRow(null, null, true)]
    [DataRow(null, 1, false)]
    [DataRow(1, null, false)]
    public void ItShouldDetermineEquality(int? leftId, int? rightId, bool areEqual)
    {
        var left = leftId is null ? default : new UnderTest(1);
        var right = rightId is null ? default : new UnderTest(1);

        if (areEqual)
        {
            Assert.AreEqual(left, right);
        }
        else
        {
            Assert.AreNotEqual(left, right);
        }

        if (left is not null && right is not null)
        {
            Assert.AreEqual(areEqual, left == right);
            Assert.AreEqual(!areEqual, left != right);
        }
    }

    [TestMethod]
    public void ItShouldDetermineHashCode()
    {
        var v1 = new UnderTest(1);
        var v2 = new UnderTest(1);

        Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
    }
}

internal class UnderTest : ValueObject
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

    public override bool Equals(object? obj) =>
        base.Equals(obj);

    public override int GetHashCode() =>
        base.GetHashCode();

    public static bool operator ==(UnderTest left, UnderTest right) =>
        EqualOperator(left, right);

    public static bool operator !=(UnderTest left, UnderTest right) =>
        NotEqualOperator(left, right);
}
