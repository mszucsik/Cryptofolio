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
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        /// <summary>
        /// A list of all ratings
        /// </summary>
        /// <remarks>The admin can view ratings given by users</remarks>
        /// <returns>Returns a list of Ratings</returns>
        /// 
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

    }
}
