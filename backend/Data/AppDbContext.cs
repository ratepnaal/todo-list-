using Microsoft.EntityFrameworkCore;
using AspSqlProject.Models;

namespace AspSqlProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TodoItem> Todos { get; set; }
    }
}
