using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardsAPI;
using BoardsAPI.Model;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Razor.Language;

namespace BoardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly BoardAPIContext _context;

        public BoardsController(BoardAPIContext context)
        {
            _context = context;
        }

        // GET: api/Boards
        [HttpGet]
        [Route("GetAllboard")]
        public IEnumerable<Board> GetAllboards()
        {

            return _context.boards.ToArray();
        }

       
        [HttpGet("GetBoard")]
        public Board GetBoard(int id)
        {

            Board result = null;
            
           var resultOfFindMethod = _context.boards.Find(id);

            if (id == resultOfFindMethod.ID) 
            {
                result = resultOfFindMethod;
                return result;
            }
            else
            {
                NotFound();
                return null;
            }

        }

        // PUT: api/Boards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoard(int id, Board board)
        {
            if (id != board.ID)
            {
                return BadRequest();
            }

            _context.Entry(board).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Boards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("RentBoard")]
        public async Task<ActionResult<Board>> RentBoard()
        {
            Board board = new Board();

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var requestBody = await reader.ReadToEndAsync();
                board = JsonSerializer.Deserialize<Board>(requestBody);
            }
            if (ModelState.IsValid)
            {
                foreach (Board surfboard in _context.boards)
                {
                    if (board.ID == board.ID)
                    {
                        board.Reserved = true;
                        break;
                    }
                }
                try
                {
                    _context.Add(board);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "rented successfully" });
                }
                catch (Exception ex)
                {
                    return NotFound(ex);
                }
            }
            return BadRequest(new { Message = "Booking was not created and an erorr occurred" });
        }

        // DELETE: api/Boards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            if (_context.boards == null)
            {
                return NotFound();
            }
            var board = await _context.boards.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            _context.boards.Remove(board);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoardExists(int id)
        {
            return (_context.boards?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
