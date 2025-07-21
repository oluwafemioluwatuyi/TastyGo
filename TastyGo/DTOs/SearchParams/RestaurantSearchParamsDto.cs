namespace TastyGo.DTOs.SearchParams;

public class RestaurantSearchParamsDto
{
    public string? Name { get; set; }
    public bool? IsOpen { get; set; }
    public string? SortBy { get; set; }
}