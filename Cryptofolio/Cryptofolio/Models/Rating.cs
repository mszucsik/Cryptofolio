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
    public class Rating
    {
        [Key]
        public int ID { get; set; }

        public string OwnerID { get; set; }

        public int Portfolio_ID { get; set; }

        // Load in the portfolio name for comments/some admin areas
        public virtual string Portfolio_Name { get; set; }

        public DateTime Creation_Date { get; set; }

        // To be added or subtracted from a rating of 50
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
