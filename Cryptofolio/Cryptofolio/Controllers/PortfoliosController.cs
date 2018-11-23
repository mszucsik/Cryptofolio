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
                    // TODO: Calculate percent change
                    portfolio.Percent_Change = 23.32;


                    displayPortfolios.Add(portfolio);
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
            }

            ViewData["newest"] = newestPortfolio;
            ViewData["popular"] = mostPopular;
            ViewData["top"] = topPortfolio;

            return View(displayPortfolios);
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
            return View(displayPortfolios);
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
                return View(portfolio);
            } else
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
            } else
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
            } else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolio.Any(e => e.ID == id);
        }
    }
}
