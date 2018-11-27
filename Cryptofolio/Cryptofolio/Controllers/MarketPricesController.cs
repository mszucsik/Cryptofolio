using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cryptofolio.Data;
using Cryptofolio.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cryptofolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MarketPricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketPricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MarketPrices
        public async Task<IActionResult> Index()
        {
            return View(await _context.MarketPrice.ToListAsync());
        }

        // GET: MarketPrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketPrice = await _context.MarketPrice
                .FirstOrDefaultAsync(m => m.ID == id);
            if (marketPrice == null)
            {
                return NotFound();
            }

            return View(marketPrice);
        }

        // GET: MarketPrices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MarketPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CurrentPrice,TimeStamp,LogoUrl,MarketCurrency,MarketCurrencyLong")] MarketPrice marketPrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marketPrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marketPrice);
        }

        // GET: MarketPrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketPrice = await _context.MarketPrice.FindAsync(id);
            if (marketPrice == null)
            {
                return NotFound();
            }
            return View(marketPrice);
        }

        // POST: MarketPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CurrentPrice,TimeStamp,LogoUrl,MarketCurrency,MarketCurrencyLong")] MarketPrice marketPrice)
        {
            if (id != marketPrice.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marketPrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketPriceExists(marketPrice.ID))
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
            return View(marketPrice);
        }

        // GET: MarketPrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketPrice = await _context.MarketPrice
                .FirstOrDefaultAsync(m => m.ID == id);
            if (marketPrice == null)
            {
                return NotFound();
            }

            return View(marketPrice);
        }

        // POST: MarketPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marketPrice = await _context.MarketPrice.FindAsync(id);
            _context.MarketPrice.Remove(marketPrice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarketPriceExists(int id)
        {
            return _context.MarketPrice.Any(e => e.ID == id);
        }
    }
}
