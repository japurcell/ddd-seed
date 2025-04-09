namespace Paging.UnitTests;

[TestClass]
internal class PagedResultTests
{
    [TestMethod]
    [DataRow(100, 1, 0, 9, 1, 7, 10)]
    [DataRow(100, 5, 40, 49, 2, 8, 10)]
    [DataRow(0, 5, -10, -1, 1, 0, 0)]
    [DataRow(-1, 5, -10, -2, 1, 0, 0)]
    public void CalculatesPageWindowsProperly(
        int totalItems,
        int currentPage,
        int expectedStartIndex,
        int expectedEndIndex,
        int expectedStartPage,
        int expectedEndPage,
        int expectedTotalPages)
    {
        var result = new PagedResult(string.Empty, totalItems, currentPage);

        Assert.AreEqual(expectedStartIndex, result.StartIndex);
        Assert.AreEqual(expectedEndIndex, result.EndIndex);
        Assert.AreEqual(expectedStartPage, result.StartPage);
        Assert.AreEqual(expectedEndPage, result.EndPage);
        Assert.AreEqual(expectedTotalPages, result.TotalPages);
        Assert.AreEqual(PagedResult.DefaultPageSize, result.PageSize);
    }
}
