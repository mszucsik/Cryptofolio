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

        public virtual string Portfolio_Name { get; set; }

        public DateTime Creation_Date { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }
    }

}
