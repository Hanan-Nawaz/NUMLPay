using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class FeeInstallments
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name = "UnpaidFees")]
        public int challan_id { get; set; }

        [ForeignKey("challan_id")]
        public UnpaidFees unpaidFees { get; set; }

        [Required]
        public int installment_no { get; set; }

        [Required]
        public float total_fee { get; set; }

        public float fine { get; set; }

        [Required]
        public string due_date { get; set; }

        [Required]
        public string valid_date { get; set; }

        public string paid_date { get; set; }

        [Required]
        public int status { get; set; }

        [Required]
        public int payment_method { get; set; }
    }
}