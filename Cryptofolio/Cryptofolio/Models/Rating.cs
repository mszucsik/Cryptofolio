using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cryptofolio.Models
{
    public class Rating
    {
        [Key]
        public int ID { get; set; }

        public string OwnerID { get; set; }

        public int Portfolio_ID { get; set; }

        public DateTime Creation_Date { get; set; }

        [EnumDataType(typeof(RatingType))]
        public RatingType Vote { get; set; }
    }

    public enum RatingType
    {
        Down = -1,
        None = 0,
        Up = 1
    }

}
