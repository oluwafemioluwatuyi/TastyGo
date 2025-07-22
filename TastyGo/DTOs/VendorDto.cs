namespace TastyGo.DTOs;

public class VendorDto
{
    public string? StoreName { get; set; }
    public List<RestaurantDto> Restaurants { get; set; }

}
