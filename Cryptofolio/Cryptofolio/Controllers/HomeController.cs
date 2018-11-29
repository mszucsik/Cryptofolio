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
            double altValue = 0;
            portfolios = getPortfolioStatistics(portfolios);
            foreach (Portfolio portfolio in portfolios)
            {
                total += portfolio.USD_Value;
                altValue += portfolio.BTC_Value;
            }

            List<String> chartRates = new List<String>();
            List<String> chartDates = new List<String>();
            List<MarketPrice> marketRates = await _context.MarketPrice.OrderBy(o => o.TimeStamp).ToListAsync();
            DateTime temp = DateTime.Now.AddDays(-31);
            double tempTotal = 0;
            foreach (MarketPrice m in marketRates) {
                if (temp < m.TimeStamp)
                {
                    chartDates.Add(m.TimeStamp.ToString());
                    chartRates.Add(tempTotal.ToString());
                    temp = m.TimeStamp;
                    tempTotal = 0;
                }
                else if (temp == m.TimeStamp)
                {
                    tempTotal += m.CurrentPrice;
                }
            }
            ViewBag.count = portfolios.Count;
            ViewBag.total = total;
            ViewBag.daychange = (total - altValue) / total * 100;
            ViewBag.chartRates = chartRates;
            ViewBag.chartDates = chartDates;
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
