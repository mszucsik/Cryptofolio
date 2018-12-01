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
using System.ComponentModel.DataAnnotations;

namespace Cryptofolio.Models
{
    // A claim of an asset that is in a portfolio, (ex. I bought 1 BTC and 5 ETH, so I have 2 asset holdings)
    public class Holding
    {
        [Key]
        public int ID { get; set; }

        public string OwnerID { get; set; }

        public int Portfolio_ID { get; set; }

        public DateTime Creation_Date { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public string AssetType { get; set; }

        [Required]
        [Range(0, 100000000)]
        public double Amount { get; set; }

        [Required]
        [Display(Name = "Purchase Price")]
        [Range(0, 100000)]
        public double PurchasePrice { get; set; }
    }

}
