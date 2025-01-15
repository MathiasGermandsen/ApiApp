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

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateUser([FromQuery] int id, [FromQuery] UserDTO userDTO)
        {
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (userToUpdate == null)
            {
                return NotFound("User not found.");
            }

            if (userDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            // Update fields if new values are provided
            if (!string.IsNullOrEmpty(userDTO.Name))
            {
                userToUpdate.Name = userDTO.Name;
            }

            if (!string.IsNullOrEmpty(userDTO.Lastname))
            {
                userToUpdate.Lastname = userDTO.Lastname;
            }

            if (!string.IsNullOrEmpty(userDTO.Email))
            {
                userToUpdate.Email = userDTO.Email;
            }

            if (!string.IsNullOrEmpty(userDTO.PhoneNumber))
            {
                userToUpdate.PhoneNumber = userDTO.PhoneNumber;
            }

            if (userDTO.Age > 0)
            {
                userToUpdate.Age = userDTO.Age;
            }

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return Ok(userToUpdate);
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