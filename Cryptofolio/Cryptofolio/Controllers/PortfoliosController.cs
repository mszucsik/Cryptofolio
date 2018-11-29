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
using Microsoft.AspNetCore.Http;

namespace Cryptofolio.Controllers
{
    public class PortfoliosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PortfoliosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Public Portfolios
        [AllowAnonymous]
        public async Task<IActionResult> Portfolios()
        {
            List<Portfolio> portfolios = await _context.Portfolio.ToListAsync();
            List<Portfolio> displayPortfolios = new List<Portfolio>();
            Portfolio newestPortfolio = new Portfolio();
            Portfolio mostPopular = new Portfolio();
            Portfolio topPortfolio = new Portfolio();
            foreach (Portfolio portfolio in portfolios)
            {
                if (portfolio.Privacy_Status == false)
                {
                    displayPortfolios.Add(portfolio);
                }
            }
            List<Portfolio> extendedPortfolios = getPortfolioStatistics(displayPortfolios);
            foreach (Portfolio portfolio in extendedPortfolios)
            {
                if (portfolio.Creation_Date > newestPortfolio.Creation_Date)
                {
                    newestPortfolio = portfolio;
                }
                if (portfolio.Rating > mostPopular.Rating)
                {
                    mostPopular = portfolio;
                }
                if (portfolio.Percent_Change > topPortfolio.Percent_Change)
                {
                    topPortfolio = portfolio;
                }
            }
            ViewData["newest"] = newestPortfolio;
            ViewData["popular"] = mostPopular;
            ViewData["top"] = topPortfolio;

            return View(extendedPortfolios);
        }

        // GET: Portfolios
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Portfolio> portfolios = await _context.Portfolio.ToListAsync();
            List<Portfolio> displayPortfolios = new List<Portfolio>();
            foreach (Portfolio portfolio in portfolios)
            {
                if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
                {
                    displayPortfolios.Add(portfolio);
                }
            }


            return View(getPortfolioStatistics(displayPortfolios));
        }


        public List<Portfolio> getPortfolioStatistics(List<Portfolio> portfolios)
        {
            var query = from m in _context.MarketPrice
                        orderby m.TimeStamp descending
                        select m;

            List<Asset> assets =  _context.Asset.Where(a => a.Activated == true).ToList();
            List<MarketPrice> marketPrices = query.ToList();
            List<MarketPrice> displayPrices = new List<MarketPrice>();
            List<MarketPrice> dayOldPrices = new List<MarketPrice>();
            var btcPrice = 0.0;
            foreach (Asset a in assets)
            {
                foreach (MarketPrice m in marketPrices)
                {
                    if (m.MarketCurrency == "BTC")
                    {
                        btcPrice = m.CurrentPrice;
                    }
                    if (a.Code == m.MarketCurrency)
                    {
                        displayPrices.Add(m);
                        marketPrices.Remove(m);
                        break;
                    }
                }
            }
            foreach (Asset a in assets)
            {
                foreach (MarketPrice m in marketPrices)
                {

                    if (a.Code == m.MarketCurrency)
                    {
                        dayOldPrices.Add(m);
                        break;
                    }
                }
            }

            foreach (Portfolio p in portfolios)
            {
                double dailyChange = 0;
                double totalChange = 0;
                double percentChange = 0;
                double totalUSD = 0;
                double totalBTC = 0;
                double oldTotal = 0;
                double totalPurchased = 0;

                List<Holding> holdings = _context.Holding.Where(a => a.Portfolio_ID == p.ID).ToList();

                foreach (Holding h in holdings)
                {
                    foreach (MarketPrice m in displayPrices)
                    {
                        if (h.AssetType == m.MarketCurrency)
                        {
                            totalUSD += h.Amount * m.CurrentPrice;
                        }
                    }
                    foreach (MarketPrice m in dayOldPrices)
                    {
                        if (h.AssetType == m.MarketCurrency)
                        {
                            oldTotal += h.Amount * m.CurrentPrice;
                        }
                    }
                    totalPurchased += h.PurchasePrice * h.Amount;
                }

                totalChange = totalUSD - totalPurchased;
                percentChange = ((totalUSD / totalPurchased)-1) * 100;
                dailyChange = (totalUSD - oldTotal) / totalUSD * 100;
                totalBTC = totalUSD / btcPrice;
                p.Total_Change = totalChange;
                p.Daily_Change = dailyChange;
                p.BTC_Value = totalBTC;
                p.USD_Value = totalUSD;
                p.Total_Purchased = totalPurchased;
                p.Percent_Change = percentChange;

            }

            return portfolios;
        }


        // GET: Portfolios/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio
                .FirstOrDefaultAsync(m => m.ID == id);
            if (portfolio == null)
            {
                return NotFound();
            }
            if (portfolio.Privacy_Status == false)
            {
                ViewData["assets"] = await _context.Asset.ToListAsync();
                List<Holding> holdings = await _context.Holding.ToListAsync();
                List<Holding> displayHoldings = new List<Holding>();
                foreach (Holding holding in holdings)
                {
                    if (holding.Portfolio_ID == id)
                    {
                        displayHoldings.Add(holding);
                    }


                }

                portfolio.Holdings = displayHoldings;

                var query = from m in _context.MarketPrice
                            orderby m.TimeStamp descending
                            select m;

                List<Asset> assets = await _context.Asset.Where(a => a.Activated == true).ToListAsync();
                List<MarketPrice> marketPrices = await query.ToListAsync();
                List<MarketPrice> displayPrices = new List<MarketPrice>();
                List<MarketPrice> dayOldPrices = new List<MarketPrice>();
                var btcPrice = 0.0;
                foreach (Asset a in assets)
                {
                    foreach (MarketPrice m in marketPrices)
                    {
                        if (m.MarketCurrency == "BTC")
                        {
                            btcPrice = m.CurrentPrice;
                        }
                        if (a.Code == m.MarketCurrency)
                        {
                            displayPrices.Add(m);
                            marketPrices.Remove(m);
                            break;
                        }
                    }
                }
                foreach (Asset a in assets)
                {
                    foreach (MarketPrice m in marketPrices)
                    {

                        if (a.Code == m.MarketCurrency)
                        {
                            dayOldPrices.Add(m);
                            break;
                        }
                    }
                }

                double dailyChange = 0;
                double totalChange = 0;
                double percentChange = 0;
                double totalUSD = 0;
                double totalBTC = 0;
                double oldTotal = 0;
                double totalPurchased = 0;

                foreach (Holding h in displayHoldings)
                {
                    foreach (MarketPrice m in displayPrices)
                    {
                        if (h.AssetType == m.MarketCurrency)
                        {
                            totalUSD += h.Amount * m.CurrentPrice;
                        }
                    }
                    foreach (MarketPrice m in dayOldPrices)
                    {
                        if (h.AssetType == m.MarketCurrency)
                        {
                            oldTotal += h.Amount * m.CurrentPrice;
                        }
                    }
                    totalPurchased += h.PurchasePrice * h.Amount;
                }

                totalChange = totalUSD - totalPurchased;
                percentChange = ((totalUSD / totalPurchased) - 1) * 100;
                dailyChange = (totalUSD - oldTotal) / totalUSD * 100;
                totalBTC = totalUSD / btcPrice;
                ViewData["dailychange"] = dailyChange;
                ViewData["percentchange"] = percentChange;
                ViewData["totalpurchased"] = totalPurchased;
                ViewData["totalchange"] = totalChange;
                ViewData["totalusd"] = totalUSD;
                ViewData["totalbtc"] = totalBTC;
                ViewData["prices"] = displayPrices;
                ViewData["dayoldprices"] = dayOldPrices;



                return View(portfolio);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Portfolios/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Portfolios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,OwnerID,Name,Rating,Creation_Date,Privacy_Status")] Portfolio portfolio)
        {
            // Setting defaults
            var user = User.Identity.Name;
            portfolio.OwnerID = user;
            portfolio.Rating = 50;
            portfolio.Creation_Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(portfolio);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(portfolio);
        }

        // GET: Portfolios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
            {
                return View(portfolio);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Portfolios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Privacy_Status")] Portfolio portfolio)
        {
            if (id != portfolio.ID)
            {
                return NotFound();
            }

            if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    var newPortfolio = await _context.Portfolio.FindAsync(portfolio.ID);
                    newPortfolio.Name = portfolio.Name;
                    newPortfolio.Privacy_Status = portfolio.Privacy_Status;
                    try
                    {
                        _context.Update(newPortfolio);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PortfolioExists(portfolio.ID))
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
                return View(portfolio);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Portfolios/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio
                .FirstOrDefaultAsync(m => m.ID == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
            {
                return View(portfolio);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Portfolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portfolio = await _context.Portfolio.FindAsync(id);
            if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
            {
                _context.Portfolio.Remove(portfolio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolio.Any(e => e.ID == id);
        }


        public async Task<IActionResult> AddHolding(int id)
        {
            var portfolio = await _context.Portfolio.FindAsync(id);
            if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
            {

                var query = from m in _context.MarketPrice
                            orderby m.TimeStamp descending
                            select m;

                List<Asset> assets = await _context.Asset.Where(a => a.Activated == true).ToListAsync();
                List<MarketPrice> marketPrices = await query.ToListAsync();
                List<MarketPrice> displayPrices = new List<MarketPrice>();
                foreach (Asset a in assets)
                {
                    foreach (MarketPrice m in marketPrices)
                {

                        if (a.Code == m.MarketCurrency)
                        {
                            displayPrices.Add(m);
                            break;
                        }
                    }
                }
                ViewData["id"] = id;
                ViewData["assets"] = displayPrices;
                return View();
            }
            else
            {
                return RedirectToAction("Details", new { id = id });
            }
        }

        // POST: Holdings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHolding([Bind("OwnerID,AssetType,PurchasePrice,Creation_Date,Amount")] Holding holding, IFormCollection form)
        {

            var portfolioId = Convert.ToInt32(form["Portfolio_ID"]);

            var portfolio = await _context.Portfolio.FindAsync(portfolioId);
            if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
            {
                if (form["AssetType"] != "Select Asset")
                {


                    var user = User.Identity.Name;
                    holding.AssetType = form["AssetType"];
                    holding.Portfolio_ID = portfolioId;
                    holding.OwnerID = user;
                    holding.Creation_Date = DateTime.Now;

                    if (ModelState.IsValid)
                    {
                        _context.Add(holding);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = portfolioId });
                    }
                }
                return View(holding);
            }
            else
            {
                return RedirectToAction("Details", new { id = portfolioId });
            }

        }

        // GET: Holdings/Edit/5
        public async Task<IActionResult> EditHolding(int? id)
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

            ViewData["id"] = id;
            return View(holding);
        }

        // POST: Holdings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHolding(int id, [Bind("ID,OwnerID,Creation_Date,Amount")] Holding holding)
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
        public async Task<IActionResult> DeleteHolding(int? id)
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

            ViewData["id"] = id;

            return View(holding);
        }

        // POST: Holdings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHoldingConfirmed(int id)
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
