using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NUMLPay_Api.Models
{
    public class ChallanVerification
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name = "FeeInstallments")]
        public int fee_installment_id { get; set; }

        [ForeignKey("fee_installment_id")]
        public FeeInstallments feeInstallments { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(8000)]
        public string image { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "Admin")]
        public string verified_by { get; set; }

        [ForeignKey("verified_by")]
        public Admin admin { get; set; }
    }
}