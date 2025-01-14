using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly Data.DatabaseContext _context;

        public CarController(Data.DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("read")]
        public async Task<IActionResult> GetAllCars([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            var totalCars = await _context.Cars.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCars / (double)pageSize);

            if (pageNumber > totalPages)
            {
                return NotFound("Page number exceeds total pages.");
            }

            var cars = await _context.Cars
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            

            return Ok(cars);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCar(
            [FromQuery] string vin, 
            [FromQuery] string manufacturer, 
            [FromQuery] string model, 
            [FromQuery] string type, 
            [FromQuery] string fuel)
        {
            if (string.IsNullOrWhiteSpace(vin) || string.IsNullOrWhiteSpace(manufacturer) || string.IsNullOrWhiteSpace(model))
            {
                return BadRequest("Required car data is missing.");
            }

            Car carToAdd = new Car()
            {
                Vin = vin,
                Manufacturer = manufacturer,
                Model = model,
                Type = type,
                Fuel = fuel,
            };

            _context.Cars.Add(carToAdd);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllCars), new { id = carToAdd.Id }, carToAdd);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromQuery] CarDTO carDto)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound($"Car with ID {id} not found");

            // Update properties based on the provided DTO
            if (!string.IsNullOrWhiteSpace(carDto.Vin)) car.Vin = carDto.Vin;
            if (!string.IsNullOrWhiteSpace(carDto.Manufacturer)) car.Manufacturer = carDto.Manufacturer;
            if (!string.IsNullOrWhiteSpace(carDto.Model)) car.Model = carDto.Model;
            if (!string.IsNullOrWhiteSpace(carDto.Type)) car.Type = carDto.Type;
            if (!string.IsNullOrWhiteSpace(carDto.Fuel)) car.Fuel = carDto.Fuel;

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "An error occurred while updating the car");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound($"Car with ID {id} not found");
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("Patch/{id}")]
        public async Task<IActionResult> PatchCar(int id, [FromQuery] string manufacturer = null, [FromQuery] string model = null)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound($"Car with ID {id} not found");
            }

            // Update fields only if query parameters are provided
            if (!string.IsNullOrEmpty(manufacturer))
            {
                car.Manufacturer = manufacturer;
            }

            if (!string.IsNullOrEmpty(model))
            {
                car.Model = model;
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Concurrency error: {ex.Message}");
            }
        }
    }
}