using BoardsAPI.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace BoardsAPI
{
    public class BoardAPIContext : IdentityDbContext
    {
        public BoardAPIContext(DbContextOptions<BoardAPIContext> options): base(options){}

        public DbSet<Board> Board { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {  
            modelBuilder.Entity<Board>().ToTable("Board");
            base.OnModelCreating(modelBuilder);
        }
    }
}
