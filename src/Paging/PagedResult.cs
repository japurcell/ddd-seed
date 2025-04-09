namespace Paging;

public class PagedResult<T> : PagedResult
{
    public static readonly PagedResult<T> Default = new(string.Empty, 0, []);

    public PagedResult(
        string basePath,
        int totalItems,
        IEnumerable<T> results,
        int currentPage = DefaultCurrentPage,
        int pageSize = DefaultPageSize,
        string? sortOn = default,
        SortDirection? sortDirection = null)
        : base(basePath, totalItems, currentPage, pageSize, sortOn, sortDirection) =>
            Results = results;

    public IEnumerable<T> Results { get; private set; }
}

public class PagedResult
{
    public const int DefaultCurrentPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPages = 7;
    public PagedResult(
        string basePath,
        int totalItems,
        int currentPage = DefaultCurrentPage,
        int pageSize = DefaultPageSize,
        string? sortOn = default,
        SortDirection? sortDirection = null)
    {
        // calculate total pages
        var totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);

        // ensure current page isn't out of range
        if (currentPage < 1)
        {
            currentPage = 1;
        }
        else if (currentPage > totalPages)
        {
            currentPage = totalPages;
        }

        int startPage, endPage;
        if (totalPages <= MaxPages)
        {
            // total pages less than max so show all pages
            startPage = 1;
            endPage = totalPages;
        }
        else
        {
            // total pages more than max so calculate start and end pages
            var maxPagesBeforeCurrentPage = (int)Math.Floor(MaxPages / (decimal)2);
            var maxPagesAfterCurrentPage = (int)Math.Ceiling(MaxPages / (decimal)2) - 1;
            if (currentPage <= maxPagesBeforeCurrentPage)
            {
                // current page near the start
                startPage = 1;
                endPage = MaxPages;
            }
            else if (currentPage + maxPagesAfterCurrentPage >= totalPages)
            {
                // current page near the end
                startPage = totalPages - MaxPages + 1;
                endPage = totalPages;
            }
            else
            {
                // current page somewhere in the middle
                startPage = currentPage - maxPagesBeforeCurrentPage;
                endPage = currentPage + maxPagesAfterCurrentPage;
            }
        }

        // calculate start and end item indexes
        var startIndex = (currentPage - 1) * pageSize;
        var endIndex = Math.Min(startIndex + pageSize - 1, totalItems - 1);

        // create an array of pages that can be looped over
        var pages = Enumerable.Range(startPage, endPage + 1 - startPage);

        // update object instance with all pager properties required by the view
        BasePath = basePath;
        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = totalPages;
        StartPage = startPage;
        EndPage = endPage;
        StartIndex = startIndex;
        EndIndex = endIndex;
        Pages = pages;
        SortOn = sortOn;
        SortDirection = sortDirection;
    }

    public string BasePath { get; private set; }
    public int TotalItems { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int StartPage { get; private set; }
    public int EndPage { get; private set; }
    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
    public IEnumerable<int> Pages { get; private set; }

    public string? SortOn { get; private set; }
    public SortDirection? SortDirection { get; private set; }

    public PagedResult WithPage(int currentPage) =>
        new(BasePath, TotalItems, currentPage, PageSize, SortOn, SortDirection);
}
