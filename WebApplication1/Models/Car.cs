namespace WebApplication1.Models;

public class Car : Common
{
    public string Vin { get; set; } 
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Fuel { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
}

public class CarDTO
{
    public string Vin { get; set; } 
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Fuel { get; set; }
}