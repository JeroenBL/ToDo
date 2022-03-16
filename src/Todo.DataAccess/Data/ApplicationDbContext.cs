using Todo.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Todo.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ToDo> Todo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Locatie naar de DB. Nu Sqlite. Maar dit kan ook SQL of MongoDB zijn.
            // Het werkt allemaal op dezelfde wijze.
            var conn = @"Data Source=c:\\temp\\Todo\\todo.db";
            optionsBuilder.UseSqlite(conn);
        }
    }
}