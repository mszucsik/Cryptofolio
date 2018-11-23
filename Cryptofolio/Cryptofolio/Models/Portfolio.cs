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

        public string OwnerID { get; set; }

        [Required]
        public string Name { get; set; }

        public int Rating { get; set; }

        public virtual double Percent_Change { get; set; }

        public virtual double BTC_Value { get; set; }

        public virtual double USD_Value { get; set; }

        [DataType(DataType.Date)]
        public DateTime Creation_Date { get; set; }

        [Required]
        [Display(Name = "Private?", Description = "If this is unchecked other users can see your portfolio")]
        public bool Privacy_Status { get; set; }

        public ICollection<Holding> Holdings { get; set; }
    }
}
