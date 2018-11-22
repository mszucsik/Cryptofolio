using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cryptofolio.Models
{
    public class Asset
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        [EnumDataType(typeof(AssetCode))]
        public AssetCode Code { get; set; }

        public double CurrentPrice { get; set; }

        public double CurrentHigh { get; set; }

        public double CurrentLow { get; set; }
    }

    public enum AssetCode
    {
        BTC = 1,
        ETH = 2,
        XLM = 3,
        XRP = 4
    }
}

