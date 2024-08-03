using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class Fines
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Fine After 10 Days of Due Date is required.")]
        public int fine_after_10_days { get; set; }

        [Required(ErrorMessage = "Fine After 30 Days of Due Date is required.")]
        public int fine_after_30_days { get; set; }

        [Required(ErrorMessage = "Fine After 60 Days of Due Date is required.")]
        public int fine_after_60_days { get; set; }

        [Required(ErrorMessage = "Fine For is required.")]
        public int fine_for { get; set; }

        [Required(ErrorMessage = "Fine For Session is required.")]
        public int session { get; set; }
    }
}