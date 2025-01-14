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
        public async Task<IActionResult> CreateHuman([FromBody] UserDTO userdto)
        {
            if (userdto == null)
            {
                return BadRequest("Human already exists");
            }

            User userToAdd = new User()
            {
                Name = userdto.Name,
                Lastname = userdto.Lastname,
                Email = userdto.Email,
                PhoneNumber = userdto.PhoneNumber,
                Age = userdto.Age,
                Cars = new List<Car>()
            };

            _context.Users.Add(userToAdd);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllUsers), new { id = userToAdd.Id }, userToAdd);
        }

        [HttpPut ("Update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
                return BadRequest("User ID mismatch");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");

            // Use reflection to dynamically update properties
            foreach (var property in typeof(User).GetProperties())
            {
                var updatedValue = property.GetValue(updatedUser);
                if (updatedValue != null && property.CanWrite)
                {
                    property.SetValue(user, updatedValue);
                }
            }

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