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

        [Display(Name = "Display Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public string Code { get; set; }

        public Boolean Activated { get; set; }
    }
}

