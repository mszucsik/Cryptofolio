/*
 *  Cryptofolio
 *  Version 1.0 (November 30, 2018)
 *  by Michael Szucsik
 *  
 *  I, Michael Szucsik, 000286230, certify that this is my original work.
 *  No other persons work was used without due acknowledgement.
 *  
 */

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
    public class AssetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        /// <summary>
        /// Displays a list of assets with some price info
        /// </summary>
        /// <returns>Asset index view</returns>
        /// 
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Asset.ToListAsync();
            foreach (Asset a in assets)
            {
                var marketPrices = await _context.MarketPrice.Where(m => m.MarketCurrency == a.Code).OrderByDescending(m => m.TimeStamp).ToListAsync();
                a.Daily_Change = 1.00 - (marketPrices[0].CurrentPrice / marketPrices[1].CurrentPrice);
                a.Current_Price = marketPrices[0].CurrentPrice;
            }
            return View(assets);
        }

        // GET: Details
        /// <summary>
        /// Requests the asset detail view
        /// </summary>
        /// <returns>Asset detail view</returns>
        /// 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Asset
                .FirstOrDefaultAsync(m => m.ID == id);

            if (asset == null)
            {
                return NotFound();
            }
            var marketPrices = await _context.MarketPrice.Where(m => m.MarketCurrency == asset.Code).OrderBy(m=>m.TimeStamp).ToListAsync();
            var displayPrices = await _context.MarketPrice.Where(m => m.MarketCurrency == asset.Code).OrderByDescending(m => m.TimeStamp).ToListAsync();
            List<MarketPrice> temp = marketPrices;
            var dailychange = 1.00 - (marketPrices[marketPrices.Count() - 1].CurrentPrice / marketPrices[marketPrices.Count() - 2].CurrentPrice);
            List<String> chartDates = new List<String>();
            List<String> chartRatesUSD = new List<String>();
            foreach (MarketPrice m in marketPrices)
            {

                    chartDates.Add(m.TimeStamp.ToShortDateString());
                    chartRatesUSD.Add(m.CurrentPrice.ToString());

            }

            ViewBag.chartDates = chartDates.ToArray();
            ViewBag.chartRatesUSD = chartRatesUSD.ToArray();
            ViewData["prices"] = displayPrices;
            asset.Daily_Change = dailychange;
            asset.Current_Price = marketPrices[marketPrices.Count()-1].CurrentPrice;

            return View(asset);
        }

        // GET: Create
        /// <summary>
        /// Requests the create asset view
        /// <remarks>
        /// </summary>
        /// <returns>Create asset view</returns>
        /// 
        public async Task<IActionResult> Create()
        {

            var query = from m in _context.MarketPrice
                        orderby m.TimeStamp descending
                        select m.MarketCurrency;
            List<String> codes = await query.Distinct().ToListAsync();

            var assetQuery = from m in _context.MarketPrice
                        orderby m.TimeStamp descending
                        select m;

            List<MarketPrice> displayPrices = await assetQuery.Distinct().ToListAsync();

            ViewData["prices"] = displayPrices;

            ViewData["assets"] = codes;

            return View();
        }

        // POST: Create
        /// <summary>
        /// Allows the admin user to create a new asset (this is different from activating an asset)
        /// <remarks>
        /// Assets can be created with existing price data in the database, once an asset is created
        /// to be linked with that data, it can only be deactivated and never deleted.</remarks>
        /// </summary>
        /// <returns>Asset details view</returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,Name,Code,Activated")] Asset asset)
        {
            if (ModelState.IsValid)
            {

                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }


        // POST: ToggleActive
        /// <summary>
        /// The allows the admin to turn an asset on or off for the system
        /// </summary>
        /// <returns>Asset index view</returns>
        /// 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var asset = await _context.Asset.FindAsync(id);
            if (asset != null)
            {
                asset.Activated = !asset.Activated;
            }
            try
            {
                _context.Update(asset);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;

            }
            return RedirectToAction(nameof(Index));

        }

    }
}
