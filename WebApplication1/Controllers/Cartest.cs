using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CarsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public IActionResult GenerateTestCars([FromQuery] int count = 1)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than 0.");
            }
            
            var validUserIds = _context.Users.Select(u => u.Id).ToList();

            if (!validUserIds.Any())
            {
                return BadRequest("No users available in the database to associate cars with.");
            }
                
            List<Car> fakeCars = Classes.FakeCarGenerator.GenerateCars(count, validUserIds);

            _context.Cars.AddRange(fakeCars);
            _context.SaveChanges();

            return Ok(new { Message = $"{count} cars generated!" });
        }
    }
}