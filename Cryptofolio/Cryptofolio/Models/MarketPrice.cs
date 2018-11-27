using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptofolio.Models
{
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

