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
using System.Net.Http;
using NuGet.Configuration;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Boards.Controllers
{
    public class ViewUserController : Controller
    {

        private readonly MvcBoardsContext _context;
        private readonly SignInManager<DefaultUser> _signInManager;
        private readonly HttpClient _httpClient = new HttpClient();

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

            var Boards = new List<Board>();

            if (User.Identity.IsAuthenticated)
            {
                string baseURL = "https://localhost:7071/api/Boards/GetAllBoards";

                HttpResponseMessage response = await _httpClient.GetAsync(baseURL);
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a list of Board objects
                Boards = JsonConvert.DeserializeObject<List<Board>>(responseBody);
            }
            else
            {
                string baseURL = "https://localhost:7071/api/V2/Boards/GetAllBoardsWithNoEquipment";

                HttpResponseMessage response = await _httpClient.GetAsync(baseURL);
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a list of Board objects
                Boards = JsonConvert.DeserializeObject<List<Board>>(responseBody);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                Boards = Boards.Where(s => s.Equipment.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "Name_desc":
                    Boards = Boards.OrderByDescending(s => s.Name).ToList();
                    break;
                case "Type_desc":
                    Boards = Boards.OrderByDescending(s => s.Type).ToList();
                    break;
                case "Type":
                    Boards = Boards.OrderBy(s => s.Type).ToList();
                    break;
                case "Length_desc":
                    Boards = Boards.OrderByDescending(s => s.Length).ToList();
                    break;
                case "Length":

                    Boards = Boards.OrderBy(s => s.Length).ToList();
                    break;
                default:
                    Boards = Boards.OrderBy(s => s.Name).ToList();
                    break;

            }

            var filteredBoards = Boards.OrderBy(s => s.Reserved).ToList();

            int pageSize = 5;
            return View(await PaginatedList<Board>.CreateAsync(filteredBoards, pageNumber ?? 1, pageSize));
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
     

            return View(board);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
         // Tilføj denne attribut for at kræve, at brugeren er logget ind
        public async Task<IActionResult> Rent(int? id, byte[] rowVersion, Board board)
        {
            if (id == null)
            {
                return NotFound();
            }


            string baseURL = "https://localhost:7071/api/Boards/RentBoard";

            var boardToUpdate = await _context.Board.FirstOrDefaultAsync(m => m.ID == id);
            boardToUpdate.StartDate = board.StartDate;
            boardToUpdate.EndDate = board.EndDate;
           


            try
            {
              
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(baseURL, boardToUpdate);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Resten af din eksisterende kode for fejlhåndtering
            }

            return View(boardToUpdate);
        }
        private bool BoardExists(int id)
        {
            return (_context.Board?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
