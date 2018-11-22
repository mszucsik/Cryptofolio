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

        public DateTime Creation_Date { get; set; }

        public Asset AssetType { get; set; }

        public double Amount { get; set; }
    }

}
