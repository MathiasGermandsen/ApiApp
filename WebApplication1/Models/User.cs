namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User : Common 
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    
    public List<Car> Cars { get; set; }
    
}