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

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHuman([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Human already exists");
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, user);
        }
    }
}