using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Boards.Models;

namespace Boards.Data
{
    public class MvcBoardsContext : DbContext
    {
        public MvcBoardsContext (DbContextOptions<MvcBoardsContext> options)
            : base(options)
        {
        }

        public DbSet<Boards.Models.Board> Board { get; set; } = default!;
    }
}
