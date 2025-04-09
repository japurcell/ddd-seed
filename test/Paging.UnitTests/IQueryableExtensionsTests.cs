namespace Paging.UnitTests;

[TestClass]
internal class IQueryableExtensionsTests
{
    [TestMethod]
    [DataRow(101, 1, "Id", SortDirection.Ascending, 10, 1, 10)]
    [DataRow(101, 1, "Id", SortDirection.Descending, 10, 101, 92)]
    [DataRow(101, 1, null, SortDirection.Ascending, 10, 1, 10)]
    [DataRow(101, 1, "Id", null, 10, 1, 10)]
    public void PageItemsProperly(
        int itemCount,
        int? currentPage,
        string? sortOn,
        SortDirection? sortDirection,
        int expectedPageCount,
        int expectedFirstItemId,
        int expectedLastItemId)
    {
        var items = Enumerable.Range(1, itemCount).Select(i => new TestModel(i)).AsQueryable();

        var (paged, totalItemCount) = items.Page(currentPage, 10, sortOn, sortDirection);
        var pagedExecuted = paged.ToArray();

        Assert.AreEqual(itemCount, totalItemCount);
        Assert.AreEqual(expectedPageCount, pagedExecuted.Length);
        Assert.AreEqual(expectedFirstItemId, pagedExecuted[0].Id);
        Assert.AreEqual(expectedLastItemId, pagedExecuted[expectedPageCount - 1].Id);
    }

    internal record TestModel(int Id);
}
