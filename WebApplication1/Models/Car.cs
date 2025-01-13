namespace WebApplication1.Models;

public class Car : Common
{
    public int Vin { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Fuel { get; set; }
    public User Owners { get; set; }
    
}