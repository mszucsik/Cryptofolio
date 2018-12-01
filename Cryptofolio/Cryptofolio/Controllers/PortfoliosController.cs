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

        // GET: Portfolios
        /// <summary>
        /// The main content of the application
        /// </summary>
        /// <remarks>All portfolios are displayed to anonymous, users, and admins.
        /// Some extra data is also returned in the ViewData in order to produce some statistics for this page</remarks>
        /// <returns>Returns a list of Portfolios</returns>
        /// 
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
                var ratings = await _context.Rating.Where(o => o.Portfolio_ID == portfolio.ID).ToListAsync();
                var updatedRating = 0;
                foreach (Rating r in ratings)
                {
                    updatedRating += (int)r.Vote;
                }
                portfolio.Rating += updatedRating;
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
        /// <summary>
        /// The Portfolio management page for users and admin
        /// </summary>
        /// <remarks>For the admin, this displays every portfolio, for a user, only their own portfolios.</remarks>
        /// <returns>Returns a list of Portfolios</returns>
        /// 
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Portfolio> portfolios = await _context.Portfolio.ToListAsync();
            List<Portfolio> displayPortfolios = new List<Portfolio>();
            foreach (Portfolio portfolio in portfolios)
            {
                if ((User.Identity.Name == portfolio.OwnerID) || User.IsInRole("Admin"))
                {
                    var ratings = await _context.Rating.Where(o => o.Portfolio_ID == portfolio.ID).ToListAsync();
                    var updatedRating = 0;
                    foreach (Rating r in ratings)
                    {
                        updatedRating += (int)r.Vote;
                    }
                    portfolio.Rating += updatedRating;
                    displayPortfolios.Add(portfolio);
                }
            }


            return View(getPortfolioStatistics(displayPortfolios));
        }


        // GET: getPortfolioStatistics
        /// <summary>
        /// A method to gather financial statistics to be displayed for each portfolio
        /// </summary>
        /// <returns>Returns a list of Portfolios with virtual data attached</returns>
        /// 
        public List<Portfolio> getPortfolioStatistics(List<Portfolio> portfolios)
        {
            var query = from m in _context.MarketPrice
                        orderby m.TimeStamp descending
                        select m;

            List<Asset> assets = _context.Asset.Where(a => a.Activated == true).ToList();
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
                percentChange = ((totalUSD / totalPurchased) - 1) * 100;
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


        // GET: Details
        /// <summary>
        /// The main view of a portfolio, all stats, holdings and more are displayed for any user logged in or not
        /// </summary>
        /// <remarks>Several peices of data such as data tables, overall totals, ratings
        /// and holdings are all given to the view with ViewData</remarks>
        /// <returns>Returns a portfolio view</returns>
        /// 
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio
                .FirstOrDefaultAsync(m => m.ID == id);

            var ratings = await _context.Rating.Where(o => o.Portfolio_ID == portfolio.ID).ToListAsync();
            var updatedRating = 0;
            foreach (Rating r in ratings)
            {
                updatedRating += (int)r.Vote;
            }
            portfolio.Rating += updatedRating;

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


                MarketPrice latestPrice = await _context.MarketPrice.OrderByDescending(o => o.TimeStamp).FirstAsync();
                List<MarketPrice> prices = await _context.MarketPrice.OrderBy(o => o.TimeStamp).ToListAsync();
                DateTime temp = latestPrice.TimeStamp.AddDays(-30);
                double tempTotalUSD = 0;
                double tempTotalBTC = 0;
                double tempTotalPurchase = 0;
                double tempPercent = 0;
                double tempBTC = 0;
                List<String> chartDates = new List<String>();
                List<String> chartRatesUSD = new List<String>();
                List<String> chartRatesBTC = new List<String>();
                List<String> chartRatesPercent = new List<String>();
                foreach (MarketPrice m in prices)
                {
                    if (m.MarketCurrency == "BTC")
                    {
                        tempBTC = m.CurrentPrice;
                    }
                    if (temp < m.TimeStamp)
                    {
                        temp = m.TimeStamp;
                        chartDates.Add(temp.ToShortDateString());
                        chartRatesUSD.Add(tempTotalUSD.ToString());
                        chartRatesBTC.Add(tempTotalBTC.ToString());
                        tempPercent = tempTotalUSD / tempTotalPurchase * 100;
                        chartRatesPercent.Add(tempPercent.ToString());
                        tempTotalUSD = 0;
                        tempTotalBTC = 0;
                        tempTotalPurchase = 0;
                        tempPercent = 0;
                    }
                    foreach (Holding h in displayHoldings)
                    {
                        if (m.MarketCurrency == h.AssetType)
                        {
                            tempTotalUSD += h.Amount * m.CurrentPrice;
                            tempTotalBTC += h.Amount * m.CurrentPrice / tempBTC;
                            tempTotalPurchase += h.Amount * h.PurchasePrice;
                        }
                    }
                }
                chartDates.Add(temp.ToShortDateString());
                chartRatesUSD.Add(tempTotalUSD.ToString());
                chartRatesBTC.Add(tempTotalBTC.ToString());
                tempPercent = tempTotalUSD / tempTotalPurchase * 100;
                chartRatesPercent.Add(tempPercent.ToString());

                ViewBag.chartDates = chartDates.Skip(1).ToArray();
                ViewBag.chartRatesUSD = chartRatesUSD.Skip(1).ToArray();
                ViewBag.chartRatesPercent = chartRatesPercent.Skip(1).ToArray();
                ViewBag.chartRatesBTC = chartRatesBTC.Skip(1).ToArray();


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

                List<Comment> comments = await _context.Comment.Where(a => a.Portfolio_ID == id).OrderByDescending(a => a.Creation_Date).ToListAsync();
                ViewData["comments"] = comments;

                return View(portfolio);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Create
        /// <summary>
        /// Opens up the create view for portfolio
        /// </summary>
        /// <returns>Create view</returns>
        /// 
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // GET: Portfolio/Create
        /// <summary>
        /// A create area for users to create new portfolios, only basic details needed
        /// </summary>
        /// <returns>Returns a portfolio details view</returns>
        /// 
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
                return RedirectToAction("Details", new { id = portfolio.ID });
            }
            return View(portfolio);
        }

        // GET: Portfolio/Edit
        /// <summary>
        /// A request for a specific portfolio to edit
        /// </summary>
        /// <returns>Returns a portfolio edit view</returns>
        /// 
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

        // POST: Portfolio/Edit
        /// <summary>
        /// A section to edit the basic details for a portfolio
        /// </summary>
        /// <returns>Returns a portfolio details view</returns>
        /// 
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
                }
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Portfolio/Delete
        /// <summary>
        /// A request for a portfolio to delete
        /// </summary>
        /// <returns>Returns a portfolio delete view</returns>
        /// 
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
                var ratings = await _context.Rating.Where(o => o.Portfolio_ID == portfolio.ID).ToListAsync();
                var updatedRating = 0;
                foreach (Rating r in ratings)
                {
                    updatedRating += (int)r.Vote;
                }
                portfolio.Rating += updatedRating;
                return View(portfolio);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Portfolio/Delete
        /// <summary>
        /// A delete page for portfolios. This displays information about the portfolio such
        /// as value and rating to deter the user from deleting.
        /// </summary>
        /// <remarks>The portfolio is deleted</remarks>
        /// <returns>Index of portfolios view is returned</returns>
        /// 
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

        // Check if a portfolio exists in the DB
        private bool PortfolioExists(int id)
        {
            return _context.Portfolio.Any(e => e.ID == id);
        }

        // GET: AddHolding
        /// <summary>
        /// Gets details about assets and market rates and displays a list of options for the user to add a holding
        /// </summary>
        /// <returns>Add holding view</returns>
        /// 
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

        // POST: AddHolding
        /// <summary>
        /// Adds a an asset holding to a portfolio
        /// </summary>
        /// <returns>Detail view for the portfolio that the holding was added to</returns>
        /// 
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

        // GET: EditHolding
        /// <summary>
        /// Provides an edit holding page wich displays data for the holding but only allows the amount to be altered
        /// </summary>
        /// <returns>Edit holding view</returns>
        /// 
        public async Task<IActionResult> EditHolding(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holding = await _context.Holding.FindAsync(id);
            var portfolio = await _context.Portfolio.FirstOrDefaultAsync(m => m.ID == holding.Portfolio_ID);

            if (holding == null)
            {
                return NotFound();
            }
            if ((User.Identity.Name == holding.OwnerID) || User.IsInRole("Admin"))
            {
               
                ViewData["id"] = portfolio.ID;

                return View(holding);
            }
            else
            {
                return RedirectToAction("Details", new { id = portfolio.ID});
            }
        }

        // POST: EditHolding
        /// <summary>
        /// Edits the holding
        /// </summary>
        /// <returns>Details view of the portfolio that the holding belongs to</returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHolding(int id, [Bind("ID", "Amount")] Holding holding)
        {
            if (id != holding.ID)
            {
                return NotFound();
            }
            var editHolding = await _context.Holding.FindAsync(holding.ID);
            editHolding.Amount = holding.Amount;
            if ((User.Identity.Name == editHolding.OwnerID) || User.IsInRole("Admin"))
            {
                try
                {
                    _context.Update(editHolding);
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
            }
            return RedirectToAction("Details", new { id = editHolding.Portfolio_ID });
        }

        // Get: DeleteHolding
        /// <summary>
        /// Gets data for the holding and displays it to the user
        /// </summary>
        /// <returns>Delete holding view</returns>
        /// 
        public async Task<IActionResult> DeleteHolding(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holding = await _context.Holding
                .FirstOrDefaultAsync(m => m.ID == id);

            var portfolio = await _context.Portfolio.FirstOrDefaultAsync(m => m.ID == holding.Portfolio_ID);

            if (holding == null)
            {
                return NotFound();
            }
            if ((User.Identity.Name == holding.OwnerID) || User.IsInRole("Admin"))
            {
                ViewData["id"] = portfolio.ID;
                return View(holding);
            }
            else
            {
                return RedirectToAction("Details", new { id = portfolio.ID });
            }

        }

        // POST: DeleteHolding
        /// <summary>
        /// Deletes the holding
        /// </summary>
        /// <returns>Details view of the portfolio that the holding belongs to</returns>
        /// 
        [HttpPost, ActionName("DeleteHolding")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHoldingConfirmed(int id)
        {
            var holding = await _context.Holding.FindAsync(id);
            _context.Holding.Remove(holding);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = holding.Portfolio_ID });
        }

        private bool HoldingExists(int id)
        {
            return _context.Holding.Any(e => e.ID == id);
        }

        // POST: AddComment
        /// <summary>
        /// Allows a user to post a comment from a partial view while viewing a portfolio
        /// </summary>
        /// <returns>Details view of the portfolio refreshed</returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment([Bind("ID,OwnerID,Portfolio_ID,Creation_Date,Message")] Comment comment)
        {
            comment.Creation_Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = comment.Portfolio_ID });
        }

        // POST: EditComment
        /// <summary>
        /// Allows a user to modify their comment message
        /// </summary>
        /// <returns>Details view of the portfolio refreshed</returns>
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditComment(int id, [Bind("ID,Message")] Comment comment)
        {
            var editComment = await _context.Comment.FindAsync(comment.ID);
            editComment.Message = comment.Message;
            if ((User.Identity.Name == editComment.OwnerID) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(editComment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;

                    }
                }
            }
            return RedirectToAction("Details", new { id = editComment.Portfolio_ID });
        }

        // POST: DeleteComment
        /// <summary>
        /// Allows a user to delete their comment message
        /// </summary>
        /// <returns>Details view of the portfolio refreshed</returns>
        /// 
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment([Bind("ID,OwnerID,Portfolio_ID,Creation_Date,Message")] Comment comment)
        {
            if ((User.Identity.Name == comment.OwnerID) || User.IsInRole("Admin"))
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = comment.Portfolio_ID });
        }

        // POST: UpVoteOnDetails
        /// <summary>
        /// Allows a user to make a vote on a portfolio, in this case, a postive one
        /// </summary>
        /// <returns>Details view of the portfolio refreshed</returns>
        /// 
        public async Task<IActionResult> UpVoteOnDetails(int id)
        {

            var portfolio = await _context.Portfolio.Where(o => o.ID == id).FirstAsync();
            if (portfolio != null)
            {
                await setVote(portfolio, User.Identity.Name, RatingType.Up);
            }
            return RedirectToAction("Details", new { id = id });

        }

        // POST: UpVoteOnList
        /// <summary>
        /// Allows a user to make a vote on a portfolio, in this case, a postive one
        /// This route is for users viewing the portfolio in a list when they rate it
        /// </summary>
        /// <returns>Index of public portfolios</returns>
        /// 
        public async Task<IActionResult> UpVoteOnList(int id)
        {
            var portfolio = await _context.Portfolio.Where(o => o.ID == id).FirstAsync();
            if (portfolio != null)
            {
                await setVote(portfolio, User.Identity.Name, RatingType.Up);
            }
            return RedirectToAction("Portfolios");

        }

        // POST: DownVoteOnDetails
        /// <summary>
        /// Allows a user to make a vote on a portfolio, in this case, a negative one
        /// This route is for users viewing the portfolio as they rate it
        /// </summary>
        /// <returns>Details view of the portfolio refreshed</returns>
        /// 
        public async Task<IActionResult> DownVoteOnDetails(int id)
        {

            var portfolio = await _context.Portfolio.Where(o => o.ID == id).FirstAsync();
            if (portfolio != null)
            {
                await setVote(portfolio, User.Identity.Name, RatingType.Down);
            }
            return RedirectToAction("Details", new { id = id });

        }

        // POST: DownVoteOnList
        /// <summary>
        /// Allows a user to make a vote on a portfolio, in this case, a negative one
        /// </summary>
        /// <returns>Index of public portfolios</returns>
        /// 
        public async Task<IActionResult> DownVoteOnList(int id)
        {
            var portfolio = await _context.Portfolio.Where(o => o.ID == id).FirstAsync();
            if (portfolio != null)
            {
                await setVote(portfolio, User.Identity.Name, RatingType.Down);
            }
            return RedirectToAction("Portfolios");

        }

        // POST: ClearVoteOnDetails
        /// <summary>
        /// Allows a user to make a vote on a portfolio, in this case, a neutral one (clear any votes made)
        /// This route is for users viewing the portfolio as they rate it
        /// </summary>
        /// <returns>Details view of the portfolio refreshed</returns>
        /// 
        public async Task<IActionResult> ClearVoteOnDetails(int id)
        {

            var portfolio = await _context.Portfolio.Where(o => o.ID == id).FirstAsync();
            if (portfolio != null)
            {
                await setVote(portfolio, User.Identity.Name, RatingType.None);
            }
            return RedirectToAction("Details", new { id = id });

        }

        // POST: ClearVoteOnList
        /// <summary>
        /// Allows a user to make a vote on a portfolio, in this case, a neutral one (clear any votes made)
        /// </summary>
        /// <returns>Index of public portfolios</returns>
        /// 
        public async Task<IActionResult> ClearVoteOnList(int id)
        {
            var portfolio = await _context.Portfolio.Where(o => o.ID == id).FirstAsync();
            if (portfolio != null)
            {
                await setVote(portfolio, User.Identity.Name, RatingType.None);
            }
            return RedirectToAction("Portfolios");

        }

        // setVote
        /// <summary>
        /// A method that either creates a new vote for a user for a portfolio, or updates their existing one
        /// </summary>
        /// <returns>Nothing, saves changes to DB</returns>
        /// 
        public async Task setVote([Bind("ID,OwnerID")] Portfolio p, string username, RatingType type)
        {
            if (username != p.OwnerID || User.IsInRole("Admin"))
            {

                var rating = await _context.Rating.Where(o => o.OwnerID == username && o.Portfolio_ID == p.ID).FirstOrDefaultAsync();
                if (rating == null)
                {
                    rating = new Rating();
                    rating.Vote = type;
                    rating.Creation_Date = DateTime.Now;
                    rating.OwnerID = User.Identity.Name;
                    rating.Portfolio_ID = p.ID;
                    _context.Add(rating);
                }
                else
                {
                    rating.Vote = type;
                    rating.Creation_Date = DateTime.Now;
                    _context.Update(rating);
                }

                await _context.SaveChangesAsync();
            }
        }


    }
}
