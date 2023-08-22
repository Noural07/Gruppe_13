using Microsoft.AspNetCore.Mvc;
using Boards.Models;

namespace Boards.Controllers
{
    public class BoardViewController : Controller
    {
        public IActionResult Create()
        {
            Board board = new Board()
            {

                ID = 1,
                Name = "Best Way Hydro",
                Length = 6,
                Width = 21,
                Thickness = 2.75,
                Volume = 38.8,
                Type = "ShortBoard",
                Price = 565,
                Equipment = "",


            };

            return View(board);
        }


    }
}