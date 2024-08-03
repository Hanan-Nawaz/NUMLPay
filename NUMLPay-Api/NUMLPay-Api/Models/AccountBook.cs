using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class AccountBook
    {

        [Key]
        public int account_id { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "users")]
        public string std_numl_id { get; set; }

        [ForeignKey("std_numl_id")]
        public Users users { get; set; }

        [Required]
        [Display(Name = "FeeInstallments")]
        public int challan_no { get; set; }

        [ForeignKey("challan_no")]
        public FeeInstallments feeInstallments { get; set; }

    }
}