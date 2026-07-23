namespace InventoryManagement.Application.Common;

public class ProductQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? MaxPageSize : value;
    }
}