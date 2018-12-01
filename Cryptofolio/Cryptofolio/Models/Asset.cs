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
    // BTC, ETH, etc
    public class Asset
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Display Name")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public string Code { get; set; }

        public Boolean Activated { get; set; }
    }
}

