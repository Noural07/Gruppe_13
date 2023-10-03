using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardsAPI.Model;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Razor.Language;

namespace BoardsAPI.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    
    public class BoardsControllerV2 : ControllerBase
    {
        private readonly BoardAPIContext _context;

        public BoardsControllerV2(BoardAPIContext context)
        {
            _context = context;
        }

        // GET: api/Boards
        [HttpGet]
        [Route("GetAllBoards")]
        public IEnumerable<Board> GetAllBoards()
        {

            return _context.Board.ToArray();
        }


        [HttpGet("GetBoard/{id}")]
        public Board GetBoard(int id)
        {

            Board result = null;

            var resultOfFindMethod = _context.Board.Find(id);

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
        public async Task<ActionResult<Board>> RentBoard(Board board)
        {
            if (board.ID == null)
            {
                return NotFound();
            }

            var boardToUpdate = await _context.Board.FirstOrDefaultAsync(m => m.ID == board.ID);
            boardToUpdate.StartDate = board.StartDate;
            boardToUpdate.EndDate = board.EndDate;

            if (boardToUpdate == null)
            {
                Board deletedBoard = new Board();
                await TryUpdateModelAsync(deletedBoard);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");

                return Ok(deletedBoard);
            }


            if (boardToUpdate.Reserved == false)
            {
                boardToUpdate.Reserved = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The chosen board is already booked");
            }

            if (await TryUpdateModelAsync(
                boardToUpdate,
                "",
                s => s.StartDate, s => s.EndDate))
            {
                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Resten af din eksisterende kode for fejlhåndtering
                }
            }

            return Ok(boardToUpdate);

            //if (_context.Board == null)
            //    return NotFound();

            //if (ModelState.IsValid)
            //{
            //    var surfboard = await _context.Board.FindAsync(board.ID);
            //    if (surfboard == null)
            //    {
            //        return NotFound();
            //    }


            //    // Create a new Rent object
            //    var rent = new Board
            //    {
            //        StartDate = board.StartDate,
            //        EndDate = board.EndDate,
            //        ID = surfboard.ID,

            //    };

            //    _context.Add(rent);
            //    await _context.SaveChangesAsync();

            //    return Ok(nameof(Index));
            //}
            //else
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            //}

            //return Ok(board);
        }


        [HttpPost]
        [Route("RentBoardWithID")]
        public async Task<ActionResult<Board>> RentBoardWithID(int id)
        {
            Board board = new Board();

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var requestBody = await reader.ReadToEndAsync();
                board = JsonSerializer.Deserialize<Board>(requestBody);
            }
            if (ModelState.IsValid)
            {
                foreach (Board surfboard in _context.Board)
                {
                    if (id == surfboard.ID)
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
            if (_context.Board == null)
            {
                return NotFound();
            }
            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            _context.Board.Remove(board);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoardExists(int id)
        {
            return (_context.Board?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
