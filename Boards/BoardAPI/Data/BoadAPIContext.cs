using BoardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardAPI.Data
{
    public class BoadAPIContext: DbContext
    {
        public BoadAPIContext(DbContextOptions<BoadAPIContext> options): base(options)
        {
            
        }

        public DbSet <BoardAPIClass> Boards { get; set; }= null;
    }
}
