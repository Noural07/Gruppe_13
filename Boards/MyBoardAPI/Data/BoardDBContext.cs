using Microsoft.EntityFrameworkCore;
using MyBoardAPI.Models;

namespace MyBoardAPI.Data
{
    public class BoardDBContext : DbContext
    {
        public BoardDBContext(DbContextOptions<BoardDBContext> options) : base(options)
        {

        }

        public DbSet<BoardAPIClass>? myBoards { get; set; }
    }
}
