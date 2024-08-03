using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_Api.Models
{
    public class UnpaidFees
    {
        [Key]
        public int challan_no { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "Users")]
        public string std_numl_id { get; set; }

        [ForeignKey("std_numl_id")]
        public Users users { get; set; }

        [Display(Name = "Challans")]
        public int? challan_id { get; set; }

        [ForeignKey("challan_id")]
        public Challans challans { get; set; }

        [Display(Name = "InstallmentManagement")]
        public int? fee_instalments_id { get; set; }

        [ForeignKey("fee_instalments_id")]
        public InstallmentManagement installmentManagement { get; set; }

        [Required]
        public int fee_id { get; set; }

        [Required]
        public int fee_type { get; set; }

        [Required]
        public int semester { get; set; }

        [Required]
        public string issue_date { get; set; }

        public int security { get; set; }
    }
}