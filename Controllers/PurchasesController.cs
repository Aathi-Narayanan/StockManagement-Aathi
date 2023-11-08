using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly StockDbContext _context;

        public PurchasesController(StockDbContext context)
        {
            _context = context;
        }

        // Logic for Get Sales Details
        public IActionResult Index()
        {
            var purchaseItems = new List<PurchaseItem>();
          purchaseItems = _context.PurchaseItem.Include(si => si.Purchase).Include(pi => pi.Product).ToList();
            var purchaseViewModels = new List<PurchaseViewModel>();

            foreach (var purchaseItem in purchaseItems)
            {
                var purchaseViewModel = new PurchaseViewModel
                {
                    PurchaseId = purchaseItem.PurchaseId,
                    Date = purchaseItem.Purchase.Date,
                    SupplierName = purchaseItem.Purchase.CustomerName,
                    PurchaseItemId = purchaseItem.PurchaseItemId,
                    ProductId = purchaseItem.ProductId,
                    ProductName = purchaseItem.Product?.Name,
                    Quantity = purchaseItem.Quantity,
                    Amount = purchaseItem.Amount
                };

                purchaseViewModels.Add(purchaseViewModel);
               
            }
            return View(purchaseViewModels);

        }
        // Create Excel
        [HttpGet]
        public ActionResult GenerateExcel(int duration)
        {
            try
            {
                var firstDay = DateTime.Now.AddDays(-(int)duration);
                var purchaseItems = new List<PurchaseItem>();
                if (duration == null || duration == 0)
                    purchaseItems = _context.PurchaseItem.Include(si => si.Purchase).Include(pi => pi.Product).ToList();
                else
                    purchaseItems = _context.PurchaseItem.Include(si => si.Purchase).Where(s => s.Purchase.Date < DateTime.Now && s.Purchase.Date > firstDay).Include(pi => pi.Product).ToList();
                var purchaseViewModels = new List<PurchaseViewModel>();

                foreach (var purchaseItem in purchaseItems)
                {
                    var purchaseViewModel = new PurchaseViewModel
                    {
                        PurchaseId = purchaseItem.PurchaseId,
                        Date = purchaseItem.Purchase.Date,
                        SupplierName = purchaseItem.Purchase.CustomerName,
                        PurchaseItemId = purchaseItem.PurchaseItemId,
                        ProductId = purchaseItem.ProductId,
                        ProductName = purchaseItem.Product?.Name,
                        Quantity = purchaseItem.Quantity,
                        Amount = purchaseItem.Amount
                    };

                    purchaseViewModels.Add(purchaseViewModel);
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Purchase Details");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "PurchaseId";
                    worksheet.Cell(currentRow, 2).Value = "Purchase Date";
                    worksheet.Cell(currentRow, 3).Value = "Customer Name";
                    worksheet.Cell(currentRow, 4).Value = "Product Name";
                    worksheet.Cell(currentRow, 5).Value = "Quantity";
                    worksheet.Cell(currentRow, 6).Value = "Amount";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
                    foreach (var user in purchaseViewModels)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.ProductId;
                        worksheet.Cell(currentRow, 2).Value = user.Date.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = user.SupplierName;
                        worksheet.Cell(currentRow, 4).Value = user.ProductName;
                        worksheet.Cell(currentRow, 5).Value = user.Quantity;
                        worksheet.Cell(currentRow, 6).Value = user.Amount;
                    }
                    var fileName = string.Format("PurchaseDetails_" + DateTime.Now.ToString("yyyyMMdd") + "-" + ".xlsx");
                    var fileId = Guid.NewGuid().ToString() + "_" + fileName;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        var content = memoryStream.ToArray();
                        HttpContext.Session.Set("fileId", content);
                    }
                    var fileNames = string.Format("PurchaseDetails_" + DateTime.Now.ToString("dd/MM/yyyy") + "-" + ".xlsx");
                    if (HttpContext.Session.Get("fileId") != null)
                    {
                        byte[] data = HttpContext.Session.Get("fileId");
                        return File(data, "application/vnd.ms-excel", fileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return new EmptyResult();
        }

        // Logic for Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Purchases == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchases/Create
        public async Task<IActionResult> Create()
        {
            var products = await _context.Products.ToListAsync();
            var purchase = new Purchase()
            {
                PurchaseItem = new PurchaseItem(),
            };
            purchase.PurchaseItem.Products = products;
            return View(purchase);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.Where(s => s.ProductId == purchase.PurchaseItem.ProductId).FirstOrDefault();
                var totalAmount = purchase.PurchaseItem.Quantity * product.Price;
                purchase.PurchaseItem.Amount = totalAmount;
                await _context.AddAsync(purchase);
                await _context.SaveChangesAsync();
                var stock = _context.Stocks.Where(s => s.ProductId == purchase.PurchaseItem.ProductId).FirstOrDefault();
                if (stock != null)
                {
                    stock.TotalQuantity = product.Quantity - purchase.PurchaseItem.Quantity;
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    stock = new Stock()
                    {
                        ProductId = purchase.PurchaseItem.ProductId,
                        TotalQuantity = product.Quantity - purchase.PurchaseItem.Quantity
                    };
                    _context.Add(stock);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(purchase);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Purchases == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            purchase.PurchaseItem = await _context.PurchaseItem.Where(s => s.PurchaseId == purchase.PurchaseId).FirstOrDefaultAsync();
            purchase.PurchaseItem.Products = await _context.Products.ToListAsync();
            return View(purchase);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PurchaseId,SupplierName,ProductId,Quantity,TotalAmount")] Purchase purchase)
        {
            if (id != purchase.PurchaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.PurchaseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Purchases == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Purchases == null)
            {
                return Problem("Entity set 'StockDbContext.Purchases'  is null.");
            }
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
          return (_context.Purchases?.Any(e => e.PurchaseId == id)).GetValueOrDefault();
        }
    }
}
