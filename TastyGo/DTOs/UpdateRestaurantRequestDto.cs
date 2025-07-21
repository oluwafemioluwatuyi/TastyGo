namespace TastyGo.DTOs;

public class UpdateRestaurantRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool? IsOpen { get; set; }
    public TimeSpan? OpensAt { get; set; }
    public TimeSpan? ClosesAt { get; set; }
    // Optional: Add any other properties that can be updated
}