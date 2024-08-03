using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class InstallmentManagement
    {
        [Key]
        public int installment_id { get; set; }

        [Required]
        public int mode { get; set; }

        [Required]
        public int fee_for { get; set; }

        [Required]
        public int is_active { get; set; }

        [Display(Name = "Admin")]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

        [ForeignKey("added_by")]
        public Admin admin { get; set; }
    }
}