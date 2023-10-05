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
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [HttpGet]
        [Route("GetAllBoardsWithNoEquipment")]

        public IEnumerable<Board> GetAllBoardsWithNoEquipment()
        {
            // Filter boards where the Equipment property is null
            return _context.Board.Where(b => b.Equipment == "")
                                .OrderBy(b => b.ID) // Order by a property like Id if needed
                                .ToList(); // Execute the query and return the results as a list
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
      




            private bool BoardExists(int id)
        {
            return (_context.Board?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
