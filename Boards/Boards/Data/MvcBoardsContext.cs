
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Boards.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Boards.Data
{
    public class MvcBoardsContext : IdentityDbContext<DefaultUser>
    {
        public MvcBoardsContext (DbContextOptions<MvcBoardsContext> options)
            : base(options)
        {
            
        }
        public DbSet<Board> Board { get; set; } = default!;
    }
}
