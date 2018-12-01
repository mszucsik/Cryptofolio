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

        // GET: Index
        /// <summary>
        /// The homepage of the app, various major statistics and chart data are shown here
        /// </summary>
        /// <remarks>The view bag contains statistics as well as a few charts of global stats of the application</remarks>
        /// <returns>Returns the index view</returns>
        /// 
        public async Task<IActionResult> Index()
        {
            List<Portfolio> portfolios = await _context.Portfolio.ToListAsync();
            MarketPrice latestPrice = await _context.MarketPrice.OrderByDescending(o => o.TimeStamp).FirstAsync();
            List<MarketPrice> marketPrices = await _context.MarketPrice.OrderBy(o => o.TimeStamp).ToListAsync();


            List<Holding> holdings = await _context.Holding.ToListAsync();
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
            foreach (MarketPrice m in marketPrices)
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
                foreach (Holding h in holdings)
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

            ViewBag.count = portfolios.Count;
            ViewBag.total = tempTotalUSD;
            ViewBag.daychange = tempPercent;
            ViewBag.chartDates = chartDates.Skip(1).ToArray();
            ViewBag.chartRatesUSD = chartRatesUSD.Skip(1).ToArray();
            ViewBag.chartRatesPercent = chartRatesPercent.Skip(1).ToArray();
            ViewBag.chartRatesBTC = chartRatesBTC.Skip(1).ToArray();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       

    }
}
