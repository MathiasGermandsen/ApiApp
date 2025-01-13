namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User : Common 
{
    public string Name { get; set; }
    public string Lastname { get; set; }

    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int Age { get; set; }
    // public List<int> Cars { get; set; }
    
}