using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public IActionResult GenerateTestUsers([FromQuery] int count = 10)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than 0.");
            }

            List<User> fakeUsers = Classes.FakeUserGenerator.GenerateUsers(count);
            
            _context.Users.AddRange(fakeUsers);
            _context.SaveChanges();

            return Ok(new { Message = $"{count} test users created successfully!" });
        }
    }
}