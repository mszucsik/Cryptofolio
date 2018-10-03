using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cryptofolio.Models
{
    public class Portfolio
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public int Rating { get; set; }

        [Required]
        public DateTime Creation_Date { get; set; }

        [Required]
        public bool Privacy_Status { get; set; }
    }
}
