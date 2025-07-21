namespace TastyGo.DTOs;

public class RestaurantDto
{
    // Properties that represent the restaurant details

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsOpen { get; set; }
    public string Description { get; set; }

    // Optional: Return menus if needed
    public Guid UserId { get; set; }
    // public List<MenuDto>? Menus { get; set; }
}

class MenuDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid RestaurantId { get; set; }
}