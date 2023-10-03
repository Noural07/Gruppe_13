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
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BoardsAPI.Controllers.V2
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    
    public class BoardsController : ControllerBase
    {
        private readonly BoardAPIContext _context;

        public BoardsController(BoardAPIContext context)
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

        }




            private bool BoardExists(int id)
        {
            return (_context.Board?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
