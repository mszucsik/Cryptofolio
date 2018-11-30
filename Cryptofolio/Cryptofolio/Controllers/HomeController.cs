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
            List<MarketPrice> marketPrices = await _context.MarketPrice.OrderBy(o => o.TimeStamp).ToListAsync();


            List<Holding> holdings = await _context.Holding.ToListAsync();
            DateTime temp = DateTime.Now.AddDays(-33);
            double tempTotalUSD = 0;
            double tempTotalBTC = 0;
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
                    tempTotalUSD = 0;
                    tempTotalBTC = 0;
                }
                foreach (Holding h in holdings)
                {
                    if (m.MarketCurrency == h.AssetType)
                    {
                        tempTotalUSD += h.Amount * m.CurrentPrice;
                        tempTotalBTC += h.Amount * m.CurrentPrice / tempBTC;
                    }
                }
            }



            ViewBag.count = portfolios.Count;
            ViewBag.total = tempTotalUSD;
            ViewBag.daychange = 20;
            ViewBag.chartDates = chartDates.Skip(1).ToArray();
            ViewBag.chartRatesUSD = chartRatesUSD.Skip(1).ToArray();
            ViewBag.chartRatesBTC = chartRatesBTC.Skip(1).ToArray();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
                p.BTC_Value = oldTotal;
                p.USD_Value = totalUSD;
                p.Total_Purchased = totalPurchased;
                p.Percent_Change = percentChange;

            }

            return portfolios;
        }

    }
}
