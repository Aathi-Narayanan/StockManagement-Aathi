using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagement.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }

        public int Quantity { get; set; }
        public bool IsActive { get; set; }

    }

    
    
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string CustomerName { get; set; }
        public PurchaseItem? PurchaseItem { get; set; }
    }

    public class PurchaseItem 
    {
        [Key]
        public int PurchaseItemId { get; set; }
        [ForeignKey(nameof(Purchase))]
        public int PurchaseId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Purchase? Purchase { get; set; }
        public Product? Product { get; set; }
        public IList<Product>? Products { get; set; }
    }
    public class Stock
    {
        [Key]
        public int StockId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
        public Product? Product { get; set; }
    }

    public class PurchaseViewModel
    {
        public int PurchaseId { get; set; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public int PurchaseItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
    
}
