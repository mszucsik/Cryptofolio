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
        [Display(Name = "Portfolio Name")]
        public string Name { get; set; }

        public int Rating { get; set; }

        [Display(Name = "Daily Change")]
        public virtual double Daily_Change { get; set; }

        [Display(Name = "Percent Change")]
        public virtual double Percent_Change { get; set; }

        [Display(Name = "Total Purchased")]
        public virtual double Total_Purchased { get; set; }

        [Display(Name = "TotalChange")]
        public virtual double Total_Change { get; set; }

        [Display(Name = "BTC Value")]
        public virtual double BTC_Value { get; set; }

        [Display(Name = "USD Value")]
        public virtual double USD_Value { get; set; }

        [DataType(DataType.Date)]
        public DateTime Creation_Date { get; set; }

        [Required]
        [Display(Name = "Private", Description = "If this is unchecked other users can see your portfolio")]
        public bool Privacy_Status { get; set; }

        public ICollection<Holding> Holdings { get; set; }
    }
}
