using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cryptofolio.Data;
using Cryptofolio.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Json;

namespace Cryptofolio.Controllers
{
    public class BittrexAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BittrexAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MarketPrices
        public async Task<IActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync("https://bittrex.com/api/v2.0/pub/markets/GetMarketSummaries");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();

                    JsonObject parsedData = (JsonObject)JsonObject.Parse(stringResult);
                    JsonArray resultList = new JsonArray(parsedData["result"]);


                    
                    

                    
                    return Ok(new
                    {
                        stringResult
                    });
                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting data from Bittrex: {httpRequestException.Message}");
                }
            }
        }

    }
}
