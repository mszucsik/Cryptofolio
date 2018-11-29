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
            return View(await _context.MarketPrice.OrderBy(o=>o.TimeStamp).ToListAsync());
        }

        private bool MarketPriceExists(int id)
        {
            return _context.MarketPrice.Any(e => e.ID == id);
        }
    }
}
