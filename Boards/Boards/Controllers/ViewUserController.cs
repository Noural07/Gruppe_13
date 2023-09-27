using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boards.Data;
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

        


        // GET: ViewUser/Edit/5
        public async Task<IActionResult> Rent(int? id)
        {
            
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            //else  
            //        {
            //            board.Reserved= true;
            //            _context.SaveChanges();
            //        }

            return View(board);

        }





        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if(boardToUpdate.Reserved == false)
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
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Board)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Board)databaseEntry.ToObject();

                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate}");
                        }
                        if (databaseValues.EndDate != clientValues.EndDate)
                        {
                            ModelState.AddModelError("EndDate", $"Current value: {databaseValues.EndDate}");
                        }
                        if (databaseValues.RowVersion != clientValues.RowVersion)
                        {
                            ModelState.AddModelError("RowVersion", $"Current value: {databaseValues.RowVersion}");
                        }


                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        boardToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                        
                    }
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
