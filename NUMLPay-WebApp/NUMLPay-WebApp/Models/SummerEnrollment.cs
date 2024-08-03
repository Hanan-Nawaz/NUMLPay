using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class SummerEnrollment
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name = "summerFee")]
        public int summer_fee_id { get; set; }

        [ForeignKey("summer_fee_id")]
        public SummerFee summerFee { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "users")]
        public string std_numl_id { get; set; }

        [ForeignKey("std_numl_id")]
        public Users users { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(30)]
        public string added_by { get; set; }

    }
}