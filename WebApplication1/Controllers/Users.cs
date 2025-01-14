using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;


namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : ControllerBase
    {
        private readonly Data.DatabaseContext _context;

        public MyController(Data.DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet ("Read")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            var totalUsers = await _context.Users.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            if (pageNumber > totalPages)
            {
                return NotFound("Page number exceeds total pages.");
            }

            var users = await _context.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return Ok(users);
        }

        [HttpPost ("Create")]
        public async Task<IActionResult> CreateHuman(
            [FromQuery] string name, 
            [FromQuery] string lastname, 
            [FromQuery] string email, 
            [FromQuery] string phoneNumber, 
            [FromQuery] int age)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Missing required parameters");
            }

            User userToAdd = new User()
            {
                Name = name,
                Lastname = lastname,
                Email = email,
                PhoneNumber = phoneNumber,
                Age = age,
                Cars = new List<Car>()
            };

            _context.Users.Add(userToAdd);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllUsers), new { id = userToAdd.Id }, userToAdd);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUser(
            int id, 
            [FromQuery] string? name, 
            [FromQuery] string? lastname, 
            [FromQuery] string? email, 
            [FromQuery] string? phoneNumber, 
            [FromQuery] int? age)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");

            // Update properties if they are provided
            if (!string.IsNullOrWhiteSpace(name)) user.Name = name;
            if (!string.IsNullOrWhiteSpace(lastname)) user.Lastname = lastname;
            if (!string.IsNullOrWhiteSpace(email)) user.Email = email;
            if (!string.IsNullOrWhiteSpace(phoneNumber)) user.PhoneNumber = phoneNumber;
            if (age.HasValue) user.Age = age.Value;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "An error occurred while updating the user");
            }
        }
        
        [HttpDelete ("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }
    
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
    
            return NoContent(); // HTTP 204
        }
        
        [HttpPatch("Patch/{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromQuery] string name = null, [FromQuery] string email = null)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found");
            }

            // Update fields only if query parameters are provided
            if (!string.IsNullOrEmpty(name))
            {
                user.Name = name;
            }

            if (!string.IsNullOrEmpty(email))
            {
                user.Email = email;
            }

            // Save changes to the database
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // HTTP 204
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Concurrency error: {ex.Message}");
            }
        }
    }
    
    
}