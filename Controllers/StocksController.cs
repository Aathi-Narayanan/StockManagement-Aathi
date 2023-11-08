using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    public class StocksController : Controller
    {
        private readonly StockDbContext _context;

        public StocksController(StockDbContext context)
        {
            _context = context;
        }

        // Get All Stocks
        public async Task<IActionResult> Index()
        {
            var productCount = await _context.Products.CountAsync();
            var sales = await _context.PurchaseItem.ToListAsync();
            ViewBag.outofstack= await _context.Stocks.Where(x=>x.TotalQuantity == 0).CountAsync();
            ViewBag.revenue = sales.Sum(x => x.Amount);
            ViewBag.sale = sales.Count();

            ViewBag.ProductCount = productCount;
              return _context.Stocks != null ? 
                          View(await _context.Stocks.Include(s=>s.Product).ToListAsync()) :
                          Problem("No data Available");
        }

       
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Stocks == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return View(stock);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,ProductId,TotalQuantity")] Stock stock)
        {
            if (id != stock.StockId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.StockId))
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
            return View(stock);
        }

        private bool StockExists(int id)
        {
          return (_context.Stocks?.Any(e => e.StockId == id)).GetValueOrDefault();
        }
    }
}
