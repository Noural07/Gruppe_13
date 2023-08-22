//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using MvcMovie.Data;
//using System;
//using System.Linq;

//namespace Boards.Models.Database
//{
//    public static class SeedData
//    {
//        public static void Initialize(IServiceProvider serviceProvider)
//        {
//            using (var context = new MvcBoardsContext(
//                serviceProvider.GetRequiredService<
//                    DbContextOptions<MvcBoardsContext>>()))
//            {
//                // Look for any movies.
//                if (context.Boards.Any())
//                {
//                    return;   // DB has been seeded
//                }

//                context Boards.AddRange(
//                    new Board
//                    {
//                        Name = "When Harry Met Sally",
//                        Length = DateTime.Parse("1989-2-12"),
//                        Width = "Romantic Comedy",
//                        Thickness = 7.99M
//                    },

//                    new Board
//                    {
//                        Title = "Ghostbusters ",
//                        ReleaseDate = DateTime.Parse("1984-3-13"),
//                        Genre = "Comedy",
//                        Price = 8.99M
//                    },

//                    new Board
//                    {
//                        Title = "Ghostbusters 2",
//                        ReleaseDate = DateTime.Parse("1986-2-23"),
//                        Genre = "Comedy",
//                        Price = 9.99M
//                    },

//                    new Board
//                    {
//                        Title = "Rio Bravo",
//                        ReleaseDate = DateTime.Parse("1959-4-15"),
//                        Genre = "Western",
//                        Price = 3.99M
//                    }
//                );
//                context.SaveChanges();
//            }
//        }
//    }
//}