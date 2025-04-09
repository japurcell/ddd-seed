namespace Paging.UnitTests;

[TestClass]
internal class FilterTests
{
    [TestMethod]
    public void ThrowWithNoFilters()
    {
        var filters = Array.Empty<Filter>();

        Assert.Throws<InvalidOperationException>(() => filters.AsOrExpression<TestModel>());
    }

    [TestMethod]
    public void BuildsOrExpression()
    {
        var term = "1";

        var filters = new[]
        {
            new Filter(nameof(TestModel.Id), FilterOperator.Contains, term),
            new Filter(nameof(TestModel.Name), FilterOperator.Contains, term)
        };

        var source = new[]
        {
            new TestModel(1, "One", ApplicationStatus.Approved),
            new TestModel(2, "Two", ApplicationStatus.Approved)
        }.AsQueryable();

        var query = source.Where(filters.AsOrExpression<TestModel>()).ToArray();

        Assert.ContainsSingle(query);
    }

    [TestMethod]
    public void ThrowWhenUsingContainsOnEnumType()
    {
        var term = "Appro";

        var filters = new[]
        {
            new Filter(nameof(TestModel.Id), FilterOperator.Contains, term),
            new Filter(nameof(TestModel.Status), FilterOperator.Contains, term)
        };

        var source = new[]
        {
            new TestModel(1, "One", ApplicationStatus.Approved),
            new TestModel(2, "Two", ApplicationStatus.Denied)
        }.AsQueryable();

        var query = source.Where(filters.AsOrExpression<TestModel>()).ToArray();

        Assert.ContainsSingle(query);
    }

    internal enum ApplicationStatus
    {
        Approved,
        Denied,
        Pending
    }

    internal record TestModel(int Id, string Name, ApplicationStatus Status);
}
