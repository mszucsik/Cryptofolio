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
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Json;
using Microsoft.AspNetCore.Authorization;

namespace Cryptofolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BittrexAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BittrexAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        /// <summary>
        /// This method was intended to pull API data to build market prices
        /// <remark>
        /// The parsing of the data returned combined with the difficulty of
        /// running a cronjob on Azure lead me to leave this section out for now.
        /// </remark>
        /// </summary>
        /// <returns>Asset results from Bittrex's API</returns>
        /// 
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
