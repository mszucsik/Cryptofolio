using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cryptofolio.Models
{
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
        public double Amount { get; set; }

        [Required]
        [Display(Name = "Purchase Price")]
        public double PurchasePrice { get; set; }
    }

}
