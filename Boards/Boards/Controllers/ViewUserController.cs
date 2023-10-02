using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boards.Data;
using Boards.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Boards.Models;

namespace Boards.Controllers
{
    public class ViewUserController : Controller
    {
        private readonly MvcBoardsContext _context;

        public ViewUserController(MvcBoardsContext context)
        {
            _context = context;
        }

        // GET: ViewUser
        public async Task<IActionResult> Index(
             string sortOrder,
             string currentFilter,
             string searchString,
             int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "Type_desc" : "Type";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_desc" : "Length";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var Boards = from s in _context.Board
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                Boards = Boards.Where(s => s.Equipment.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Name_desc":
                    Boards = Boards.OrderByDescending(s => s.Name);
                    break;
                case "Type_desc":
                    Boards = Boards.OrderByDescending(s => s.Type);
                    break;
                case "Type":
                    Boards = Boards.OrderBy(s => s.Type);
                    break;
                case "Length_desc":
                    Boards = Boards.OrderByDescending(s => s.Length);
                    break;
                case "Length":

                    Boards = Boards.OrderBy(s => s.Length);
                    break;
                default:
                    Boards = Boards.OrderBy(s => s.Name);
                    break;

            }
            Boards = Boards.OrderBy(s => s.Reserved);

            int pageSize = 5;
            return View(await PaginatedList<Board>.CreateAsync(Boards.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: ViewUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.ID == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }
        public async Task<IActionResult> Rent(int? id)
        {
            // Create HttpClient
            using HttpClient client = new HttpClient();

            // Send a POST request to the API
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7292/api/Boards/RentBoard");
            // Get the response message from the API
            var jsonresponse = await response.Content.ReadAsStringAsync();

            var rootObject = JsonSerializer.Deserialize<Board>(jsonresponse);
            // Update the local board entity (if necessary)
           
                await _context.SaveChangesAsync();

               

                // Return a view or message as needed
                return View(response); // You may want to return a different view or message here
            
           
        }


        //// GET: ViewUser/Edit/5
        //public async Task<IActionResult> Rent(int? id)
        //{

        //    if (id == null || _context.Board == null)
        //    {
        //        return NotFound();
        //    }

        //    var board = await _context.Board.FindAsync(id);
        //    if (board == null)
        //    {
        //        return NotFound();
        //    }
        //    //else  
        //    //        {
        //    //            board.Reserved= true;
        //    //            _context.SaveChanges();
        //    //        }

        //    return View(board);

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Tilføj denne attribut for at kræve, at brugeren er logget ind
        public async Task<IActionResult> Rent(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardToUpdate = await _context.Board.FirstOrDefaultAsync(m => m.ID == id);

            if (boardToUpdate == null)
            {
                Board deletedBoard = new Board();
                await TryUpdateModelAsync(deletedBoard);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");

                return View(deletedBoard);
            }

            _context.Entry(boardToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            if (boardToUpdate.Reserved == false)
            {
                boardToUpdate.Reserved = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The chosen board is already booked");
            }

            if (await TryUpdateModelAsync<Board>(
                boardToUpdate,
                "",
                s => s.StartDate, s => s.EndDate, s => s.RowVersion))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Resten af din eksisterende kode for fejlhåndtering
                }
            }

            return View(boardToUpdate);
        }
        private bool BoardExists(int id)
        {
            return (_context.Board?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
