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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptofolio.Models
{
    // Market prices are a given asset's price at a given time,
    // there is a ton of this data needed to produce complex graphs
    public class MarketPrice
    {

        [Key]
        public int ID { get; set; }

        [Display(Name = "Price")]
        public double CurrentPrice { get; set; }

        [Display(Name = "Price Date")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Logo")]
        public string LogoUrl { get; set; }

        [Display(Name = "Code")]
        public string MarketCurrency { get; set; }

        [Display(Name = "Name")]
        public string MarketCurrencyLong { get; set; }
    }
}

