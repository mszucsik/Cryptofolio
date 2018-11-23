using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cryptofolio.Models;
using Cryptofolio.Data;
using Microsoft.EntityFrameworkCore;

namespace Cryptofolio.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Portfolio> portfolios = await _context.Portfolio.ToListAsync();
            double total = 0;
            foreach (Portfolio portfolio in portfolios)
            {
                total += portfolio.USD_Value;
            }
            ViewBag.count = portfolios.Count;
            ViewBag.total = total;
            ViewBag.daychange = -4.17;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
