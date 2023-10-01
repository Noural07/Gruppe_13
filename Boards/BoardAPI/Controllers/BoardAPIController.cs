using BoardAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boards.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardAPIController : ControllerBase
    {
        private readonly BoadAPIContext _context;

        public BoardAPIController(BoadAPIContext context)
        {
            _context = context;
        }

        [HttpPost("rent/{id}")]
        public async Task<IActionResult> Rent(int id)
        {
            var board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            board.Reserved = true;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Board reserved successfully" });
        }
    }
}
