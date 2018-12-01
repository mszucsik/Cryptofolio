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
    [Authorize(Roles = "Admin")]
    public class MarketPricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketPricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        /// <summary>
        /// A list of all market prices that have been gathered
        /// </summary>
        /// <remarks>The intention of this is to track odd values that may come in through the API
        /// feeding the database. The admin can check to see what prices may be causing issues</remarks>
        /// <returns>Returns a list of MarketPrice</returns>
        /// 
        public async Task<IActionResult> Index()
        {
            return View(await _context.MarketPrice.OrderBy(o=>o.TimeStamp).ToListAsync());
        }
    }
}
