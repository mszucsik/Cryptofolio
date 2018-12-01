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
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        public string OwnerID { get; set; }

        public int Portfolio_ID { get; set; }

        // To load the name of the portfolio when needed
        public virtual string Portfolio_Name { get; set; }

        public DateTime Creation_Date { get; set; }

        [StringLength(1000)] // Comments can be a whole book, why not
        public string Message { get; set; }
    }

}
