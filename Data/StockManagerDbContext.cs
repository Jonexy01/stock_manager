using Microsoft.EntityFrameworkCore;
using stockmanager.Models;

namespace stockmanager.Data
{
    public class StockManagerDbContext : DbContext
    {
        public StockManagerDbContext(DbContextOptions<StockManagerDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Stock> Stocks { get; set; }
    }
}
