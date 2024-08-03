using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NUMLPay_WebApp.Models
{
    public class FeePlan
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string name { get; set; }

        [Required(ErrorMessage = "Discount is required in Percentage.")]
        public int discount { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(30)]
        [Display(Name = "Admin")]
        public string added_by { get; set; }

        [ForeignKey("added_by")]
        public Admin admin { get; set; }

        [Required]
        public int is_active { get; set; }
    }
}