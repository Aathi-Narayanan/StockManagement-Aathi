using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItem { get; set; }
      
        public DbSet<Stock> Stocks { get; set; }
    }
}
