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
            else  
                    {
                        board.Reserved= true;
                        _context.SaveChanges();
                    }
            return RedirectToAction(nameof(Index));

        }

        


        // POST: ViewUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Rent(int id, [Bind("ID, Reserved")] Board board)
        //{
        //    if (id != board.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(board);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BoardExists(board.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(board);
        //}
        private bool BoardExists(int id)
        {
            return (_context.Board?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
