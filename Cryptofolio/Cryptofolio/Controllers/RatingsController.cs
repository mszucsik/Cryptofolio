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
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MarketPrices
        public async Task<IActionResult> Index()
        {
            var ratings = await _context.Rating.OrderBy(o => o.Creation_Date).ToListAsync();

            foreach(Rating r in ratings)
            {
                var portfolio = await _context.Portfolio.Where(o => o.ID == r.Portfolio_ID).FirstOrDefaultAsync();
                r.Portfolio_Name = portfolio.Name;
            }
                
            return View(ratings);
        }

        private bool MarketPriceExists(int id)
        {
            return _context.Rating.Any(e => e.ID == id);
        }
    }
}
