﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cryptofolio.Data;
using Cryptofolio.Models;
using Microsoft.AspNetCore.Http;

namespace Cryptofolio.Controllers
{
    public class HoldingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoldingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Holdings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Holding.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            List<MarketPrice> marketRate = await _context.MarketPrice.ToListAsync();
            List<MarketPrice> currentPrices = new List<MarketPrice>();
            foreach(MarketPrice m in marketRate)
            {
                if (m.TimeStamp > DateTime.Now.AddHours(-3))
                {
                    currentPrices.Add(m);
                }
            }
            ViewData["assets"] = currentPrices;
            return View();
        }

        // POST: Holdings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OwnerID,AssetType,PurchasePrice,Creation_Date,Amount")] Holding holding, IFormCollection form)
        {

            if (form["AssetType"] != "Select Asset")
            {
                var user = User.Identity.Name;
                holding.AssetType = form["AssetType"];
                holding.OwnerID = user;
                holding.Creation_Date = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _context.Add(holding);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(holding);
        }

        // GET: Holdings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holding = await _context.Holding.FindAsync(id);
            if (holding == null)
            {
                return NotFound();
            }
            return View(holding);
        }

        // POST: Holdings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OwnerID,Creation_Date,Amount")] Holding holding)
        {
            if (id != holding.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(holding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoldingExists(holding.ID))
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
            return View(holding);
        }

        // GET: Holdings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holding = await _context.Holding
                .FirstOrDefaultAsync(m => m.ID == id);
            if (holding == null)
            {
                return NotFound();
            }

            return View(holding);
        }

        // POST: Holdings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var holding = await _context.Holding.FindAsync(id);
            _context.Holding.Remove(holding);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoldingExists(int id)
        {
            return _context.Holding.Any(e => e.ID == id);
        }
    }
}
