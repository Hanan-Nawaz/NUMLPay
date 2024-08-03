using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class Fines
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int fine_after_10_days { get; set; }

        [Required]
        public int fine_after_30_days { get; set; }

        [Required]
        public int fine_after_60_days { get; set; }

        [Required]
        public int fine_for { get; set; }

        [Required]
        public int session { get; set; }
    }
}